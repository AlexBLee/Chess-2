﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    public Button onlineButton;

    public Button whiteButton;
    public Button blackButton;
    public Button randomButton;

    public GameObject onlineMenu;
    public GameObject selectionPanel;
    public GameObject selectionPanel2;

    public TMP_InputField timeField;
    public TMP_InputField incField;

    // Start is called before the first frame update
    void Start()
    {
        selectionPanel.gameObject.SetActive(false);

        playButton.onClick.AddListener(PlayGame);
        onlineButton.onClick.AddListener(ShowOnlineMenu);
        quitButton.onClick.AddListener(Application.Quit);

        whiteButton.onClick.AddListener(ChooseWhite);
        blackButton.onClick.AddListener(ChooseBlack);
        randomButton.onClick.AddListener(ChooseRandom);

        GetPlayerPrefTimes();
    }

    public void PlayGame()
    {
        playButton.gameObject.SetActive(false);
        onlineButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        selectionPanel.gameObject.SetActive(true);
        selectionPanel2.gameObject.SetActive(true);
    }

    public void ShowOnlineMenu()
    {
        playButton.gameObject.SetActive(false);
        onlineButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        onlineMenu.gameObject.SetActive(true);
    }

    public void ChooseWhite()
    {
        GameManager.whiteSide = true;
        SceneManager.LoadScene("Main");
    }

    public void ChooseBlack()
    {
        GameManager.whiteSide = false;
        SceneManager.LoadScene("Main");
    }

    public void ChooseRandom()
    {
        // The reason it's 0,2 rather than 0,1 is because the max value is not inclusive
        GameManager.whiteSide = (Random.Range(0,2) == 0) ? true : false;
        SceneManager.LoadScene("Main");
    }

    public void GetPlayerPrefTimes()
    {
        timeField.text = PlayerPrefs.GetInt("TimePerSide", 5).ToString();
        incField.text = PlayerPrefs.GetInt("IncTime", 5).ToString();
    }

    public void SetTime(string value)
    {
        PlayerPrefs.SetInt("TimePerSide", int.Parse(value));
    }

    public void SetIncrementTime(string value)
    {
        PlayerPrefs.SetInt("IncTime", int.Parse(value));
    }
}
