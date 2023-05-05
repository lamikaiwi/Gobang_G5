using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Netcode.Transports.UTP;
using Newtonsoft.Json;

public class NetworkConnecter : NetworkBehaviour
{
    /*
     * This is the main part of server, in short it will recieve message from client, and send that message to target client
     * For every function with header [ServerRpc(RequireOwnership = false)] is called by client and run on server.
     * For every function with header [ClientRpc] is called by server and run on client.
     * For every function with no header is called by server and run on server. We can do some data validation here if we want.
     */
    public static NetworkConnecter Instance { get; set; }


    Dictionary<int, ClassManager.Room> rooms2 = new();

    Dictionary<ulong, string> Dict_CID_Name = new();
    Dictionary<string, ulong> Dict_Name_CID = new();
    Dictionary<string, int> Dict_Name_rID = new();
    Dictionary<string, ClassManager.PlayerLog> Dict_Name_Log = new();


    public override void OnNetworkSpawn()
    {
        Instance = this;

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;


    }
    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;

    }




    void HandleClientConnected(ulong clientID)
    {
        if (IsServer) {
            Send_Message_To_Target_Client(ClassManager.MessageType.sync_roomList, JsonConvert.SerializeObject(rooms2), clientID);
        }

        if (IsClient) {

            string message = JsonConvert.SerializeObject(PlayerLocalData.GetPlayerLog());
            Send_Message_To_Server_ServerRpc(ClassManager.MessageType.dict_add, message);

            string message2 = JsonConvert.SerializeObject(PlayerLocalData.GetPlayerName());
            Send_Message_To_All_ServerRpc(ClassManager.MessageType.friend_online, message2);
        }


        
    }
    void HandleClientDisconnected(ulong clientID)
    {

        if (IsServer)
        {
            string offline_name = Dict_CID_Name[clientID];

            Send_Message_To_All_ClientRPC(ClassManager.MessageType.friend_offline, JsonConvert.SerializeObject(offline_name));

            
            if (Dict_Name_rID.ContainsKey(offline_name))
            {
                (int, string, int) leaveRoomMessage = (Dict_Name_rID[offline_name], offline_name, 4);
                string message = JsonConvert.SerializeObject(leaveRoomMessage);

                rooms2[leaveRoomMessage.Item1].Update_Room(leaveRoomMessage.Item2, leaveRoomMessage.Item3);
                if (rooms2[leaveRoomMessage.Item1].Get_PlayerNum() == 0)
                {
                    rooms2.Remove(leaveRoomMessage.Item1);
                    Dict_Name_rID.Remove(leaveRoomMessage.Item2);
                }

                Send_Message_To_All_ClientRPC(ClassManager.MessageType.room_update, message);
            }
            

            Dict_Name_Log.Remove(offline_name);
            Dict_Name_CID.Remove(offline_name);
            Dict_CID_Name.Remove(clientID);
            PlayerListMenuUI.Instanse.Remove_Player(clientID);
        }



    }





    [ServerRpc(RequireOwnership = false)]
    public void Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType messageType, string message, string _clientNames)
    {
        if (!IsServer) return;

        if (!Dict_Name_CID.ContainsKey(_clientNames))
            return;
        ulong targetCID = Dict_Name_CID[_clientNames];
        Send_Message_To_Target_Client(messageType, message, targetCID );
    }

    void Send_Message_To_Target_Client(ClassManager.MessageType messageType, string message, ulong clientIDs) {
        if (!IsServer) return;
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientIDs }
            }
        };
        Server_Client_Message_ClientRpc(messageType, message, clientRpcParams);

    }
    void Send_Message_To_Target_Client(ClassManager.MessageType messageType, string message, string clientName)
    {
        if (!IsServer) return;
        ulong clientIDs = Dict_Name_CID[clientName];
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientIDs }
            }
        };
        Server_Client_Message_ClientRpc(messageType, message, clientRpcParams);

    }


    [ServerRpc(RequireOwnership = false)]
    public void Send_Message_To_Server_ServerRpc(ClassManager.MessageType messageType, string _message, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        ulong senderCID = serverRpcParams.Receive.SenderClientId;

        if (messageType == ClassManager.MessageType.dict_add)
        {
            ClassManager.PlayerLog tempLog = JsonConvert.DeserializeObject<ClassManager.PlayerLog>(_message);
            string name = tempLog.playerName;
            Dict_CID_Name.Add(senderCID, name);
            Dict_Name_CID.Add(name, senderCID);
            Dict_Name_Log.Add(name, tempLog);
            PlayerListMenuUI.Instanse.Add_Player(senderCID, name);
        }
        else if (messageType == ClassManager.MessageType.host_room)
        {
            int leastAvailableKey = 0;
            string p1Name = JsonConvert.DeserializeObject<string>(_message);
            while (rooms2.ContainsKey(leastAvailableKey))
            {
                leastAvailableKey++;
            }
            ClassManager.Room room2 = new(leastAvailableKey, p1Name);
            Dict_Name_rID.Add(p1Name, leastAvailableKey);
            
            rooms2.Add(leastAvailableKey, room2);
            string newMessage = JsonConvert.SerializeObject(room2);
            string message2 = JsonConvert.SerializeObject((leastAvailableKey, p1Name, 0));
            Send_Message_To_All_ClientRPC(ClassManager.MessageType.room_update, message2);
            Send_Message_To_Target_Client(ClassManager.MessageType.host_room, newMessage, senderCID);
        }
        else if (messageType == ClassManager.MessageType.room_update)
        {
            (int, string, int) message = JsonConvert.DeserializeObject<(int, string, int)>(_message);
            rooms2[message.Item1].Update_Room(message.Item2, message.Item3);
            if (message.Item3 == 4) {
                Dict_Name_rID.Remove(message.Item2);
            }

            if (rooms2[message.Item1].Get_PlayerNum() == 0)
            {
                rooms2.Remove(message.Item1);
            }

            Send_Message_To_All_ClientRPC(messageType, _message);
        }
        else if (messageType == ClassManager.MessageType.room_change_state) {
            int roomID = JsonConvert.DeserializeObject<int>(_message);

            ClassManager.Room theRoom = rooms2[roomID];
            theRoom.Change_State();
            Send_Message_To_All_ClientRPC(messageType, _message);
            //foreach ()
            string formattedDateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            (string, ClassManager.PlayerLog, ClassManager.PlayerLog, ClassManager.Room) temp = (formattedDateTime, Dict_Name_Log[theRoom.player1Name], Dict_Name_Log[theRoom.player2Name], theRoom);
            string message = JsonConvert.SerializeObject(temp);
            foreach (string targetName in theRoom.Get_All_Name()) {
                Send_Message_To_Target_Client(ClassManager.MessageType.room_init, message, targetName);
            
            }

        }

    }

    [ClientRpc]
    void Server_Client_Message_ClientRpc(ClassManager.MessageType messageType, string message, ClientRpcParams clientRpcParams) {
        GameManager.Instance.Receive_Message(messageType, message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Send_Message_To_All_ServerRpc(ClassManager.MessageType messageType, string message)
    {
        if (!IsServer) return;
        Send_Message_To_All_ClientRPC(messageType, message);
    }

    [ClientRpc]
    private void Send_Message_To_All_ClientRPC(ClassManager.MessageType messageType, string message)
    {
        GameManager.Instance.Receive_Message(messageType, message);
    }




}
