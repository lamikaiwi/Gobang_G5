using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class cell_script : MonoBehaviour
{

    Vector2 pos;
    Button btn;
    chessBoardManager grandparent;
    public void click() {
        btn.interactable = false;
        //grandparent.make_a_move(pos);
    }

    public void set_pos(Vector2 vec) {
        pos = vec;
    }

    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(click);
        grandparent = gameObject.transform.parent.parent.GetComponent<chessBoardManager>();
    }

}
