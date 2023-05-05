using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
public class AdminMenuUI : MonoBehaviour
{
    /*
     *  This script is the manager of AdminMenu.
     *  There are 5 main function
     *  1. Refresh
     *  2. Show_Profile
     *  3. Show_Game_Record
     *  4. Show_Friend_List
     *  5. Delete_User
     *  
     *  Each of them will call an IEnumerator.
     *  The IEnumerator interface is used to define the behavior of coroutine methods, 
     *  code inside the coroutine runs on a separate thread from the main thread.
     * 
     *  Refresh: Run the Get_All_Users_IEnumerator() IEnumerator, it will access the database and get all the user names, and show it on user list.
     *  Show_Profile: Run the Show_ProfileE() IEnumerator, it will access the database and show the selected user profile.
     *  Show_Game_Record: Run the Get_Game_Records_IEnumerator() IEnumerator, it will access the database and show the selected user game record.
     *  Show_Friend_List: Run the Get_Friend_Dict_IEnumerator() IEnumerator, it will access the database and show the selected user friend list.
     *  Delete_User: Run the Delete_User_IEnumerator() IEnumerator, it will delect the selected user immediately and update the database.
     *  
     */
    public static AdminMenuUI Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject user_grid;
    [SerializeField] Transform user_grid_parent;

    [SerializeField] Button profileBtn;
    [SerializeField] Button gameRecordBtn;
    [SerializeField] Button friendListBtn;
    [SerializeField] Button delUserBtn;
    [SerializeField] Button refreshBtn;


    [SerializeField] GameObject profileMenu;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerScore;
    [SerializeField] TMP_Text playerCoin;

    [SerializeField] GameObject gameRecordMenu;
    [SerializeField] GameObject friendListMenu;


    string current_name = "";
    List<GameObject> grid = new();
    private void Start()
    {
        profileBtn?.onClick.AddListener(() => Show_Profile());
        gameRecordBtn?.onClick.AddListener(() => Show_Game_Record());
        friendListBtn?.onClick.AddListener(() => Show_Friend_List());
        delUserBtn?.onClick.AddListener(() => Delete_User());
        refreshBtn?.onClick.AddListener(() => Refresh());
    }

    IEnumerator Show_ProfileE() {
        string jsonRecord = "";
        var task3 = FirebaseManager.Instance.GetUserLogReference(current_name).GetValueAsync().ContinueWithOnMainThread(task3 =>
        {
            if (task3.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task3.Result;
                jsonRecord = snapshot.GetRawJsonValue();
            }
        });

        yield return new WaitUntil(() => task3.IsCompleted);
        ClassManager.PlayerLog temp = JsonConvert.DeserializeObject<ClassManager.PlayerLog>(jsonRecord);
        profileMenu.SetActive(true);
        playerName.text = temp.playerName;
        playerScore.text = temp.score.ToString();
        playerCoin.text = temp.coin.ToString();

    }

    void Show_Profile() {
        StartCoroutine(Show_ProfileE());
    }


    IEnumerator Get_Game_Records_IEnumerator()
    {
        List<string> record_keys = new();
        var _task = FirebaseManager.Instance.GetPlayerGameRecordReference(current_name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var keys in snapshot.Children)
                {
                    record_keys.Add(keys.Key.ToString());
                }
            }
        });
        yield return new WaitUntil(() => _task.IsCompleted);

        List<ClassManager.GameRecord> gameRecords = new();
        for (int i = record_keys.Count - 1; i >= 0; i--)
        {
            var _task2 = FirebaseManager.Instance.GetGameReference().Child(record_keys[i]).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                var jsonRecord = task.Result.GetRawJsonValue();
                gameRecords.Add(JsonConvert.DeserializeObject<ClassManager.GameRecord>(jsonRecord));
            });
            yield return new WaitUntil(() => _task2.IsCompleted);
        }

        PlayerLocalData.Set_Temp_Game_Records(gameRecords);
        gameRecordMenu.SetActive(true);
        PopMessageUI.Instance.Pop_LoadingPanel(false);
    }


    void Show_Game_Record() {
        PopMessageUI.Instance.Pop_LoadingPanel(true);
        StartCoroutine(Get_Game_Records_IEnumerator());
    }


    IEnumerator Get_Friend_Dict_IEnumerator()
    {
        Dictionary<string, int> friend_dict = new();
        var _task = FirebaseManager.Instance.GetFriendListReference(current_name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var fd in snapshot.Children)
                {
                    friend_dict.Add(fd.Key.ToString(), 0);
                }
            }
        });
        yield return new WaitUntil(() => _task.IsCompleted);
        PlayerLocalData.Set_Friend_Dict(friend_dict);
        friendListMenu.SetActive(true);

    }

    void Show_Friend_List() {

        StartCoroutine(Get_Friend_Dict_IEnumerator());
    }



    IEnumerator Delete_User_IEnumerator()
    {

        Dictionary<string, int> friend_dict = new();
        var _task = FirebaseManager.Instance.GetFriendListReference(current_name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var fd in snapshot.Children)
                {
                    friend_dict.Add(fd.Key.ToString(), 0);
                }
            }
        });
        yield return new WaitUntil(() => _task.IsCompleted);

        foreach (KeyValuePair<string, int> fdName in friend_dict)
        {
            var _task2 = FirebaseManager.Instance.GetFriendListReference(fdName.Key).Child(current_name).SetValueAsync(null);

            NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.friend_deleted, current_name, fdName.Key);
            yield return new WaitUntil(() => _task2.IsCompleted);
        }
        
        var _task3 = FirebaseManager.Instance.GetUserReference().Child(current_name).SetValueAsync(null);
        yield return new WaitUntil(() => _task3.IsCompleted);

        NetworkConnecter.Instance.Send_Message_To_Target_Client_ServerRpc(ClassManager.MessageType.being_deleted, null, current_name);
        Refresh();
    }

    void Delete_User() {

        StartCoroutine(Delete_User_IEnumerator());
    }


    private void OnEnable()
    {
        Refresh();
    }

    void Refresh() {
        foreach (var item in grid) {
            Destroy(item);
        }
        grid.Clear();
        PopMessageUI.Instance.Pop_LoadingPanel(true);
        StartCoroutine(Get_All_Users_IEnumerator());
    }


    IEnumerator Get_All_Users_IEnumerator()
    {
        List<string> userNames = new();
        var _task = FirebaseManager.Instance.GetUserReference().GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var keys in snapshot.Children)
                {
                    userNames.Add(keys.Key.ToString());
                }
            }
        });
        yield return new WaitUntil(() => _task.IsCompleted);


        foreach (var _name in userNames)
        {

            var record_instanse = Instantiate(user_grid);
            record_instanse.transform.SetParent(user_grid_parent);
            record_instanse.transform.localScale = new Vector3(1, 1, 1);
            record_instanse.GetComponent<Admin_Grid>().Init(_name);
            grid.Add(record_instanse);


        }
        PopMessageUI.Instance.Pop_LoadingPanel(false);
    }

    public void OnGridClicked(string _name) {
        current_name = _name;
    }

}
