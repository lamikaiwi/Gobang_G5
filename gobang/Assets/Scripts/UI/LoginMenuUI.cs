using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net;
using Unity.Netcode;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LoginMenuUI : MonoBehaviour
{
    public static LoginMenuUI Instance { set; get; }

    public UnityEvent OnServerStart;

    [SerializeField] TMP_InputField inputName;
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] Button settingBtn;
    [SerializeField] GameObject settingMenu;


    [SerializeField] GameObject Player_List_Menu;

    public GameObject quantumConsole;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        settingBtn?.onClick.AddListener(() => settingMenu.SetActive(true));

    }

    string allowChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public bool CheckInputValid()
    {
        if (inputName.text.Length < 2)
        {
            Pop_Fail_Message("UserName invalid");
            return false;
        }
        if (inputPassword.text.Length < 2)
        {
            Pop_Fail_Message("Password invalid");
            return false;
        }
        if (!Regex.IsMatch(inputName.text + inputPassword.text, $"^[{allowChars}]+$"))
        {
            Pop_Fail_Message("Character forbidden");
            return false;
        }

        return true;
    }

    public void Login() {
        if (string.Equals(inputName.text, "server")) {

            string ipAddress = inputPassword.text.TrimEnd('\n');
            print("IP address = " + ipAddress + "-");
            PlayerLocalData.SetTargetIP(ipAddress);
            quantumConsole.SetActive(true);
            Player_List_Menu.SetActive(true);
            //string ipAddress = new WebClient().DownloadString("https://icanhazip.com/");

            FirebaseManager.Instance.GetServerReference().SetValueAsync(ipAddress).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    print("Success to update server ip address to firebase!");
                    GameManager.Instance.StartServerByAddress(ipAddress);
                    gameObject.SetActive(false);
                }
            });
            return;
        }

        if (!CheckInputValid()) 
            return;

        FirebaseManager.Instance.Before_Login(inputName.text, inputPassword.text);
    }

    public void Pop_Fail_Message(string output) {
        PopMessageUI.Instance.Pop_Fail_Message(output);
    }



}
