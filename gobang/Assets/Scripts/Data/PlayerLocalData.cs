using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
public static class PlayerLocalData
{

    /*
     *  This script is to store the player local data as a gameobject.
     *  Such as player log, friend_dict, game_records, temp_game_records, background_list (chessboard skin)
     * 
     */

    private static ClassManager.PlayerLog log;
    private static string targetIP = "127.0.0.1";
    private static Dictionary<string, int> friend_dict = new();
    static List<string> temp_friend_dict = new();

    static List<ClassManager.GameRecord> game_records = new();

    static List<ClassManager.GameRecord> temp_game_records = new();

    static int roomID = -1;
    static ClassManager.State playerState;
    static Dictionary<int, ClassManager.Room> roomsList = new();

    static List<int> background_list = new();

    static int selectedBackground = 0;

    public static void Set_SelectedBackground(int num) {
        selectedBackground = num;
    }
    public static int Get_SelectedBackground()
    {
        return selectedBackground;
    }

    public static void Set_Background_Dict(List<int> list) {
        background_list = list;

    }
    public static List<int> Get_Background_Dict()
    {
        return background_list;
    }


    public static ClassManager.State Get_PlayerState() {
        return playerState;
    }

    public static void Set_PlayerState(ClassManager.State state)
    {
        playerState = state;
        if (state == ClassManager.State.playing || state == ClassManager.State.waitingRoom)
        {
            foreach (KeyValuePair<string, int> fd in friend_dict)
            {
                if (fd.Value == 0)
                    continue;
                string message = JsonConvert.SerializeObject(GetPlayerName());
                NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.friend_playing, message, fd.Key);
            }
        }
        else if (state == ClassManager.State.onMenu) {
            foreach (KeyValuePair<string, int> fd in friend_dict)
            {
                if (fd.Value == 0)
                    continue;
                string message = JsonConvert.SerializeObject(GetPlayerName());
                NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.friend_onMenu, message, fd.Key);
            }
        }

    }

    public static void Set_Rooms_List(Dictionary<int, ClassManager.Room> _roomsList) {
        roomsList = _roomsList;
    }

    public static void Update_Rooms_List((int, string, int) _message) {
        if (roomsList.ContainsKey(_message.Item1))
        {
            roomsList[_message.Item1].Update_Room(_message.Item2, _message.Item3);
            if (roomID == _message.Item1)
            {
                WaitingRoomMenuUI.Instance.Init(roomsList[_message.Item1]);
            }

            if (roomsList[_message.Item1].Get_PlayerNum() == 0)
            {
                roomsList.Remove(_message.Item1);
            }

        }
        else {

            ClassManager.Room room = new(_message.Item1, _message.Item2);
            roomsList.Add(_message.Item1, room);
        }


    }
    public static void Update_Rooms_List(int _message)
    {
        if (roomsList.ContainsKey(_message))
        {
            roomsList[_message].Change_State();
        }

    }

    public static Dictionary<int, ClassManager.Room> Get_Rooms_List()
    {
        return roomsList;
    }
    public static ClassManager.Room Get_Current_Room_Info()
    {
        return roomsList[roomID];
    }


    public static void Set_RoomID(int _ID) {
        roomID = _ID;
    }
    public static int Get_RoomID()
    {
        return roomID;
    }

    public static void Set_Game_Records(List<ClassManager.GameRecord> _gameRecords) {
        game_records = _gameRecords;
    }

    public static ClassManager.GameRecord Get_Game_Records() {
        if (game_records.Count <= 0)
            return null;

        ClassManager.GameRecord temp = game_records[0];
        game_records.RemoveAt(0);
        return temp;        
    }

    public static void Add_Temp_Game_Records(ClassManager.GameRecord game) {
        temp_game_records.Add(game);
    }

    public static void Set_Temp_Game_Records(List<ClassManager.GameRecord> _gameRecords)
    {
        temp_game_records = _gameRecords;
    }

    public static ClassManager.GameRecord Get_Temp_Game_Records()
    {
        if (temp_game_records.Count <= 0)
            return null;

        ClassManager.GameRecord temp = temp_game_records[0];
        temp_game_records.RemoveAt(0);
        return temp;
    }




    public static void Set_Friend_Dict(Dictionary<string, int> _dict)
    {
        friend_dict = _dict;
    }

    public static void Add_New_Friend(string _name, int _state)
    {
        friend_dict.Add(_name, _state);
        if (FriendListMenuUI.Instance != null)
        {
            FriendListMenuUI.Instance.Refresh();
        }
        //temp_friend_dict.Add(_name);
    }

    public static (string, int) Get_Temp_New_Friend() {
        if (temp_friend_dict.Count <= 0)
            return (null, 0);

        string _name = temp_friend_dict[0];
        temp_friend_dict.RemoveAt(0);
        return (_name, friend_dict[_name]);
    }

    public static Dictionary<string, int> Get_Friend_Dict()
    {
        return friend_dict;
    }

    public static void Update_Friend_Dict(string _name, int _state) {
        if (_state == 5)
        {
            friend_dict.Remove(_name);
        }
        else
        {
            friend_dict[_name] = _state;
        }

  
        if (FriendListMenuUI.Instance != null)
        {
            FriendListMenuUI.Instance.Refresh();
        }
    }

    public static string[] Get_All_Friend_Name() {
        string[] keysArray = new string[friend_dict.Count];
        int i = 0;
        foreach (string key in friend_dict.Keys)
        {
            keysArray[i] = key;
            i++;
        }

        return keysArray;
    }



    public static bool Friend_Exist(string _name) {
        if (friend_dict.ContainsKey(_name))
            return true;
        return false;
    }






    public static void SetPlayerLog(ClassManager.PlayerLog _log) { log = _log; }

    public static ClassManager.PlayerLog GetPlayerLog() { return log; }

    public static string GetPlayerName() { return log.playerName; }
    public static int GetPlayerScore() { return log.score; }

    public static int GetPlayerCoin() { return log.coin; }

    public static string GetTargetIP() { return targetIP; }

    public static void SetTargetIP(string IP) { targetIP = IP; }


    public static void UpdataScore(int score)
    {
        log.score += score;
        FirebaseManager.Instance.Updata_Playerlog();
    }

    public static void UpdataCoin(int coin)
    {
        log.coin += coin;
        FirebaseManager.Instance.Updata_Playerlog();
    }




}
