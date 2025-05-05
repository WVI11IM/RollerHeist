using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public int levelNumber;
    public InputManager inputManager;

    public GameObject winPanel;
    public TextMeshProUGUI highScoreText;

    public Animator uiAnimator;
    private Animator pauseAnimator;
    public static GameManager Instance;

    public GameObject pausePanel;
    public bool canPause = true;
    public bool isPaused = false;

    //target arrow reference
    private GameObject objIndicator;
    private GameObject exitIndicator;

    //collider switches to trigger when escaping
    public BoxCollider exitCollider;

    public GameState state;

    public Button pauseButtonToSelect;
    public Button gameOverButtonToSelect;
    public Button winButtonToSelect;

    public static event Action<GameState> OnGameStateChange;

    private InputAction cancelAction;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AudioMixerManager.Instance.LoadVolumes();
        objIndicator = GameObject.FindGameObjectWithTag("ObjectTarget");
        exitIndicator = GameObject.FindGameObjectWithTag("ExitTarget");
        pauseAnimator = pausePanel.GetComponent<Animator>();

        if(state != GameState.None) UpdateGameState(GameState.Invade);

        var inputModule = EventSystem.current?.GetComponent<InputSystemUIInputModule>();
        if (inputModule != null)
        {
            cancelAction = inputModule.cancel;
            if (cancelAction != null)
            {
                cancelAction.Enable();
                cancelAction.performed += OnCancelPerformed;
            }
        }
        else
        {
            Debug.LogError("InputSystemUIInputModule not found in the EventSystem!");
        }
    }

    private void Update()
    {
        if (inputManager.paused)
        {
            if (!isPaused && canPause)
            {
                Pause();
            }
            else if (isPaused)
            {
                Resume(); 
            }
        }
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
            case GameState.None:
                HandleNone();
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    private void HandleInvade()
    {
        EnemyManager.Instance.canSpawn = true;
        EnemyManager.Instance.canFollow = true;

        objIndicator.SetActive(true);
        exitIndicator.SetActive(false);

        exitCollider.isTrigger = false;
    }

    private void HandleEscape()
    {
        objIndicator.SetActive(false);
        exitIndicator.SetActive(true);

        exitCollider.isTrigger = true;
    }

    private void HandleWin()
    {
        canPause = false;
        winButtonToSelect.Select();
        PlayerPrefs.SetInt("justPlayedLevel", 1);
        SFXManager.Instance.PlayUISFX("missaoCumprida");

        //Stop Time
        //Open Win Panel
        winPanel.SetActive(true);
        //Update HighScore
        CheckTimeScore();

        SFXManager.Instance.StopAllLoopingSFX();

        UpdateHighScore();
        PlayerPrefs.Save();

        //Freeze time
        AudioListener.pause = true;
        Time.timeScale = 0f;

        AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0);
    }

    private void HandleLose()
    {
        //Stop Time
        //Open Lose Panel
        GameOver();
    }

    public void GameOver()
    {
        gameOverButtonToSelect.Select();
        AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0);
        canPause = false;
        uiAnimator.SetBool("gameIsOver", true);
        StartCoroutine(GameOverPause());
        MusicManager.Instance.StopAllLoopingMusic();
        SFXManager.Instance.StopAllLoopingSFX();
        SFXManager.Instance.PlayUISFX("gameOver");
        SFXManager.Instance.PlaySFX("sirene");
    }

    private void HandleNone()
    {
        /*
        EnemyManager.Instance.canSpawn = false;
        EnemyManager.Instance.canFollow = false;
        */

        objIndicator.SetActive(false);
        exitIndicator.SetActive(false);

        //exitCollider.isTrigger = false;
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("justPlayedLevel", 0);
        SFXManager.Instance.PlayUISFX("play");
        AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0);
        AudioListener.pause = false;
        Time.timeScale = 1f;
        MusicManager.Instance.StopAllLoopingMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SFXManager.Instance.PlayUISFX("negativo2");
        AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0);
        AudioListener.pause = false;
        Time.timeScale = 1f;
        MusicManager.Instance.StopAllLoopingMusic();
        SceneManager.LoadScene("Menu");
    }

    public void Pause()
    {
        pauseButtonToSelect.Select();
        SFXManager.Instance.PlayUISFX("negativo1");
        SFXManager.Instance.PlayUISFX("woosh2");
        AudioMixerManager.Instance.ChangeMusicSnapshot("Paused", 0f);   //COLOCAR SCRIPT PARA EVENT DA ANIMACAO
        pausePanel.SetActive(true);
        isPaused = true;
        AudioMixerManager.Instance.UpdateSliders();
        Time.timeScale = 0f;
        AudioListener.pause = true;
        SFXManager.Instance.PauseLoopingSFX();
    }

    public void Resume()
    {
        SFXManager.Instance.PlayUISFX("positivo1");
        AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0f);
        AudioListener.pause = false;
        SFXManager.Instance.ResumeLoopingSFX();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        isPaused = false;
    }

    public void AudioOptions()
    {
        if(pauseAnimator.GetBool("audioOpen") == false)
        {
            SFXManager.Instance.PlayUISFX("positivo1");
            SFXManager.Instance.PlayUISFX("woosh1");
            pauseAnimator.SetBool("controlsOpen", false);
            pauseAnimator.SetBool("audioOpen", true);
        }
        else
        {
            pauseAnimator.SetBool("audioOpen", false);
        }
    }

    public void ControlsOptions()
    {
        if (pauseAnimator.GetBool("controlsOpen") == false)
        {
            SFXManager.Instance.PlayUISFX("positivo1");
            SFXManager.Instance.PlayUISFX("woosh1");
            pauseAnimator.SetBool("audioOpen", false);
            pauseAnimator.SetBool("controlsOpen", true);
        }
        else
        {
            pauseAnimator.SetBool("controlsOpen", false);
        }
    }

    ///////////////////////////////////////////////////////

    private void CheckTimeScore()
    {
        if (Timer.Instance.elapsedTime < PlayerPrefs.GetFloat("Level" + levelNumber + "HighScore", 6039.999f))
        {
            PlayerPrefs.SetFloat("Level" + levelNumber + "HighScore", Timer.Instance.elapsedTime);
        }
    }

    private void UpdateHighScore()
    {
        Debug.Log(PlayerPrefs.GetFloat("Level" + levelNumber + "HighScore"));
        int minutes = Mathf.FloorToInt((PlayerPrefs.GetFloat("Level" + levelNumber + "HighScore", 0)) / 60);
        int seconds = Mathf.FloorToInt((PlayerPrefs.GetFloat("Level" + levelNumber + "HighScore", 0)) % 60);
        int miliseconds = (int)((PlayerPrefs.GetFloat("Level" + levelNumber + "HighScore", 0)) * 1000) % 1000;
        Debug.Log(minutes + ":" + seconds + ":" + miliseconds);
        switch (PlayerPrefs.GetInt("languageInt"))
        {
            case 0: //Português
                highScoreText.text = "Tempo Recorde:\n" + minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
                break;
            case 1: //Inglês
                highScoreText.text = "Best time:\n" + minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
                break;
        }
    }

    IEnumerator GameOverPause()
    {
        for (int i = 0; i < 4; i++)
        {
            Time.timeScale -= 0.25f;
            yield return new WaitForSeconds(1);
        }
    }
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel action performed via gamepad!");
        // Add your logic here
    }

    void OnDestroy()
    {
        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancelPerformed;
            cancelAction.Disable();
        }
    }
}

public enum GameState
{
    Invade,
    Escape,
    Win,
    Lose,
    None
}
