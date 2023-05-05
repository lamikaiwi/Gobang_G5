using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class SignupMenuUI : MonoBehaviour
{
    public static SignupMenuUI Instance { set; get; }
    [SerializeField] TMP_InputField inputName;
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] TMP_InputField inputPassword2;

    private void Awake()
    {
        Instance = this;
    }

    string allowChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public bool CheckInputValid() {
        if (inputName.text.Length < 2)
        {
            Pop_Fail_Message("UserName invalid");
            return false;
        }
        if (string.Equals("randomPlayer", inputName.text))
        {
            Pop_Fail_Message("UserName invalid");
            return false;
        }
        if (string.Equals("server", inputName.text))
        {
            Pop_Fail_Message("UserName invalid");
            return false;
        }
        if (inputName.text.Length >= 5 && inputName.text.Substring(0, 5) == "admin")
        {
            Pop_Fail_Message("UserName invalid");
            return false;
        }
        if (inputPassword.text.Length < 2)
        {
            Pop_Fail_Message("Password invalid");
            return false;
        }

        if (!Regex.IsMatch(inputName.text + inputPassword.text + inputPassword2.text, $"^[{allowChars}]+$"))
        {
            Pop_Fail_Message("Character forbidden");
            return false;
        }

        return true;
    }

    public bool CheckPasswordTheSame() {
        if (String.Equals(inputPassword.text, inputPassword2.text)) {
            return true;
        }
        return false;
    }

    public void Register()
    {
        if (!CheckInputValid()) {
            return;
        }
        if (!CheckPasswordTheSame()) {
            Pop_Fail_Message("Password are not the same ");
            return;
        }

        FirebaseManager.Instance.Before_Registering(inputName.text, inputPassword.text);
    }

    public void Pop_Fail_Message(string output) {
        PopMessageUI.Instance.Pop_Fail_Message(output);
    }


    private void OnEnable()
    {
        inputName.text = "";
        inputPassword.text = "";
        inputPassword2.text = "";
    }
}
