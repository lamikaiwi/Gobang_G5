using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class InGameMenuUI : MonoBehaviour
{
    public static InGameMenuUI Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] TMP_Text player1Name;
    [SerializeField] TMP_Text player1Score;
    [SerializeField] TMP_Text player2Name;
    [SerializeField] TMP_Text player2Score;
    [SerializeField] TMP_Text Top;
    [SerializeField] Button retract_btn;
    [SerializeField] GameObject retract_Message;
    [SerializeField] Button agree_retract_btn;
    [SerializeField] Button disagree_retract_btn;
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    [SerializeField] TMP_Text startTimeText;
    [SerializeField] TMP_Text elapsedTimeText;
    [SerializeField] GameObject wait_Message;
    [SerializeField] GameObject matchOverMenu;



    [SerializeField] RectTransform chatBox;
    [SerializeField] TMP_Text chatBox_text;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button returnKey;
    [SerializeField] Button resizeBtn;


    bool timerRunning;
    float elapsedTime;

    private void Start()
    {
        retract_btn?.onClick.AddListener(() => ChessBoardManager.Instance.Ask_For_Retract());
        returnKey?.onClick.AddListener(() => Send_Message());
        resizeBtn?.onClick.AddListener(() => Resize_ChatBox());

    }

    private void OnEnable()
    {

        player1Name.text = MatchData.Get_Match_Data().player1Name;
        player1Score.text = MatchData.Get_Match_Data().player1Score.ToString();
        player2Name.text = MatchData.Get_Match_Data().player2Name;
        player2Score.text = MatchData.Get_Match_Data().player2Score.ToString();
        retract_btn.interactable = false;
        chatbox_size = 325;
        chatBox.sizeDelta = new Vector2(500f, chatbox_size);
        chatBox_text.text = "";
        inputField.text = "";
        startTimeText.text = MatchData.Get_Game_Start_Time();
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.randomplayer)
        {
            Top.text = "Random";
            inputField.interactable = false;
        }
        else
        {
            Top.text = "Multiplayer";
            inputField.interactable = true;
        }
        leftArrow.SetActive(true);
        rightArrow.SetActive(false);
        elapsedTime = 0;
        timerRunning = true;

    }

    int chatbox_size;
    void Resize_ChatBox() {
        if (chatbox_size == 325)
        {
            chatbox_size = 537;
        }
        else if (chatbox_size == 537) {
            chatbox_size = 725;
        }
        else if (chatbox_size == 725) {
            chatbox_size = 325;
        }
        chatBox.sizeDelta = new Vector2(500f, chatbox_size);
    }

    public void Send_Message() {
        if (string.Equals(inputField.text, ""))
            return;

        string message = $"{PlayerLocalData.GetPlayerName()}: {inputField.text}\n";
        inputField.text = "";
        foreach (string pname in PlayerLocalData.Get_Current_Room_Info().Get_All_Name())
        {
            NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.room_chat, message, pname);
        }
    }

    public void Receive_Chat_Message(string str) {
        chatBox_text.text += str;
    }

    public void LeaveRoom()
    {
        matchOverMenu.gameObject.SetActive(false);
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.multiplayer)
        {
            string myName = PlayerLocalData.GetPlayerName();

            (int, string, int) message = (PlayerLocalData.Get_RoomID(), myName, 4);
            print(message);
            //ClassManager.Room newRoom = roomInfo.Remove_Name(myName);

            string messageJson = JsonConvert.SerializeObject(message);

            NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, messageJson);
            PlayerLocalData.Set_RoomID(-1);
        }

        PlayerLocalData.Set_PlayerState(ClassManager.State.onMenu);
        GameManager.Instance.OnEnterMainMenu();

    }


    public void ChangeArrow(int current_stone_color) {
        if (current_stone_color == 1) {
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
        else {
            leftArrow.SetActive(true);
            rightArrow.SetActive(false);
        }
    }


    public string Get_Elapsed_Time() { 
        return string.Format("{0:00}:{1:00}", Mathf.FloorToInt(elapsedTime / 60f), Mathf.FloorToInt(elapsedTime % 60f));
    }

    public void Stop_Timering() {
        timerRunning = false;
    }

    private void Update()
    {
        if (timerRunning) {
            elapsedTime += Time.deltaTime;
            elapsedTimeText.text = Get_Elapsed_Time();
        }

    }



}
