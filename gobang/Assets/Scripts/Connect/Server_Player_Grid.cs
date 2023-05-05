using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Server_Player_Grid : MonoBehaviour
{
    /*
     * This script is to init the text, which is a user name,
     * in the button that located in online user list, which is only accessable by server
     */

    [SerializeField] TMP_Text CID;
    [SerializeField] TMP_Text playerName;



    public void Init(string _CID, string _playerName)
    {
        CID.text = _CID;
        playerName.text = _playerName;
    }
}
