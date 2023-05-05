using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class ClassManager
{

    [Serializable]
    public class GameRecord
    {
        public string startTime;
        public string elapsedTime;
        public string player1Name;
        public string player2Name;
        public int player1Score;
        public int player2Score;
        public string winnerName;
        public GameMode gameMode;
        public List<(int,int)> stoneList = new();

        public GameRecord()
        {
            this.startTime = "startTime";
            this.elapsedTime = "elapsedTime";
            this.player1Name = "player1Name";
            this.player2Name = "player2Name";
            this.player1Score = 0;
            this.player2Score = 0;
            this.winnerName = "winnerName";
            gameMode = GameMode.singleplayer;
        }



        public GameRecord(GameMode mode)
        {
            this.startTime = "startTime";
            this.elapsedTime = "elapsedTime";
            this.player1Name = "player1Name";
            this.player2Name = "player2Name";
            this.player1Score = 0;
            this.player2Score = 0;
            this.winnerName = "winnerName";
            gameMode = mode;
        }

        public GameRecord(GameMode mode, string _startTime, PlayerLog player1, PlayerLog player2)
        {
            this.gameMode = mode;
            this.startTime = _startTime;
            this.player1Name = player1.playerName;
            this.player2Name = player2.playerName;
            this.player1Score = player1.score;
            this.player2Score = player2.score;
            this.stoneList.Clear();
        }

        public void AddStone(int x, int y)
        {
            this.stoneList.Add((x, y));
        }
    }


    [Serializable]
    public class PlayerLog
    {
        public string playerName;
        public string userType;
        public int coin;
        public int score;


        public PlayerLog(string _playerName, int _coin, int _score)
        {

            this.playerName = _playerName;
            this.coin = _coin;
            this.score = _score;
            this.userType = "0";
        }

        public PlayerLog(string _playerName)
        {
            this.playerName = _playerName;
            this.coin = 0;
            this.score = 0;
            this.userType = "0";
        }

        public PlayerLog()
        {
            this.playerName = "";
            this.coin = 0;
            this.score = 0;
            this.userType = "";
        }

        public void SetAsAdmin()
        {
            this.userType = "1";
        }
    }

    [Serializable]
    public class PlayerFriendList {


    }

    [Serializable]
    public class Room
    {
        public int roomID;
        public int playerNum;
        public string state;
        public string player1Name;
        public string player2Name;
        public List<string> spectorName = new();
        public Room()
        {

        }

        public Room(int _roomID, string p1Name)
        {
            this.roomID = _roomID;
            this.playerNum = 1;
            this.state = "waiting";
            this.player1Name = p1Name;
            this.player2Name = "";

        }

        public void Update_Room(string _name, int _pos) {
            if (_pos == 0)
            {
                this.spectorName.Add(_name);
                this.playerNum++;
                return;
            }

            Remove_Name(_name);
            if (_pos == 1)
            {
                this.player1Name = _name;
            }
            else if (_pos == 2)
            {
                this.player2Name = _name;
            }
            else if (_pos == 3)
            {
                this.spectorName.Add(_name);
            }
            else if (_pos == 4)
            {
                this.playerNum--;
            }
        }


        private void Remove_Name(string _name) {
            if (string.Equals(this.player1Name, _name))
            {
                this.player1Name = "";
            }
            else if (string.Equals(this.player2Name, _name))
            {
                this.player2Name = "";
            }
            else {
                this.spectorName.Remove(_name);
            }
        }

        public string[] Get_All_Name() {

            List<string> newList = new List<string>(spectorName);
            newList.Add(player1Name);
            newList.Add(player2Name);
            return newList.ToArray();
        }

        public string[] Get_Players()
        {
            string[] names = new string[2];
            names[0] = player1Name;
            names[1] = player2Name;
            return names;
        }

        public string GetOpponent(string player)
        {
            if (string.Equals(player, this.player1Name))
                return player2Name;
            return player1Name;

        }

        public int Get_PlayerNum() {
            return this.playerNum;
        }

        public void Change_State() {
            this.state = "playing";
        }
    }

    public class PlayerInfo {
        public string playerName;
        public string playerPassword;

        public PlayerInfo(string _name, string _password) {
            this.playerName = _name;
            this.playerPassword = _password;
        }
    }

    public class RoomTitle
    {
        public int roomID;
        public int status;
        public string player1Name;
        public string player2Name;

        public RoomTitle() { }
    }

    public enum GameMode
    {
        singleplayer,
        randomplayer,
        multiplayer,
        spector
    }

    public enum State { 
        onMenu,
        waitingRoom,
        playing
    
    
    }

    public enum MessageType
    {
        place_a_stone,
        ask_for_retract,
        response_to_retract,
        friend_request,
        friend_request_accepted,
        game_invite,
        dict_add,
        friend_online,
        friend_offline,
        friend_onMenu,
        friend_playing,
        friend_deleted,
        being_deleted,
        host_room,
        room_update,
        room_change_state,
        room_init,
        room_finish,
        room_chat,
        sync_roomList
    }


    [Serializable]
    public class Message
    {
        public MessageType messageType;
        public string message;

    }



}
