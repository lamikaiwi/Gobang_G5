using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentData
{
    //public static OpponentData Instance { get; set; }


    private static ClassManager.PlayerLog log;

    private static ulong clientID;




    public static void SetPlayerLog(ClassManager.PlayerLog _log)
    {
        log = _log;
    }

    public static ClassManager.PlayerLog GetPlayerLog()
    {
        return log;
    }

    public static ulong getClientID()
    {
        return clientID;
    }

    public static string GetPlayerName()
    {
        return log.playerName;
    }

    public static int GetPlayerScore()
    {
        return log.score;
    }






}
