using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class grid_script : MonoBehaviour
{
    [HideInInspector] private Vector2Int xy_index;

    public void on_click()
    {



        if (MatchData.Get_Game_Mode() == ClassManager.GameMode.multiplayer)
        {
            string jsonR = JsonConvert.SerializeObject(xy_index);
            foreach (string pname in PlayerLocalData.Get_Current_Room_Info().Get_All_Name()) {
                NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.place_a_stone, jsonR, pname);
            }

            //NetworkConnecter.Instance.Send_Message_To_Room_ServerRpc(ClassManager.MessageType.place_a_stone, PlayerLocalData.Get_RoomID(), jsonR);


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
