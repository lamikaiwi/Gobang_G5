using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
public class WaitingRoomMenuUI : MonoBehaviour
{
    public static WaitingRoomMenuUI Instance { set; get; }
    private void Awake() { Instance = this; }

    [SerializeField] Button joinP1;
    [SerializeField] TMP_Text P1Name;
    [SerializeField] Button joinP2;
    [SerializeField] TMP_Text P2Name;
    [SerializeField] Button joinSpector;
    [SerializeField] TMP_Text spectorList;

    [SerializeField] TMP_Text RoomID;

    [SerializeField] Button startBtn;
    [SerializeField] Button inviteBtn;
    [SerializeField] GameObject FD_List;
    [SerializeField] Button leaveRoom;
    

    // Start is called before the first frame update
    void Start()
    {
        joinP1?.onClick.AddListener(() => JoinP1());
        joinP2?.onClick.AddListener(() => JoinP2());
        joinSpector?.onClick.AddListener(() => JoinSpector());
        startBtn?.onClick.AddListener(() => StartGame());
        leaveRoom?.onClick.AddListener(() => LeaveRoom());
        inviteBtn?.onClick.AddListener(() => InviteFD());
    }

    ClassManager.Room roomInfo;


    public void Init(ClassManager.Room _room)
    {
        RoomID.text = $"Room ID: {_room.roomID}";
        roomInfo = _room;
        PlayerLocalData.Set_RoomID(roomInfo.roomID);
        Set_UI();
    }

    void InviteFD()
    {
        FD_List.SetActive(true);
    }


    /*
     
             if (roomsList.ContainsKey(_message.Item1))
        {
            roomsList[_message.Item1].Update_Room(_message.Item2, _message.Item3);
            if (roomID == _message.Item1)
            {
                WaitingRoomMenuUI.Instance.Update_Room(_message);
            }

        }
        else {

            ClassManager.Room room = new(_message.Item1, _message.Item2);
            roomsList.Add(_message.Item1, room);
        }
     */

    public void Update_Room((int, string, int) info) {
        roomInfo.Update_Room(info.Item2, info.Item3);

        Set_UI();
    }

    void Set_UI() {

        bool isPlayer = false;

        startBtn.interactable = false;
        joinP1.gameObject.SetActive(false);
        joinP2.gameObject.SetActive(false);
        joinSpector.interactable = false;

        if (string.Equals( roomInfo.player1Name,""))
        {
            P1Name.text = "";
            joinP1.gameObject.SetActive(true);
        }
        else {
            P1Name.text = roomInfo.player1Name;

            if (string.Equals(roomInfo.player1Name, PlayerLocalData.GetPlayerName())) {
                isPlayer = true;
                if (!string.Equals(roomInfo.player2Name, ""))
                    startBtn.interactable = true;
            }
        }

        if (string.Equals(roomInfo.player2Name, ""))
        {
            P2Name.text = "";
            joinP2.gameObject.SetActive(true);
        }
        else
        {
            P2Name.text = roomInfo.player2Name;
            if (string.Equals(roomInfo.player2Name, PlayerLocalData.GetPlayerName()))
            {
                isPlayer = true;
            }
        }
        string specList = "";
        foreach (string spec in roomInfo.spectorName)
        {
            specList += $"{spec}\n";
        }
        spectorList.text = specList;

        if (isPlayer)
            joinSpector.interactable = true;

    }


    void JoinP1() {
        string myName = PlayerLocalData.GetPlayerName();
        int roomID = roomInfo.roomID;
        (int, string, int) message = (roomID, myName, 1);
        
        //ClassManager.Room newRoom = roomInfo.Remove_Name(myName);
        //newRoom.player1Name = myName;
        //newRoom.playerNum++;

        string messageJson = JsonConvert.SerializeObject(message);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, messageJson);
    }

    void JoinP2()
    {
        string myName = PlayerLocalData.GetPlayerName();
        int roomID = roomInfo.roomID;
        (int, string, int) message = (roomID, myName, 2);

        //ClassManager.Room newRoom = roomInfo.Remove_Name(myName);
        //newRoom.player2Name = myName;
        //newRoom.playerNum++;

        string messageJson = JsonConvert.SerializeObject(message);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, messageJson);
    }
    void JoinSpector()
    {
        string myName = PlayerLocalData.GetPlayerName();
        int roomID = roomInfo.roomID;
        (int, string, int) message = (roomID, myName, 3);

        //ClassManager.Room newRoom = roomInfo.Remove_Name(myName);
        //newRoom.spectorName.Add(myName);
        //newRoom.playerNum++;

        string messageJson = JsonConvert.SerializeObject(message);
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, messageJson);
    }

    public void LeaveRoom() {
        string myName = PlayerLocalData.GetPlayerName();
        int roomID = roomInfo.roomID;
        (int, string, int) message = (roomID, myName, 4);

        //ClassManager.Room newRoom = roomInfo.Remove_Name(myName);

        string messageJson = JsonConvert.SerializeObject(message);

        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_update, messageJson);
        PlayerLocalData.Set_RoomID(-1);
        PlayerLocalData.Set_PlayerState(ClassManager.State.onMenu);
        GameManager.Instance.OnEnterMainMenu();
        
    }
    void StartGame() {
        string message = JsonConvert.SerializeObject(PlayerLocalData.Get_RoomID());
        NetworkConnecter.Instance.Send_Message_To_Server_ServerRpc(ClassManager.MessageType.room_change_state, message);


    }


}
