using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Admin_Grid : MonoBehaviour
{
    /*
     *  This script is to init the text, which is a user name, in the button that located in user-list
     * 
     */
    public TMP_Text nameField;
    public Button button;

    private void Start()
    {
        button?.onClick.AddListener(() => AdminMenuUI.Instance.OnGridClicked(nameField.text));
    }


    public void Init(string _name) {
        nameField.text = _name;
    }
}
