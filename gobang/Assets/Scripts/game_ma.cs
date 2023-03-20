using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_ma : MonoBehaviour
{
    public bool isWhite = true;

    void place() {
        if (isWhite == true)
            isWhite = false;
        else
            isWhite = true;
    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
