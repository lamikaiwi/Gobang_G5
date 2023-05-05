using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardBG : MonoBehaviour
{
    public static ChessBoardBG Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }

    public Sprite[] sprites;


    private void OnEnable()
    {
        int bg = PlayerLocalData.Get_SelectedBackground();
        Change_Skin(bg);
    }

    public void Change_Skin(int num) {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[num];
        gameObject.GetComponent<SpriteRenderer>().size = new Vector2(9, 9);
    }

}
