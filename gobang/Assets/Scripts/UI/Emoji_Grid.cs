using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Emoji_Grid : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] TMP_Text emoji;
    [SerializeField] TMP_InputField inputField;

    private void Start()
    {
        btn?.onClick.AddListener(() => OnClick());
    }

    void OnClick()
    {
        inputField.text += emoji.text;

    }


}
