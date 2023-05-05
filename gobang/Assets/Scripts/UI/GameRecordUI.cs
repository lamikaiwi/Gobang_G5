using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameRecordUI : MonoBehaviour
{
    public static GameRecordUI Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] GameObject chessBoard;
    [SerializeField] TMP_Text p1Name;
    [SerializeField] TMP_Text p1Score;
    [SerializeField] GameObject p1Win_Icon;
    [SerializeField] GameObject p1Lose_Icon;
    [SerializeField] TMP_Text p2Name;
    [SerializeField] TMP_Text p21Score;
    [SerializeField] GameObject p2Win_Icon;
    [SerializeField] GameObject p2Lose_Icon;
    [SerializeField] TMP_Text sTime;
    [SerializeField] TMP_Text eTime;
    [SerializeField] Transform recordBtn;
    [SerializeField] Transform recordBtn_parent;
    List<Transform> Record_Instanse_List = new();
    [SerializeField] Button forwardBtn;
    [SerializeField] Button backwardBtn;
    [SerializeField] Button ExitBtn;


    List<Transform> transforms = new();
    private void OnEnable()
    {
        forwardBtn.interactable = false;
        backwardBtn.interactable = false;
        chessBoard.SetActive(true);

        while (true)
        {
            ClassManager.GameRecord temp = PlayerLocalData.Get_Temp_Game_Records();
            if (temp == null)
                break;
            Add_New_Game_Record(temp, true);
        }
    }

    private void OnDisable()
    {
        foreach (var item in transforms) {
            Destroy(item.gameObject);
        }
        transforms.Clear();
        chessBoard.SetActive(false);
    }

    private void Start()
    {
        forwardBtn?.onClick.AddListener(() => Move_Forward());
        backwardBtn?.onClick.AddListener(() => Move_Backward());
        ExitBtn?.onClick.AddListener(() => Exit_UI());
    }

    void Load_Game_Records() {
        var record = PlayerLocalData.Get_Game_Records();

        Add_New_Game_Record(record, false);
    }

    public void Add_New_Game_Record(ClassManager.GameRecord record, bool moveToFront) {
        if (record == null)
            return;

        var record_instanse = Instantiate(recordBtn);
        Record_Instanse_List.Add(record_instanse);
        record_instanse.SetParent(recordBtn_parent);
        if (moveToFront)
            record_instanse.SetSiblingIndex(0);
        record_instanse.localScale = new Vector3(1, 1, 1);
        record_instanse.GetComponent<GameRecordBtn>().Init(record);

        if (GameManager.Instance.IsAdmin())
            transforms.Add(record_instanse);
    }


    public void Set_Data(ClassManager.GameRecord gameRecord) {
        forwardBtn.interactable = true;
        backwardBtn.interactable = true;
        p1Name.text = gameRecord.player1Name;
        p1Score.text = gameRecord.player1Score.ToString();
        p2Name.text = gameRecord.player2Name;
        p21Score.text = gameRecord.player2Score.ToString();
        sTime.text = gameRecord.startTime;
        eTime.text = gameRecord.elapsedTime;

        if (string.Equals(gameRecord.winnerName, gameRecord.player1Name))
        {
            p1Win_Icon.SetActive(true);
            p1Lose_Icon.SetActive(false);
            p2Win_Icon.SetActive(false);
            p2Lose_Icon.SetActive(true);
        }
        else {
            p1Win_Icon.SetActive(false);
            p1Lose_Icon.SetActive(true);
            p2Win_Icon.SetActive(true);
            p2Lose_Icon.SetActive(false);
        }

    }

    public void Refresh() {
        for (int i = 0; i < 5; i++)
        {
            Load_Game_Records();
        }
    }

    void Move_Forward() {
        ChessBoardManager.Instance.Display_Forward();
    }
    void Move_Backward()
    {
        ChessBoardManager.Instance.Display_Backward();
    }

    void Exit_UI() {
        GameManager.Instance.OnEnterMainMenu();
    }

    public void Clear_Game_Record() {
        foreach (var item in Record_Instanse_List) {
            Destroy(item.gameObject);
        }
        Record_Instanse_List.Clear();
        gameObject.SetActive(false);
    }


}
