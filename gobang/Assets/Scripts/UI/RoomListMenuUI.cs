using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
public class RoomListMenuUI : MonoBehaviour
{
    public static RoomListMenuUI Instance { set; get; }
    private void Awake() { Instance = this; }



    [SerializeField] Button hostGameBtn;
    [SerializeField] Button cancelBtn;
    [SerializeField] GameObject roomGrid;
    [SerializeField] Transform roomGrid_parent;

    private void Start()
    {
        hostGameBtn?.onClick.AddListener(() => Host());
        cancelBtn?.onClick.AddListener(() => GameManager.Instance.OnEnterMainMenu());
    }

    List<GameObject> roomGridList = new();

    private void OnEnable()
    {
        Refresh();

    }

    public void Refresh() {
        foreach (var item in roomGridList)
        {
            Destroy(item);
        }
        roomGridList.Clear();
        foreach (KeyValuePair<int, ClassManager.Room> keyValuePair in PlayerLocalData.Get_Rooms_List())
        {
            var room_instanse = Instantiate(roomGrid);
            roomGridList.Add(room_instanse);
            room_instanse.transform.SetParent(roomGrid_parent);
            room_instanse.transform.localScale = new Vector3(1, 1, 1);
            room_instanse.GetComponent<Room_Grids>().Init(keyValuePair);
        }
    }




    void Host() {
        GameManager.Instance.Host_Room();
        this.gameObject.SetActive(false);
    }


}
