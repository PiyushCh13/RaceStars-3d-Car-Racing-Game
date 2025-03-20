using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text LapText, currentlapTime , bestlapTime , carPosition , countdownText , goText , raceFinishResult;
    public GameObject resultScreen ,pauseScreen;
    public bool ispaused;

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
    }

    public void PauseUnpause()
    {
        ispaused = !ispaused;

        pauseScreen.SetActive(ispaused);

        if (ispaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }



}
