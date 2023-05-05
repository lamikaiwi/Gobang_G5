using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using UnityEngine.Events;
using Firebase.Extensions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    /*
     *  This script manage to access data from database, which is firebase.
     *  There are 9 main function.
     *      Get_Game_Records
     *      Get_Friend_Dict
     *      Get_Item_List
     *      GetServerIP - it is abandoned in this version. It used to get a available server IP from the database (there maybe more than 1 server), and connect to it.
     *      Before_Registering - it will check whether the account already exists, if not, will call Register()
     *      Register
     *      Before_Login
     *      Updata_Playerlog
     *      Updata_Friend_List
     *    
     *    There are also some DatabaseReference type function.
     *    GetUserReference
     *    GetUserLogReference
     *    GetFriendListReference
     *    GetItemListReference
     *    GetPlayerGameRecordReference
     *    ...
     *    GetServerReference
     *    
     *    These function will return the reference of database.
     */
    public static FirebaseManager Instance { set; get; }

    private string playerName_local;


    private void Awake()
    {
        Instance = this;


    }

   
    IEnumerator Get_Game_Records_IEnumerator(string playerName)
    {
        List<string> record_keys = new();
        var _task = GetPlayerGameRecordReference(playerName).GetValueAsync().ContinueWithOnMainThread(task =>
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
            var _task2 = GetGameReference().Child(record_keys[i]).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                var jsonRecord = task.Result.GetRawJsonValue();
                gameRecords.Add(JsonConvert.DeserializeObject<ClassManager.GameRecord>(jsonRecord));
            });
            yield return new WaitUntil(() => _task2.IsCompleted);
        }
        PlayerLocalData.Set_Game_Records(gameRecords);
    }


    public void Get_Game_Records(string playerName) {
        StartCoroutine(Get_Game_Records_IEnumerator(playerName));
    
    }



    IEnumerator Get_Friend_Dict_IEnumerator(string playerName)
    {
        Dictionary<string, int> friend_dict = new();
        var _task = GetFriendListReference(playerName).GetValueAsync().ContinueWithOnMainThread(task =>
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
    }

    public void Get_Friend_Dict(string playerName)
    {
        StartCoroutine(Get_Friend_Dict_IEnumerator(playerName));
    }


    IEnumerator Get_Item_List_IEnumerator(string playerName)
    {
        List<int> item_list = new();
        item_list.Add(0);
        item_list.Add(1);

        for (int i = 2; i < 6; i++)
        {
            var _task = GetItemListReference(playerName).Child(i.ToString()).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null) {
                    item_list.Add(i);
                }

                    
                
            });
            yield return new WaitUntil(() => _task.IsCompleted);

        }

        PlayerLocalData.Set_Background_Dict(item_list);
    }

    public void Get_Item_List(string playerName)
    {
        StartCoroutine(Get_Item_List_IEnumerator(playerName));
    }



    public void GetServerIP() {
        GetServerReference().GetValueAsync().ContinueWithOnMainThread(task2 =>
        {
            if (task2.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task2.Result;
                //PlayerLocalData.SetTargetIP(snapshot.ToString());
            }
        });

    }

    public void Register(string userName, string password) {



        playerName_local = userName;
        
        ClassManager.PlayerLog pgd = new(userName);
        PlayerLocalData.SetPlayerLog(pgd);

        var jsonRecord = JsonConvert.SerializeObject(pgd);

        GetUserLogReference(playerName_local).SetRawJsonValueAsync(jsonRecord);

        GetUserReference().Child(userName).Child("password").SetValueAsync(password);
        GameManager.Instance.OnPlayerLogin();



        /*
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                ClassManager.PlayerLog pgd = new(playerName, email);
                pgd.name = userName;
                var jsonRecord = JsonConvert.SerializeObject(pgd);
                GetUserLogReference().SetRawJsonValueAsync(jsonRecord);
                PlayerLocalData.Instance.SetPlayerLog(pgd);
                GameManager.Instance.OnPlayerLogin();
            }
        });
        */
    }

    IEnumerator Login(string username, string password)
    {
        string jsonRecord = "";
        DatabaseReference reference = GetUserReference();
        var task = reference.Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                jsonRecord = snapshot.GetRawJsonValue();

            }

        });

        yield return new WaitUntil(() => task.IsCompleted);

        if (jsonRecord == null)
        {
            LoginMenuUI.Instance.Pop_Fail_Message("Account does not exist");
            yield break;
        }
        playerName_local = username;

        string jsonRecord2 = "";
        var task2 = GetUserReference().Child(playerName_local).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                jsonRecord2 = snapshot.Value.ToString();

            }
            if (task.IsCanceled)
            {
                LoginMenuUI.Instance.Pop_Fail_Message("Firebase no response");
            }
            if (task.IsFaulted)
            {
                LoginMenuUI.Instance.Pop_Fail_Message("Firebase no response");
            }

        });

        yield return new WaitUntil(() => task2.IsCompleted);

        if (!string.Equals(jsonRecord2, password))
        {
            LoginMenuUI.Instance.Pop_Fail_Message("Password is wrong");
            yield break;
        }

        string jsonRecord3 = "";
        var task3 = GetUserLogReference(playerName_local).GetValueAsync().ContinueWithOnMainThread(task3 =>
        {
            if (task3.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task3.Result;
                jsonRecord3 = snapshot.GetRawJsonValue();

            }
            if (task3.IsCanceled)
            {
                LoginMenuUI.Instance.Pop_Fail_Message("Firebase no response");
            }
            if (task3.IsFaulted)
            {
                LoginMenuUI.Instance.Pop_Fail_Message("Firebase no response");
            }
        });

        yield return new WaitUntil(() => task3.IsCompleted);

        PlayerLocalData.SetPlayerLog(JsonUtility.FromJson<ClassManager.PlayerLog>(jsonRecord3));
        GameManager.Instance.OnPlayerLogin();


    }

    
    public void Updata_Playerlog() {
        var jsonRecord = JsonConvert.SerializeObject(PlayerLocalData.GetPlayerLog());
        GetUserLogReference(playerName_local).SetRawJsonValueAsync(jsonRecord);
    }

    public void Updata_Friend_List() { 
    
    
    }


    public void Before_Registering(string username, string password) {
        DatabaseReference reference = GetUserReference();
        reference.Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                var jsonRecord = snapshot.GetRawJsonValue();
                if (jsonRecord != null)
                {
                    SignupMenuUI.Instance.Pop_Fail_Message("Account already exist");
                }
                else
                {
                    Register(username, password);
                }



            }

        });
    }

    public void Before_Login(string username, string password) {
        StartCoroutine(Login(username, password));
    }

    public DatabaseReference GetUserReference()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("users");
    }

    public DatabaseReference GetUserLogReference(string _playerName) {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("users").Child(_playerName).Child("user_log");
    }
    public DatabaseReference GetFriendListReference(string _playerName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("users").Child(_playerName).Child("friend_list");
    }
    public DatabaseReference GetItemListReference(string _playerName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("users").Child(_playerName).Child("item_list");
    }
    public DatabaseReference GetPlayerGameRecordReference(string _playerName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("users").Child(_playerName).Child("player_game_record");
    }
    public DatabaseReference GetGameReference() {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("game_records");
    }
    public DatabaseReference GetCurrentGameReference(string gameID)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("current_game_records").Child(gameID);
    }
    public DatabaseReference GetOnlineUsersReference()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("online_users");
    }
    public DatabaseReference GetTestReference()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("test").Child("test3");
    }
    public DatabaseReference GetServerReference()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child("server");
    }




}







