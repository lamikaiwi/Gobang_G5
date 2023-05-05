using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class grid_script : MonoBehaviour
{
    /*
     * The gameObject with this script is the 19*19 grids in chessboard, when it is clicked, if in multiplayer mode, send message to server 
     * 
     */
    [HideInInspector] private Vector2Int xy_index;

    public void on_click()
    {
        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.multiplayer)
        {
            string jsonR = JsonConvert.SerializeObject(xy_index);
            foreach (string pname in PlayerLocalData.Get_Current_Room_Info().Get_All_Name()) {
                NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.place_a_stone, jsonR, pname);
            }
        }
        else
        {
            ChessBoardManager.Instance.Place_A_Stone(xy_index.x, xy_index.y);
        }
        gameObject.SetActive(false);
    }

    public void Set_Index(int x, int y) {
        xy_index = new Vector2Int(x, y);
    }


}
