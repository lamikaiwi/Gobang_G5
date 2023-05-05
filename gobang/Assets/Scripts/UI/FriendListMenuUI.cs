using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendListMenuUI : MonoBehaviour
{
    public static FriendListMenuUI Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] GameObject friendGrid;
    [SerializeField] Transform friendGrid_parent;
    [SerializeField] TMP_InputField add_friend;
    [SerializeField] Button btn;
    List<GameObject> friendList = new();

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        btn?.onClick.AddListener(() => Add_Friend());
        if (GameManager.Instance.IsAdmin())
            btn.interactable = false;
    }

    public void Refresh()
    {
        foreach (var item in friendList)
        {
            Destroy(item);
        }
        friendList.Clear();
        foreach (var fd in PlayerLocalData.Get_Friend_Dict())
        {
            Add_To_List(fd.Key, fd.Value);
        }
    }


    public void Add_To_List(string _name, int state)
    {
        var fd_instanse = Instantiate(friendGrid);
        fd_instanse.transform.SetParent(friendGrid_parent);
        if (state == 2)
            fd_instanse.transform.SetSiblingIndex(0);
        
        fd_instanse.transform.localScale = new Vector3(1, 1, 1);
        Friend_Grid temp = fd_instanse.GetComponent<Friend_Grid>();
        temp.Init(state, _name);
        friendList.Add(fd_instanse);

    }

    public void Add_Friend() {  //btn
        GameManager.Instance.Send_Friend_Request(add_friend.text);
        add_friend.text = "";
    }


}
