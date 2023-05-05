using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopMessageUI : MonoBehaviour
{
    public static PopMessageUI Instance { set; get; }

    [SerializeField] GameObject fail_pop;
    [SerializeField] TMP_Text outputText;

    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject bannedPanel;
    [SerializeField] Button bannedBtn1;
    [SerializeField] Button bannedBtn2;
    [SerializeField] Transform pop_message_prefab;
    [SerializeField] Transform pop_message_parent;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bannedBtn1?.onClick.AddListener(() => GameManager.Instance.Logout());
        bannedBtn2?.onClick.AddListener(() => GameManager.Instance.Logout());
    }


    public void Pop_Fail_Message(string _output) {
        outputText.text = _output;
        fail_pop.gameObject.SetActive(true);
    }

    public void Pop_LoadingPanel(bool input)
    {
        loadingPanel.SetActive(input);
    }

    public void Pop_BannedMessage() {
        bannedPanel.SetActive(true);
    }


    public void Init_Pop(ClassManager.MessageType messageType, string _message)
    {
        var pop_instanse = Instantiate(pop_message_prefab);

        RectTransform temp = pop_instanse.GetComponent<RectTransform>();
        RectTransform _parent = GetComponent<RectTransform>();
        temp.anchoredPosition = _parent.position;
        temp.anchorMin = new Vector2(1, 0);
        temp.anchorMax = new Vector2(0, 1);
        temp.pivot = new Vector2(0.5f, 0.5f);
        temp.sizeDelta = _parent.rect.size;
        temp.transform.SetParent(_parent);
        pop_instanse.localScale = new Vector3(1, 1, 1);
        pop_instanse.GetComponent<PopMessage>().Init(messageType, _message);

    }


}
