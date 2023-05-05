using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileMenuUI : MonoBehaviour
{
    public static ProfileMenuUI Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] TMP_Text nameField;
    [SerializeField] TMP_Text coinField;
    [SerializeField] TMP_Text scoreField;

    private void OnEnable()
    {
        nameField.text = PlayerLocalData.GetPlayerName();
        coinField.text = PlayerLocalData.GetPlayerCoin().ToString();
        scoreField.text = PlayerLocalData.GetPlayerScore().ToString();
    }
}
