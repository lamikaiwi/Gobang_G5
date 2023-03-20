using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cell_controller : MonoBehaviour
{
    float cell_size = 52.5f;
    Vector2 offset;
    RectTransform[] childTransforms;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector2(31.25f, -31.25f);
        childTransforms = this.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform childTransform in childTransforms)
        {
            Vector2 childPosition = childTransform.anchoredPosition;
            Vector2 child_index = childPosition - offset;
            child_index.x /= cell_size;
            child_index.y /= cell_size;
            cell_script cell_Script = childTransform.GetComponent<cell_script>();
            if (cell_Script != null)
                cell_Script.set_pos(child_index);
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
