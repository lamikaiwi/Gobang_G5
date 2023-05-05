using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopMessage : MonoBehaviour
{

    [SerializeField] TMP_Text Text;

    string info = "";
    ClassManager.MessageType messageType;
    public void Init(ClassManager.MessageType _messageType, string message)
    {
        info = message;
        messageType = _messageType;

        string output="";
        if (_messageType == ClassManager.MessageType.friend_request) {
            output = $"You've got a friend request from {message}";
        }
        Text.text = output;
    }


    public void Agree()
    {
        GameManager.Instance.Pop_Message_Response(messageType, info, true);
        Destroy(gameObject);
            
    }

    public void Disagree()
    {
        GameManager.Instance.Pop_Message_Response(messageType, info, false);
        Destroy(gameObject);
    }



}
