using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grids : MonoBehaviour
{
    Vector2 offset = new Vector2(-3.969f, -3.969f);
    float cell_size = 0.441f;
    Transform[] childTransforms;


    // Start is called before the first frame update
    void Start()
    {
        int x = 0, y = 0;
        childTransforms = this.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childTransforms)
        {
            if (!childTransform.gameObject.CompareTag("grid"))
                continue;

            Vector2 childPosition = new Vector2(x * cell_size, y * cell_size);
            childPosition += offset;
            childTransform.position = childPosition;


            x++;
            if (x >18) {
                x = 0;
                y++;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
