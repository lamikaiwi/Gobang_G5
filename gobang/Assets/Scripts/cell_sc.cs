using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell_sc : MonoBehaviour
{

    bool isO;

    public GameObject white;
    public GameObject black;
    GameObject board;
    game_ma gameMaScript;
    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("board");
        gameMaScript = board.GetComponent<game_ma>();
        isO = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OO() {
        if (isO) {
            return;
        }

        Debug.Log("My name is " + gameObject.name);
        
        Vector3 pos = transform.position;

        if (gameMaScript.isWhite)
            Instantiate(white, pos, Quaternion.identity);
        else
            Instantiate(black, pos, Quaternion.identity);
        isO = true;
        board.SendMessage("place");
    }

}
