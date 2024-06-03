using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public TextMeshProUGUI highScoreText;
    //public TextMeshProUGUI scoreText;

    public GameObject GameOverUI;
    public static GameManager Instance;

    //target arrow reference
    private GameObject objIndicator;
    private GameObject exitIndicator;

    //collider switches to trigger when escaping
    public BoxCollider exitCollider;



    public GameState state;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        objIndicator = GameObject.FindGameObjectWithTag("ObjectTarget");
        exitIndicator = GameObject.FindGameObjectWithTag("ExitTarget");

        UpdateGameState(GameState.Invade);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Restart();
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Invade:
                HandleInvade();
                break;
            case GameState.Escape:
                HandleEscape();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    private void HandleInvade()
    {
        //Level Start
        //When player didnt get the pieces yet
        //reset elapsedTime
        //bool runTime = true;
        EnemyManager.Instance.canSpawn = true;
        EnemyManager.Instance.canFollow = true;

        objIndicator.SetActive(true);
        exitIndicator.SetActive(false);

        exitCollider.isTrigger = false;
    }

    private void HandleEscape()
    {
        //Pleyer got pieces and need to escape
        //Spawn more security guards
        //EnemyManager.Instance.canSpawn = true;

        objIndicator.SetActive(false);
        exitIndicator.SetActive(true);

        exitCollider.isTrigger = true;
    }

    private void HandleWin()
    {
        //Stop Time
        //Open Win Panel
        winPanel.SetActive(true);
        //Update HighScore
        CheckTimeScore();
        //scoreText.text = Timer.Instance.timerText.text;
        UpdateHighScore();
        //SetScore();
    }

    private void HandleLose()
    {
        //Stop Time
        //Open Lose Panel
        GameOver();
    }

    public void GameOver()
    {
        GameOverUI.SetActive(true);
    }

    public void Restart()
    {
        MusicManager.Instance.StopAllLoopingMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    ///////////////////////////////////////////////////////

    private void CheckTimeScore()
    {
        if (Timer.Instance.elapsedTime < PlayerPrefs.GetFloat("HighScore", 6039.999f))
        {
            PlayerPrefs.SetFloat("HighScore", Timer.Instance.elapsedTime);
        }
    }

    private void UpdateHighScore()
    {
        Debug.Log(PlayerPrefs.GetFloat("HighScore"));
        int minutes = Mathf.FloorToInt((PlayerPrefs.GetFloat("HighScore", 0)) / 60);
        int seconds = Mathf.FloorToInt((PlayerPrefs.GetFloat("HighScore", 0)) % 60);
        int miliseconds = (int)((PlayerPrefs.GetFloat("HighScore", 0)) * 1000) % 1000;
        Debug.Log(minutes + ":" + seconds + ":" + miliseconds);
        highScoreText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
    }
}

public enum GameState
{
    Invade,
    Escape,
    Win,
    Lose
}
