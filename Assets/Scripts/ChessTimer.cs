﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ChessTimer : MonoBehaviour
{
    public float whiteTimerValue = 300;
    public float blackTimerValue = 300;

    public TextMeshProUGUI topTimerText;
    public TextMeshProUGUI bottomTimerText;

    private void Start() 
    {
        UpdateText(topTimerText, blackTimerValue);
        UpdateText(bottomTimerText, whiteTimerValue);
    }

    public void StartCountdown()
    {
        StopAllCoroutines();
        if (GameManager.instance.whiteTurn)
        {
            StartCoroutine(StartWhiteCountdown());
        }
        else
        {
            StartCoroutine(StartBlackCountdown());
        }
    }

    // TODO: Combine these two functions into one..
    // Problem: can't pass parameters as reference types :(
    public IEnumerator StartBlackCountdown()
    {
        while (blackTimerValue > 0)
        {
            yield return new WaitForSeconds(1.0f);

            blackTimerValue--;
            UpdateText(!GameManager.whiteSide ? bottomTimerText : topTimerText, blackTimerValue);
        }
        GameManager.instance.ActivateGameOver("White wins by timeout");
    }

    public IEnumerator StartWhiteCountdown()
    {
        while (whiteTimerValue > 0)
        {
            yield return new WaitForSeconds(1.0f);

            whiteTimerValue--;
            UpdateText(GameManager.whiteSide ? bottomTimerText : topTimerText, whiteTimerValue);
        }
        GameManager.instance.ActivateGameOver("Black wins by timeout");
    }

    public void UpdateText(TextMeshProUGUI text, float timerValue)
    {
        TimeSpan time = TimeSpan.FromSeconds(timerValue);
        text.text = time.ToString(@"mm\:ss");
    }
}