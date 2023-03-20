using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid_script : MonoBehaviour
{
    
    public void on_click()
    {
        transform.parent.parent.GetComponent<chessBoardManager>().grid_clicked(transform);

    }

}
