using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerListMenuUI : MonoBehaviour
{
    public static PlayerListMenuUI Instanse { set; get; }
    [SerializeField] Transform list_parent;
    [SerializeField] Transform playerData;
    [SerializeField] TMP_Text ipText;

    Dictionary<ulong, Transform> playerList = new();

    private void Awake()
    {
        Instanse = this;
    }

    private void OnEnable()
    {
        ipText.text = $"Server: {PlayerLocalData.GetTargetIP()}";
    }

    public void Add_Player(ulong CID, string name) {
        var player_instanse = Instantiate(playerData);
        playerList.Add(CID, player_instanse);
        player_instanse.SetParent(list_parent);
        player_instanse.SetSiblingIndex(0);
        player_instanse.localScale = new Vector3(1, 1, 1);
     
        player_instanse.GetComponent<Server_Player_Grid>().Init(CID.ToString(), name);

    }

    public void Remove_Player(ulong CID) {
        Destroy(playerList[CID].gameObject);
        playerList.Remove(CID);
    }

}
