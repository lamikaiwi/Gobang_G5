using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class readInputKey : MonoBehaviour
{
    /*
     * This script mainly used in login and sign-up stage, allow users to use "TAB", "Enter" short keys.
     * Say when the user is typing the name in the name_text_field, he can press TAB to move to password_text_field
     * 
     */
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
