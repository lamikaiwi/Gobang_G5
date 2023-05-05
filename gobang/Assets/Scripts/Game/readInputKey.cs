using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class readInputKey : MonoBehaviour
{
    public UnityEvent onKeyPressed;

    [SerializeField] private KeyCode[] getKey;


    void Update()
    {
        if (Input.anyKeyDown)
            foreach (KeyCode key in getKey){
                if (Input.GetKeyDown(key))
                {
                    onKeyPressed?.Invoke();
                }
                
            }
    }

    public void Click() {
        onKeyPressed?.Invoke();
    }


}
