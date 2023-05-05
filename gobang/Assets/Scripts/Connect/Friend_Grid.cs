using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
public class Friend_Grid : MonoBehaviour
{
    [SerializeField] Image state_icon;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Button onClick;

    private void Start()
    {
        onClick?.onClick.AddListener(() => Invite_Friend());
    }

    public void Init(int state, string _playerName)
    {


        playerName.text = _playerName;
        Change_State(state);
    }

    public void Change_State(int state) {
        if (state == 0)
        {
            state_icon.color = new Color32(150, 150, 150, 255);
        }
        else if (state == 1)
        {
            state_icon.color = new Color32(80, 200, 80, 255);
        }
        else if (state == 2)
        {
            state_icon.color = new Color32(255, 100, 0, 255);
        }

    }

    void Invite_Friend() {
        if (PlayerLocalData.Get_PlayerState() == ClassManager.State.waitingRoom) {
            (string, int) temp = (PlayerLocalData.GetPlayerName(), PlayerLocalData.Get_RoomID());
            string message = JsonConvert.SerializeObject(temp);
            NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.game_invite, message, playerName.text);
        }
    }

}
