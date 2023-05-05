using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Room_Grids : MonoBehaviour
{


    [SerializeField] TMP_Text roomID;
    [SerializeField] TMP_Text player1Name;
    [SerializeField] TMP_Text player2Name;
    [SerializeField] TMP_Text roomState;
    [SerializeField] TMP_Text specNum;
    [SerializeField] Button clickBtn;

    int rID;

    private void Start()
    {
        clickBtn?.onClick.AddListener(() => OnClick());
    }


    public void Init(KeyValuePair<int, ClassManager.Room> keyValue) {
        rID = keyValue.Key;
        roomID.text = keyValue.Key.ToString();
        player1Name.text = keyValue.Value.player1Name;
        player2Name.text = keyValue.Value.player2Name;
        roomState.text = keyValue.Value.state;
        if (string.Equals(roomState.text, "waiting"))
            clickBtn.interactable = true;
        else
            clickBtn.interactable = false;
        specNum.text = keyValue.Value.spectorName.Count.ToString();
    }

    public void OnClick()
    {
        GameManager.Instance.Join_Room(rID);



        RoomListMenuUI.Instance.gameObject.SetActive(false);
    }
}
