using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { set; get; }

    [SerializeField] Button profileBtn;
    [SerializeField] Button gameRecordBtn;
    [SerializeField] Button friendListBtn;
    [SerializeField] Button shoppingBtn;
    [SerializeField] GameObject profileMenu;
    [SerializeField] GameObject friendListMenu;
    [SerializeField] GameObject gameRecordUI;
    [SerializeField] GameObject shoppingMenu;

    [SerializeField] Button randomBtn;
    [SerializeField] Button multiplayBtn;
    [SerializeField] Button exitBtn;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        profileBtn?.onClick.AddListener(() => Show_Profile());
        gameRecordBtn?.onClick.AddListener(() => Show_GameRecord());
        friendListBtn?.onClick.AddListener(() => Show_FriendList());
        randomBtn?.onClick.AddListener(() => RandomPlay());
        multiplayBtn?.onClick.AddListener(() => Multiplay());
        shoppingBtn?.onClick.AddListener(() => Show_ShoppingMenu());
        exitBtn?.onClick.AddListener(() => GameManager.Instance.Logout());
    }

    void Show_Profile() {
        profileMenu.SetActive(true);

    }

    void Show_FriendList() {
        friendListMenu.SetActive(true);
    }
    void Show_GameRecord() {
        gameRecordUI.SetActive(true);
    }

    void Show_ShoppingMenu() {
        shoppingMenu.SetActive(true);
    }

    void Multiplay() {
        GameManager.Instance.OnRoomListMenu();
    }
    void RandomPlay()
    {
        GameManager.Instance.OnStartRandomMode();
    }

}
