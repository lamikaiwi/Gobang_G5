using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Admin_Grid : MonoBehaviour
{

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
