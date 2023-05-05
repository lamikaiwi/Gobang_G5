using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShoppingMenuUI : MonoBehaviour
{
    public static  ShoppingMenuUI Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject chessBoard;

    [SerializeField] TMP_Text totalCoin;
    [SerializeField] Button buyBtn;
    [SerializeField] TMP_Text buyBtn_text;

    [SerializeField] Button btn0;
    [SerializeField] Button btn1;
    [SerializeField] Button btn2;
    [SerializeField] GameObject lock2;
    [SerializeField] Button btn3;
    [SerializeField] GameObject lock3;
    [SerializeField] Button btn4;
    [SerializeField] GameObject lock4;
    [SerializeField] Button btn5;
    [SerializeField] GameObject lock5;

    [SerializeField] Button Exit;

    List<int> bg_list = new();

    Dictionary<int, int> price_dict = new();
    int current_bg;
    private void Start()
    {
        btn0?.onClick.AddListener(() => Change_Skin(0));
        btn1?.onClick.AddListener(() => Change_Skin(1));
        btn2?.onClick.AddListener(() => Change_Skin(2));
        btn3?.onClick.AddListener(() => Change_Skin(3));
        btn4?.onClick.AddListener(() => Change_Skin(4));
        btn5?.onClick.AddListener(() => Change_Skin(5));
        buyBtn?.onClick.AddListener(() => Purchase());
        Exit?.onClick.AddListener(() => Back());
        price_dict.Add(2, 10);
        price_dict.Add(3, 10);
        price_dict.Add(4, 10);
        price_dict.Add(5, 10);
    }


    private void OnEnable()
    {
        totalCoin.text = PlayerLocalData.GetPlayerCoin().ToString();
        chessBoard.SetActive(true);
        current_bg = 0;
        Show_List();
    }

    void Back() {
        GameManager.Instance.OnEnterMainMenu();
    }

    void Show_List() {
        bg_list = PlayerLocalData.Get_Background_Dict();

        lock2.SetActive(true);
        lock3.SetActive(true);
        lock4.SetActive(true);
        lock5.SetActive(true);



        if (bg_list.Contains(2))
            lock2.SetActive(false);
        if (bg_list.Contains(3))
            lock3.SetActive(false);
        if (bg_list.Contains(4))
            lock4.SetActive(false);
        if (bg_list.Contains(5))
            lock5.SetActive(false);

    }

    void Change_Skin(int skin_no)
    {
        ChessBoardBG.Instance.Change_Skin(skin_no);
        current_bg = skin_no;
        if(bg_list.Contains(skin_no))
        {
            buyBtn_text.text = "Equip";
        }
        else {
            buyBtn_text.text = "Purchase";
        }
    }

    void Purchase() {
        if (bg_list.Contains(current_bg))
        {
            PlayerLocalData.Set_SelectedBackground(current_bg);
        
        }
        else {
            if (PlayerLocalData.GetPlayerCoin() >= price_dict[current_bg]) {
                bg_list.Add(current_bg);
                PlayerLocalData.Set_Background_Dict(bg_list);
                PlayerLocalData.Set_SelectedBackground(current_bg);
                buyBtn_text.text = "Equip";
                totalCoin.text = (PlayerLocalData.GetPlayerCoin() - 10).ToString();
                PlayerLocalData.UpdataCoin(price_dict[current_bg] * -1);
                FirebaseManager.Instance.GetItemListReference(PlayerLocalData.GetPlayerName()).Child(current_bg.ToString()).SetValueAsync(true);
                
                Show_List();
            }
        }
    }


}
