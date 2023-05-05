using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { set; get; }

    public UnityEvent OnUserLogin;

    public UnityEvent OnUserStartGame;

    public UnityEvent OnUserEndGame;

    private void Awake()
    {
        Instance = this;
    }

    public void Event_OnUserLogin()
    {
        OnUserLogin?.Invoke();
    }

    public void Event_OnUserStartGame()
    {
        OnUserStartGame?.Invoke();
    }

    public void Event_OnUserEndGame()
    {
        OnUserEndGame?.Invoke();
    }

}
