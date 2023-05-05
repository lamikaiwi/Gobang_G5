using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public static class MatchData
{
    static ClassManager.GameRecord game_record;
    static bool isWinner;




    public static void Init(ClassManager.GameMode _game_mode, string _start_time, ClassManager.PlayerLog player1, ClassManager.PlayerLog player2) {

        if (_game_mode == ClassManager.GameMode.singleplayer)
        {
            game_record = new(_game_mode);
        }
        else if (_game_mode == ClassManager.GameMode.multiplayer)
        {
            game_record = new ClassManager.GameRecord(_game_mode, _start_time, player1, player2);

        }
        else if (_game_mode == ClassManager.GameMode.randomplayer) {
            game_record = new ClassManager.GameRecord(_game_mode, _start_time, player1, player2);
        }
        else if (_game_mode == ClassManager.GameMode.spector)
        {
            game_record = new(_game_mode);
        }

    }

    public static ClassManager.GameRecord Get_Match_Data() { return game_record; }



    public static ClassManager.GameMode Get_Game_Mode() {
        return game_record.gameMode;
    }

    public static void Make_A_Move(Vector2Int vec) {
        game_record.AddStone(vec.x, vec.y);
    }

    public static Vector2Int Retract_A_Move( )
    {
        (int, int) temp = game_record.stoneList[game_record.stoneList.Count - 1];
        //Vector2Int temp = game_record.stoneList[game_record.stoneList.Count - 1];
        game_record.stoneList.RemoveAt(game_record.stoneList.Count - 1);
        return new Vector2Int(temp.Item1, temp.Item2);
    }

    public static void Match_Over_Set(int _current_stone_color, bool _isWinner)
    {
        isWinner = _isWinner;
        if (_current_stone_color == -1)
        {
            game_record.winnerName = game_record.player1Name;
        }
        else {
            game_record.winnerName = game_record.player2Name;
        }
        game_record.elapsedTime = InGameMenuUI.Instance.Get_Elapsed_Time();

        //PlayerLocalData.Add_Temp_Game_Records(game_record);

    }

    public static void Add_Temp_Game_Records() {
        PlayerLocalData.Add_Temp_Game_Records(game_record);
    }

    public static string Get_Game_Start_Time() {
        return game_record.startTime;
    }

    public static string Get_Game_Elapsed_Time() {
        return game_record.elapsedTime;
    }

    public static void Upload_Match_Data() {

        string game_string = JsonConvert.SerializeObject(game_record);
        string keys = FirebaseManager.Instance.GetGameReference().Push().Key;
        FirebaseManager.Instance.GetGameReference().Child(keys).SetRawJsonValueAsync(game_string);
        FirebaseManager.Instance.GetPlayerGameRecordReference(PlayerLocalData.GetPlayerName()).Child(keys).SetValueAsync("win");
        if (!string.Equals(OpponentData.GetPlayerName(), "randomPlayer"))
        {
            FirebaseManager.Instance.GetPlayerGameRecordReference(OpponentData.GetPlayerName()).Child(keys).SetValueAsync("lose");
        }
    }

    public static bool Get_Is_Winner() {
        return isWinner;
    }
}
