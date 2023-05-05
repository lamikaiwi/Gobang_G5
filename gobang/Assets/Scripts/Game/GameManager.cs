using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;
using Unity.Netcode.Transports.UTP;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*
     * This script manage some important event such as
     * OnPlayerLogin, OnPlayerLogout, OnEnterMainMenu, OnRoomListMenu, OnEnterWaitingRoom,
     * OnStartRandomMode, OnMatchStart, OnMatchOver
     * 
     * And it also manage the message sending/ receiving process
     * Send_Message_To_Server, Send_Message_To_All, Send_Message_To_Target, Receive_Message
     * 
     * Send_Message_To_Server - send a message to server, the receiver is the server.
     * Send_Message_To_All -  send a message to server, and the server will send this message to every client who is online.
     * Send_Message_To_Target - send a message to server, and the server will send this message to the target client.
     * Receive_Message - called by server, it perform different action depends on the message type 
     * 
     */
    public static GameManager Instance { set; get; }


    //public event Action OnEnterMainMenu;

    [SerializeField] GameObject loginMenuUI;
    [SerializeField] GameObject signupMenuUI;
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject chessBoard;

    [SerializeField] GameObject ProfileMenuUI;
    [SerializeField] GameObject InGameMenu;
    [SerializeField] GameObject MatchOverPanel;
    [SerializeField] GameObject GameRecordMenu;
    [SerializeField] GameObject ShoppingMenu;
    [SerializeField] GameObject RoomListMenu;
    [SerializeField] GameObject WaitingRoomMenu;
    [SerializeField] GameObject friendListMenu;
    [SerializeField] GameObject InviteMessage;

    [SerializeField] GameObject BGM;

    [SerializeField] GameObject AdminMenu;


    bool isAdmin = false;
    public bool IsAdmin() { return isAdmin; }

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        FirebaseManager.Instance.GetServerIP();
    }

    void AdminMode() {
        isAdmin = true;
        AdminMenu.SetActive(true);
    }

    public void OnPlayerLogin()
    {

        loginMenuUI.SetActive(false);
        signupMenuUI.SetActive(false);

        string playerName = PlayerLocalData.GetPlayerName();
        FirebaseManager.Instance.Get_Game_Records(playerName);
        FirebaseManager.Instance.Get_Friend_Dict(playerName);
        FirebaseManager.Instance.Get_Item_List(playerName);
        ConnectToServer();

        if (playerName.Length >= 5 && playerName.Substring(0, 5) == "admin")
        {
            AdminMode();
            return;
        }

        mainMenuUI.SetActive(true);
        BGM.SetActive(true);
        //chessBoard.SetActive(true);   




    }
    public void OnPlayerLogout()
    {
        if (PlayerLocalData.Get_RoomID() != -1 && PlayerLocalData.Get_PlayerState() == ClassManager.State.waitingRoom)
        {
            WaitingRoomMenuUI.Instance.LeaveRoom();
        }
        loginMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
        chessBoard.SetActive(false);

    }


    public void OnEnterMainMenu() {

        InGameMenu.SetActive(false);
        GameRecordMenu.SetActive(false);
        RoomListMenu.SetActive(false);
        chessBoard.SetActive(false);
        WaitingRoomMenu.SetActive(false);
        ShoppingMenu.SetActive(false);

        if (!isAdmin)
        {
            mainMenuUI.SetActive(true);

        }


        //ChessBoardManager.Instance.Init(ClassManager.GameMode.singleplayer);
    }
    public void OnRoomListMenu()
    {
        //string jsonR = JsonConvert.SerializeObject(PlayerLocalData.GetPlayerLog());
        RoomListMenu.SetActive(true);
        //mainMenuUI.SetActive(false);
        //NetworkConnecter.Instance.AddPlayerToReadyListServerRpc(jsonR);
    }

    public void OnEnterWaitingRoom() {
        mainMenuUI.SetActive(false);
        GameRecordMenu.SetActive(false);
        ShoppingMenu.SetActive(false);
        RoomListMenu.SetActive(false);
        WaitingRoomMenu.SetActive(true);
    }

    public void OnStartRandomMode() {
        chessBoard.SetActive(true);
        ChessBoardManager.Instance.Init_Game(ClassManager.GameMode.randomplayer, null, null, null);

    }

    

    public void OnMatchStart()
    {
        ProfileMenuUI.SetActive(false);
        friendListMenu.SetActive(false);
        WaitingRoomMenu.SetActive(false);
        InGameMenu.SetActive(true);
        //chessBoard.SetActive(true);
        //ChessBoardManager.Instance.Init_Game();
    }

    public void OnMatchOver()
    {
        MatchOverPanel.SetActive(true);
    }

    public void StartServerByAddress(string ipAdress)       //must keep it here
    {

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ipAdress,  // IP address is a string
            (ushort)7777 
        );
        NetworkManager.Singleton.StartServer();
        print("Server Running...");
    }

    void ConnectToServer()                                  //must keep it here
    {
        //string IP = PlayerLocalData.GetTargetIP();
        string IP = PlayerLocalData.GetTargetIP();
        print("IP = " + IP);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            IP, 
            (ushort)7777 
        );
        
        NetworkManager.Singleton.StartClient();
    }

    public void Send_Message_To_Server(ClassManager.MessageType type, object obj)
    {
        string message = JsonConvert.SerializeObject(obj);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(type, message);

    }
    public void Send_Message_To_Target(ClassManager.MessageType type, object obj, string targetNames)
    {
        string message = JsonConvert.SerializeObject(obj);
        NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(type, message, targetNames);

    }
    public void Send_Message_To_All(ClassManager.MessageType type, object obj) {
        string message = JsonConvert.SerializeObject(obj);
        NetworkConnecter.Instance.Send_Message_To_All_ServerRpc(type, message);
    }

    public void Receive_Message(ClassManager.MessageType messageType, string _message)
    {
        if (messageType == ClassManager.MessageType.friend_request)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            //PopMessageUI.Instance.Friend_Request_Message(message);
            PopMessageUI.Instance.Init_Pop(messageType, message);
        }
        else if (messageType == ClassManager.MessageType.friend_request_accepted)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            PlayerLocalData.Add_New_Friend(message, 1);
        }
        else if (messageType == ClassManager.MessageType.friend_online)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            if (string.Equals(PlayerLocalData.GetPlayerName(), message))
                return;
            if (PlayerLocalData.Friend_Exist(message))
            {
                PlayerLocalData.Update_Friend_Dict(message, 1);
                if (PlayerLocalData.Get_PlayerState() == ClassManager.State.onMenu)
                {
                    Send_Message_To_Target(ClassManager.MessageType.friend_onMenu, PlayerLocalData.GetPlayerName(), message);
                }
                else if (PlayerLocalData.Get_PlayerState() == ClassManager.State.playing || PlayerLocalData.Get_PlayerState() == ClassManager.State.waitingRoom)
                {
                    Send_Message_To_Target(ClassManager.MessageType.friend_playing, PlayerLocalData.GetPlayerName(), message);
                }
            }

        }
        else if (messageType == ClassManager.MessageType.friend_offline)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            if (PlayerLocalData.Friend_Exist(message))
                PlayerLocalData.Update_Friend_Dict(message, 0);
        }
        else if (messageType == ClassManager.MessageType.friend_onMenu)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            if (PlayerLocalData.Friend_Exist(message))
                PlayerLocalData.Update_Friend_Dict(message, 1);
        }
        else if (messageType == ClassManager.MessageType.friend_playing)
        {
            string message = JsonConvert.DeserializeObject<string>(_message);
            if (PlayerLocalData.Friend_Exist(message))
                PlayerLocalData.Update_Friend_Dict(message, 2);
        }
        else if (messageType == ClassManager.MessageType.friend_deleted)
        {
            if (PlayerLocalData.Friend_Exist(_message))
                PlayerLocalData.Update_Friend_Dict(_message, 5);
        }
        else if (messageType == ClassManager.MessageType.being_deleted)
        {
            PopMessageUI.Instance.Pop_BannedMessage();
        }
        else if (messageType == ClassManager.MessageType.host_room)
        {
            ClassManager.Room message = JsonConvert.DeserializeObject<ClassManager.Room>(_message);
            WaitingRoomMenu.SetActive(true);
            WaitingRoomMenu.GetComponent<WaitingRoomMenuUI>().Init(message);
        }
        else if (messageType == ClassManager.MessageType.room_update)
        {

            (int, string, int) message = JsonConvert.DeserializeObject<(int, string, int)>(_message);
            PlayerLocalData.Update_Rooms_List(message);

            if (RoomListMenuUI.Instance != null)
            {
                RoomListMenuUI.Instance.Refresh();
            }
            //WaitingRoomMenu.GetComponent<WaitingRoomMenuUI>().Update_Room(message);
        }
        else if (messageType == ClassManager.MessageType.room_change_state)
        {

            int message = JsonConvert.DeserializeObject<int>(_message);
            PlayerLocalData.Update_Rooms_List(message);

            if (RoomListMenuUI.Instance != null)
            {
                RoomListMenuUI.Instance.Refresh();
            }

        }
        else if (messageType == ClassManager.MessageType.room_init)
        {
            (string, ClassManager.PlayerLog, ClassManager.PlayerLog) message = JsonConvert.DeserializeObject<(string, ClassManager.PlayerLog, ClassManager.PlayerLog)>(_message);
            chessBoard.SetActive(true);
            //InGameMenu.SetActive(true);

            ChessBoardManager.Instance.Init_Game(ClassManager.GameMode.multiplayer, message.Item1, message.Item2, message.Item3);

        }
        else if (messageType == ClassManager.MessageType.sync_roomList)
        {
            Dictionary<int, ClassManager.Room> temp = JsonConvert.DeserializeObject<Dictionary<int, ClassManager.Room>>(_message);
            PlayerLocalData.Set_Rooms_List(temp);


        }
        else if (messageType == ClassManager.MessageType.game_invite)
        {
            if (InviteMessage.activeSelf)
                return;
            if (PlayerLocalData.Get_PlayerState() != ClassManager.State.onMenu)
                return;
            (string, int) inviteMessage = JsonConvert.DeserializeObject<(string, int)>(_message);

            InviteMessage.SetActive(true);
            InviteMessage.GetComponent<InviteMessage>().Init(inviteMessage);

        }
        else if (messageType == ClassManager.MessageType.place_a_stone)
        {
            //InGameMenuUI.Instance.Place_A_Stone_Multiplayer(_message);
            ChessBoardManager.Instance.Place_A_Stone_Multiplayer(_message);

        }
        else if (messageType == ClassManager.MessageType.room_chat)
        {
            InGameMenuUI.Instance.Receive_Chat_Message(_message);
        }
        else if (messageType == ClassManager.MessageType.ask_for_retract)
        {
            ChessBoardManager.Instance.Receive_Retract_Message();
        }
        else if (messageType == ClassManager.MessageType.response_to_retract)
        {
            ChessBoardManager.Instance.Response_To_Retract(_message);
            //essBoardManager.Instance.Receive_Retract_Message();
        }

    }

    public void Send_Friend_Request(string targetName) {
        if (PlayerLocalData.Friend_Exist(targetName)) {
            PopMessageUI.Instance.Pop_Fail_Message("Friend already exists");
            return;
        }
        Send_Message_To_Target(ClassManager.MessageType.friend_request, PlayerLocalData.GetPlayerName(), targetName);
        PopMessageUI.Instance.Pop_Fail_Message("Friend request sent");
    }


    public void Host_Room()
    {
        string playerName = PlayerLocalData.GetPlayerName();
        string JsonN = JsonConvert.SerializeObject(playerName);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.host_room, JsonN);
        PlayerLocalData.Set_PlayerState(ClassManager.State.waitingRoom);
        OnEnterWaitingRoom();
    }

    public void Join_Room(int _roomID) {

        string playerName = PlayerLocalData.GetPlayerName();
        (int, string, int) message = (_roomID, playerName, 0);
        string JsonN = JsonConvert.SerializeObject(message);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, JsonN);
        PlayerLocalData.Set_PlayerState(ClassManager.State.waitingRoom);
        OnEnterWaitingRoom();
        WaitingRoomMenu.SetActive(true);
        WaitingRoomMenu.GetComponent<WaitingRoomMenuUI>().Init(PlayerLocalData.Get_Rooms_List()[_roomID]);
    }


    public void Pop_Message_Response(ClassManager.MessageType messageType, string _message, bool response) {
        if (messageType == ClassManager.MessageType.friend_request) {
            if (!response)
                return;

            string formattedDateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            string myName = PlayerLocalData.GetPlayerName();
            FirebaseManager.Instance.GetFriendListReference(_message).Child(myName).SetValueAsync(formattedDateTime);
            FirebaseManager.Instance.GetFriendListReference(myName).Child(_message).SetValueAsync(formattedDateTime);

            PlayerLocalData.Add_New_Friend(_message, 1);

            Send_Message_To_Target(ClassManager.MessageType.friend_request_accepted, myName, _message);
        }
    
    }

    public void Logout() {
        GameRecordMenu.SetActive(true);
        GameRecordUI.Instance.Clear_Game_Record();
        OnPlayerLogout();
        
        NetworkManager.Singleton.Shutdown();
        //SceneManager.LoadScene("multiplayer");
    }


    void OnApplicationQuit()
    {

        
        OnPlayerLogout();
    }



}
