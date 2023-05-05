using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.Net;
using TMPro;
using Firebase.Extensions;
using Newtonsoft.Json;
[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public RectTransform qq;

    public bool key1 = false;
    public bool key2 = false;
    public string _name="";
    List<string> record_keys = new();
    List<ClassManager.GameRecord> game_records = new();


    IEnumerator Get_Game_Records(string playerName) {
        var _task = FirebaseManager.Instance.GetPlayerGameRecordReference(playerName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var keys in snapshot.Children)
                {
                    //print($"{keys.Key}  {keys.Value}");
                    record_keys.Add(keys.Value.ToString());

                }

            }

        });

        yield return new WaitUntil(() => _task.IsCompleted);

        for (int i = record_keys.Count - 1; i >= 0; i--)
        {
            var _task2 = FirebaseManager.Instance.GetGameReference().Child(record_keys[i]).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                var jsonRecord = task.Result.GetRawJsonValue();

                game_records.Add(JsonConvert.DeserializeObject<ClassManager.GameRecord>(jsonRecord));
                //print($"added {jsonRecord}");


            });
            yield return new WaitUntil(() => _task2.IsCompleted);
        }

        foreach (var item in game_records)
        {
            print(item.startTime);

        }

    }

    //[Serializable]

    private void OnEnable()
    {
       
    }
    void Test1()
    {

        string temp = "hello world";

        string temp2 = JsonConvert.SerializeObject(temp);
        print("1");
        //string temp3 = JsonConvert.DeserializeObject<string>(temp);
        print("2");
        string temp4 = JsonConvert.DeserializeObject<string>(temp2);

        print($"{temp}     {temp2}");
        print($"     {temp4}");
    }

    void Test2() {

        //ClassManager.PlayerLog tempLog = JsonConvert.DeserializeObject<ClassManager.PlayerLog>(null);
        ClassManager.PlayerLog temp = new();
        string a = JsonConvert.SerializeObject(temp);
        print("a = " + a);
        Test3(temp);

    }

    void Test3(object ss) {
        string a = JsonConvert.SerializeObject(ss);
        print("a = " + a);
    }

    IEnumerator Get_Game_Records_IEnumerator( )
    {
        List<(string,string)> record_keys = new();
        var _task = FirebaseManager.Instance.GetPlayerGameRecordReference("chow").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var keys in snapshot.Children)
                {
                    record_keys.Add((keys.Key.ToString(), keys.Value.ToString()));
                }
            }
        });
        yield return new WaitUntil(() => _task.IsCompleted);

        foreach (var t in record_keys) {
            print($"{t.Item1}    {t.Item2}");
        
        }

    }

    void Test4() {
        StartCoroutine(Get_Game_Records_IEnumerator());
    }

    void Test5() {
        PopMessageUI.Instance.Init_Pop(ClassManager.MessageType.friend_request, _name);
    }

    public string str="";
    void Test6() {
           
        for(int i = 0; i < 5; i++)
        {
            str += $"{i}{i}{i}\n";

        }
        print(str);
    }

    void Test7() {



        Dictionary<int, ClassManager.Room> t = new();

        for (int i = 0; i < 5; i++) {
            ClassManager.Room room = new(i, $"{i}{i}{i}");
            t.Add(i, room);

        }
        PlayerLocalData.Set_Rooms_List(t);

    }
    public float s;
    void Test8() {
        qq.sizeDelta = new Vector2(500f, s);
// 325  537 725
    }

    private void Update()
    {
        if (key1) {
            key1 = false;
            Test8();
            //FirebaseManager.Instance.Get_Game_Records("chow");

        }


       

        return;
    }



}
