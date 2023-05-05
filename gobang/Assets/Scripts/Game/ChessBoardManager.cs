using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using Newtonsoft.Json;

public class ChessBoardManager : MonoBehaviour
{

    public static ChessBoardManager Instance { get; set; }


    [SerializeField] private GameObject blocker;
    [SerializeField] GameObject retract_Message;
    [SerializeField] private GameObject wait_Message;
    [SerializeField] private Transform white_stone;
    [SerializeField] private Transform black_stone;
    [SerializeField] private AudioClip playStoneClip;
    [SerializeField] private Button retract_button;
    [SerializeField] private GameObject ExitBtn;


    private int current_stone_color = -1;    //  1 = white,  -1 = black
    private int my_stone_color = -1;     //  1 = white,  -1 = black
    //private int x, y;
    private int steps;

    private Transform stones_parent;
    AudioSource audioSource;
    
    Transform[,] grid_transforms = new Transform[19, 19];
    Transform[,] stones_transforms = new Transform[19, 19];
    int[,] chess_board_grids = new int[19, 19];

    Vector2 offset = new Vector2(-3.969f, -3.969f);
    float cell_size = 0.441f;

    
    private void Awake()
    {
        Instance = this;

        audioSource = gameObject.GetComponent<AudioSource>();
        stones_parent = transform.Find("Stones_parent");

        int _x = 0, _y = 0;

        Transform grids = transform.Find("Grids");
        Transform[] childTransforms = grids.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childTransforms)
        {
            if (!childTransform.gameObject.CompareTag("grid"))
                continue;


            Vector2 childPosition = new Vector2(_x * cell_size, _y * cell_size);
            childPosition += offset;
            childTransform.parent = grids;
            childTransform.localPosition = childPosition;
            childTransform.GetComponent<grid_script>().Set_Index(_x, _y);
            grid_transforms[_x, _y] = childTransform;
            _x++;
            if (_x > 18) { _x = 0; _y++; }
        }
    }


    private void OnEnable()
    {
        //Init(ClassManager.GameMode.singleplayer);
        Reset_GoBoard();
    }

    public void Init(ClassManager.GameMode mode) {

        Init_Game(mode, null, null, null);
    }

    public int Get_My_Stone_Color() {
        return my_stone_color;
    }

    private void Step_Update(int _step) {
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.randomplayer)
            return;
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.spector)
            return;
        if (my_stone_color == 10)
            return;

        steps += _step;
        if (steps < 2)
        {
            retract_button.interactable = false;
        }
        else {
            retract_button.interactable = true;
        }
    }

    public void Init_Game(ClassManager.GameMode mode, string startTime, ClassManager.PlayerLog _p1Log, ClassManager.PlayerLog _p2Log) {

        audioSource.clip = playStoneClip;
        steps = 0;


        if (mode == ClassManager.GameMode.singleplayer)
        {
            MatchData.Init(mode, null, null, null);
            blocker.SetActive(false);
        }
        else if (mode == ClassManager.GameMode.randomplayer)
        {
            ClassManager.PlayerLog randomPlayer = new("randomPlayer");
            string formattedDateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            MatchData.Init(mode, formattedDateTime, PlayerLocalData.GetPlayerLog(), randomPlayer);
            my_stone_color = -1;
            blocker.SetActive(false);
            OpponentData.SetPlayerLog(randomPlayer);
            GameManager.Instance.OnMatchStart();
        }
        else if (mode == ClassManager.GameMode.multiplayer)
        {
            string myName = PlayerLocalData.GetPlayerName();
            ClassManager.PlayerLog player1Log = _p1Log;
            ClassManager.PlayerLog player2Log = _p2Log;


            MatchData.Init(mode, startTime, player1Log, player2Log);
            


            if (player1Log.playerName == PlayerLocalData.GetPlayerName())
            {
                my_stone_color = -1;
                blocker.SetActive(false);
                OpponentData.SetPlayerLog(player2Log);

            }
            else if (player2Log.playerName == PlayerLocalData.GetPlayerName())
            {
                my_stone_color = 1;
                blocker.SetActive(true);
                OpponentData.SetPlayerLog(player1Log);
            }
            else {
                my_stone_color = 10;
                blocker.SetActive(true);

            }

            GameManager.Instance.OnMatchStart();
        }
        else if (mode == ClassManager.GameMode.spector)
        {
            MatchData.Init(mode, null, null, null);
            blocker.SetActive(true);

        }

        Step_Update(0);
        Reset_GoBoard();

    }


    public void Send_Message_To_Opponent(ClassManager.MessageType type, string message) {
        NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(type, message, OpponentData.GetPlayerName());
        //NetworkConnecter.Instance.Send_Message_To_Opponent_ServerRpc(type, MatchData.Get_Room_ID(), message);
    }

    public void Send_Message_To_Players(ClassManager.MessageType type, string message)
    {
        //NetworkConnecter.Instance.Send_Message_To_Players_ServerRpc(type, MatchData.Get_Room_ID(), message);
    }
    public void Send_Message_To_Room(ClassManager.MessageType type, string message)
    {
        foreach (string pname in PlayerLocalData.Get_Current_Room_Info().Get_All_Name())
        {
            NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(type, message, pname);
        }
        //NetworkConnecter.Instance.Send_Message_To_Room_ServerRpc(type, MatchData.Get_Room_ID(), message);
    }


    public void Response_To_Retract(string message)
    {

        wait_Message.SetActive(false);
        int retract_steps = int.Parse(message);
        //int retract_steps = JsonConvert.DeserializeObject<int>(message);

        if (retract_steps == 0)
        {
            PopMessageUI.Instance.Pop_Fail_Message("Retract is rejected");
        }
        else
        {
            for (int i = 0; i < retract_steps; i++)
            {
                Retract_Move();
            }
        }

    }


    public void Place_A_Stone_Multiplayer(string jsonR) {
        Vector2Int xy_index = JsonConvert.DeserializeObject<Vector2Int>(jsonR);
        Place_A_Stone(xy_index.x, xy_index.y);
    }

    public void Place_A_Stone(int x, int y)
    {

        grid_transforms[x, y].gameObject.SetActive(false);
        Vector3 vec3 = new(x * cell_size + offset.x, y * cell_size + offset.y, 0);
        Transform newStone;
        if (current_stone_color == 1)
            newStone = Instantiate(white_stone);
        else
            newStone = Instantiate(black_stone);
        newStone.parent = stones_parent;
        newStone.localPosition = vec3;
        newStone.tag = "stone";
        stones_transforms[x, y] = newStone;
        chess_board_grids[x, y] = current_stone_color;

        if (ClassManager.GameMode.spector != MatchData.Get_Game_Mode()) {
            MatchData.Make_A_Move(new Vector2Int(x, y));
            Step_Update(1);
            Play_Sound();
            Check_Win_Condition(x, y);
        }

    }

    public void Ask_For_Retract() {
        wait_Message.SetActive(true);
        Send_Message_To_Opponent(ClassManager.MessageType.ask_for_retract, null);
        //NetworkConnecter.Instance.Asking_For_Retract_ServerRpc(MatchData.Get_Room_ID());
    }
    public void Receive_Retract_Message()
    {
        retract_Message.SetActive(true);
    }



    public void Response_To_Request(bool isAgree) {
        if (isAgree)
        {
            blocker.SetActive(true);
            if (current_stone_color == my_stone_color)
            {
                Send_Message_To_Room(ClassManager.MessageType.response_to_retract, "1");
            }
            else {
                Send_Message_To_Room(ClassManager.MessageType.response_to_retract, "2");
            }

        }
        else {
            Send_Message_To_Opponent(ClassManager.MessageType.response_to_retract, "0");
        }
        retract_Message.SetActive(false);
        //wait_Message.SetActive(false);
    }


    private void Retract_Move() {
        Vector2Int vec2 = MatchData.Retract_A_Move();
        chess_board_grids[vec2.x, vec2.y] = 0;
        Destroy(stones_transforms[vec2.x, vec2.y].gameObject);
        grid_transforms[vec2.x, vec2.y].gameObject.SetActive(true);
        Change_Turn();
        Step_Update(-1);
    }

    private void Play_Sound()
    {
        audioSource.clip = playStoneClip;
        audioSource.Play();
    }

    private void Check_Win_Condition(int x, int y) {
        int _x, _y, counter;


        // check hori
        _x = x;
        _y = y;
        counter = 0;
        while (0 <= _x && _x <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x--;
        }
        _x++;
        while (0 <= _x && _x <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x++;
            counter++;
        }
        if (counter >= 5) {
            Gameover();
            return;
        }


        // check vert
        _x = x;
        _y = y;
        counter = 0;
        while (0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _y--;
        }
        _y++;
        while (0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _y++;
            counter++;
        }
        if (counter >= 5)
        {
            Gameover();
            return;
        }

        // check ¥´±× LB-RT
        _x = x;
        _y = y;
        counter = 0;
        while (0 <= _x && _x <= 18 && 0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x--;
            _y--;
        }
        _x++;
        _y++;
        while (0 <= _x && _x <= 18 && 0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x++;
            _y++;
            counter++;
        }
        if (counter >= 5)
        {
            Gameover();
            return;
        }

        // check ¥´±× LT-RB
        _x = x;
        _y = y;
        counter = 0;
        while (0 <= _x && _x <= 18 && 0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x--;
            _y++;
        }
        _x++;
        _y--;
        while (0 <= _x && _x <= 18 && 0 <= _y && _y <= 18 && chess_board_grids[_x, _y] == current_stone_color)
        {
            _x++;
            _y--;
            counter++;
        }
        if (counter >= 5)
        {
            Gameover();
            return;
        }

        Change_Turn();
    }

    private void Change_Turn()
    {
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.singleplayer)
        {
            current_stone_color *= -1;
        }
        else if (MatchData.Get_Game_Mode() == ClassManager.GameMode.randomplayer)
        {
            current_stone_color *= -1;
            blocker.SetActive(my_stone_color != current_stone_color);
            InGameMenuUI.Instance.ChangeArrow(current_stone_color);
            int randomX, randomY;
            if (my_stone_color != current_stone_color)
            {
                while (true)
                {
                    randomX = UnityEngine.Random.Range(0, 19);
                    randomY = UnityEngine.Random.Range(0, 19);
                    if (chess_board_grids[randomX, randomY] == 0)
                        break;

                }
                Place_A_Stone(randomX, randomY);
            }

        }
        else if (MatchData.Get_Game_Mode() == ClassManager.GameMode.multiplayer)
        {
            current_stone_color *= -1;
            
            blocker.SetActive(my_stone_color != current_stone_color);
            InGameMenuUI.Instance.ChangeArrow(current_stone_color);
        }
        else if (MatchData.Get_Game_Mode() == ClassManager.GameMode.spector) {
            current_stone_color *= -1;
        }

    }

    private void Reset_GoBoard() {

        current_stone_color = -1;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                chess_board_grids[i, j] = 0;
            }
        }

        foreach (Transform grid in grid_transforms) {
            grid.gameObject.SetActive(true);
        }

        //Transform[] allStone = stones_parent.GetComponentsInChildren<Transform>();
        foreach (Transform s in stones_transforms)
        {
            if (s == null)
                continue;
            if (!s.gameObject.CompareTag("stone"))
                continue;

            Destroy(s.gameObject);
        }
    }

    int display_index;
    List<(int, int)> stoneList = new();
    public void Display_Stone_List(List<(int, int)> _stoneList) {
        stoneList = _stoneList;
        Init(ClassManager.GameMode.spector);

        for (int i = 0; i < stoneList.Count; i++) {
            (int, int) pos = stoneList[i];
            Place_A_Stone(pos.Item1, pos.Item2);
            Change_Turn();
        }
        display_index = stoneList.Count - 1;
        Change_Turn();
    }

    public void Display_Backward() {
        if (display_index < 0)
            return;
           
        (int, int) pos = stoneList[display_index];
        chess_board_grids[pos.Item1, pos.Item2] = 0;
        Destroy(stones_transforms[pos.Item1, pos.Item2].gameObject);
        grid_transforms[pos.Item1, pos.Item2].gameObject.SetActive(true);
        Change_Turn();
        display_index--;
    }

    public void Display_Forward() {

        if (display_index >= stoneList.Count - 1)
            return;
        display_index++;
        (int, int) pos = stoneList[display_index];
        Change_Turn();
        Place_A_Stone(pos.Item1, pos.Item2);
        Play_Sound();
    }



    private void Gameover()
    {
        blocker.SetActive(true);
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.singleplayer)
        {

        }
        else if (MatchData.Get_Game_Mode() == ClassManager.GameMode.randomplayer)
        {
            bool isWinner = my_stone_color == current_stone_color;
            InGameMenuUI.Instance.Stop_Timering();
            MatchData.Match_Over_Set(current_stone_color, isWinner);
            MatchData.Upload_Match_Data();
            if (isWinner)
            {
                
                print("You win");
                
            }
            else
            {
                print("You lose");
            }
            
            GameManager.Instance.OnMatchOver();
        }
        else if (MatchData.Get_Game_Mode() == ClassManager.GameMode.multiplayer) {
            bool isWinner = my_stone_color == current_stone_color;
            InGameMenuUI.Instance.Stop_Timering();
            MatchData.Match_Over_Set(current_stone_color, isWinner);
            if(my_stone_color != 10)
                MatchData.Add_Temp_Game_Records();
            if (isWinner)
            {
                MatchData.Upload_Match_Data();

            }
            if (my_stone_color != 10)
                GameManager.Instance.OnMatchOver();
            else
            { 
                ExitBtn.SetActive(true);
            }
        }
    }



}
