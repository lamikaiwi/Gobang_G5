using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Server_Player_Grid : MonoBehaviour
{

    [SerializeField] TMP_Text CID;
    [SerializeField] TMP_Text playerName;



    public void Init(string _CID, string _playerName)
    {
        CID.text = _CID;
        playerName.text = _playerName;
    }
}
