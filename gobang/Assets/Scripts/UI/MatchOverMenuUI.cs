using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MatchOverMenuUI : MonoBehaviour
{
    public static MatchOverMenuUI Instance { set; get; }

    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text totalCoinText;
    [SerializeField] TMP_Text totalScoreText;

    [SerializeField] GameObject scoreDecreaseIcon;
    [SerializeField] GameObject scoreIncreaseIcon;
    [SerializeField] GameObject winIcon;
    [SerializeField] GameObject loseIcon;

    int winningScore = 100;

    private void Awake()
    {
        Instance = this;
        //GameManager.Instance.OnMatchOver += GameOverUpdate;
    }

    private void OnEnable()
    {
        print($"rID = {PlayerLocalData.Get_RoomID()} ");
        bool isWinner = MatchData.Get_Is_Winner();
        int winnerScore;
        int loserScore;

        if (isWinner)
        {
            winnerScore = PlayerLocalData.GetPlayerScore();
            loserScore = OpponentData.GetPlayerScore();
        }
        else {
            loserScore = PlayerLocalData.GetPlayerScore();
            winnerScore = OpponentData.GetPlayerScore();
        }
        winnerScore = Mathf.Max(1, winnerScore);
        loserScore = Mathf.Max(1, loserScore);

        float scoreResult = loserScore / winnerScore;
        scoreResult = Mathf.Min(scoreResult, 1.5f);
        scoreResult = Mathf.Max(scoreResult, 0.5f);
        scoreResult = winningScore * scoreResult;
        int scoreResult_int = Mathf.RoundToInt(scoreResult);

        int coinsEarned = UnityEngine.Random.Range(5, 10);

        if (isWinner)
        {

            coinsEarned += 20;
            scoreIncreaseIcon.SetActive(true);
            scoreDecreaseIcon.SetActive(false);
            winIcon.SetActive(true);
            loseIcon.SetActive(false);
            scoreText.text = $"+{scoreResult_int}";
            totalScoreText.text = $"{PlayerLocalData.GetPlayerScore() + scoreResult_int}";
            PlayerLocalData.UpdataScore(scoreResult_int);
        }
        else
        {

            scoreIncreaseIcon.SetActive(false);
            scoreDecreaseIcon.SetActive(true);
            winIcon.SetActive(false);
            loseIcon.SetActive(true);
            scoreResult_int = Mathf.Min(PlayerLocalData.GetPlayerScore(), scoreResult_int);
            scoreText.text = $"-{scoreResult_int}";
            totalScoreText.text = $"{PlayerLocalData.GetPlayerScore() - scoreResult_int}";
            PlayerLocalData.UpdataScore(scoreResult_int * -1);
        }
        coinText.text = $"+{coinsEarned}";
        totalCoinText.text = $"{PlayerLocalData.GetPlayerCoin() + coinsEarned}";




        PlayerLocalData.UpdataCoin(coinsEarned);
    }


}
