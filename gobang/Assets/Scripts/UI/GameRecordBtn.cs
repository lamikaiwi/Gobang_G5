using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameRecordBtn : MonoBehaviour
{
    // Start is called before the first frame update

    ClassManager.GameRecord gameRecord;
    [SerializeField] TMP_Text player1Name;
    [SerializeField] TMP_Text player2Name;
    [SerializeField] TMP_Text startTime;
    [SerializeField] TMP_Text elapsedTime;

    public void Init(ClassManager.GameRecord _gameRecord) {
        gameRecord = _gameRecord;
        player1Name.text = gameRecord.player1Name;
        player2Name.text = gameRecord.player2Name;
        startTime.text = gameRecord.startTime;
        elapsedTime.text = gameRecord.elapsedTime;
    }

    public void OnClick() {
        GameRecordUI.Instance.Set_Data(gameRecord);
        ChessBoardManager.Instance.Display_Stone_List(gameRecord.stoneList);
    }


}
