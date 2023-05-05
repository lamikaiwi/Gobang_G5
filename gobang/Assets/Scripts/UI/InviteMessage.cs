using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InviteMessage : MonoBehaviour
{
    [SerializeField] Button agreeBtn;
    [SerializeField] Button disgreeBtn;
    [SerializeField] TMP_Text inviteText;
    (string, int) message;

    private void Start()
    {
        agreeBtn?.onClick.AddListener(() => Argee());
        disgreeBtn?.onClick.AddListener(() => disargee());
    }

    public void Init((string, int) _message) {
        message = _message;
        inviteText.text = $"{message.Item1} invite you to room {message.Item2}";

    }


    void Argee() {

        GameManager.Instance.OnEnterWaitingRoom();
        GameManager.Instance.Join_Room(message.Item2);
        gameObject.SetActive(false);
    }
    void disargee()
    {
        gameObject.SetActive(false);
    }

}
