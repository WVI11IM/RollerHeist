using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Cinemachine;

public class MainMenu : MonoBehaviour
{   
    public MenuState menuState;
    public int levelToSelect = 1;
    private bool selectedLevel = false;

    public UnityEvent titleScreenEvent;
    public UnityEvent mainScreenEvent;
    public UnityEvent playScreenEvent;
    public UnityEvent levelsScreenEvent;
    public UnityEvent optionsScreenEvent;
    public UnityEvent creditsScreenEvent;
    public UnityEvent controlsScreenEvent;
    public UnityEvent audioScreenEvent;
    public UnityEvent skinsScreenEvent;
    public UnityEvent exitScreenEvent;
  
    UITransitionManager uiTransitionManager;
    public CinemachineVirtualCamera titleCamera, mainCamera;
    public Animator levelSelectAnimator;
    public Animator menuOptionsAnimator;

    private void Start()
    {

        uiTransitionManager = GetComponent<UITransitionManager>();
        AudioMixerManager.Instance.LoadVolumes();
        AudioMixerManager.Instance.UpdateSliders();
        MusicManager.Instance.PlayMusicLoop("beanbag");
        if (PlayerPrefs.GetInt("titleScreenActivated") == 0)
        {
            ChangeMenuState("Title");
            PlayerPrefs.SetInt("titleScreenActivated", 1);
        }
        else
        {
            ChangeMenuState("Main");
        }

        //didTutorial
        //completedLevel
    }

    private void Update()
    {
        if (menuState == MenuState.Levels)
        {
            levelToSelect = PlayerPrefs.GetInt("levelToSelect");
            levelSelectAnimator.SetInteger("level", levelToSelect);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadScene("Tutorial");
        }
    }

    public void ChangeMenuState(string state)
    {
        switch (state)
        {
            case "Title":
                menuState = MenuState.Title;
                break;

            case "Main":
                menuState = MenuState.Main;
                break;

            case "Play":
                menuState = MenuState.Play;
                break;

            case "Levels":
                menuState = MenuState.Levels;
                break;

            case "Options":
                menuState = MenuState.Options;
                break;

            case "Controls":
                menuState = MenuState.Controls;
                break;

            case "Audio":
                menuState = MenuState.Audio;
                break;

            case "Credits":
                menuState = MenuState.Credits;
                break;

            case "Skins":
                menuState = MenuState.Skins;
                break;

            case "Exit":
                menuState = MenuState.Exit;
                break;
        }
        UpdateMenuState();
    }

    public void UpdateMenuState()
    {
        switch (menuState)
        {
            case MenuState.Title:
                titleScreenEvent.Invoke();
                break;

            case MenuState.Main:
                mainScreenEvent.Invoke();
                break;

            case MenuState.Play:
                playScreenEvent.Invoke();
                break;

            case MenuState.Levels:
                LoadLevelSelection();
                levelsScreenEvent.Invoke();
                break;

            case MenuState.Options:
                optionsScreenEvent.Invoke();
                break;

            case MenuState.Controls:
                controlsScreenEvent.Invoke();
                menuOptionsAnimator.SetInteger("menuState", 1);
                break;

            case MenuState.Audio:
                audioScreenEvent.Invoke();
                menuOptionsAnimator.SetInteger("menuState", 0);
                break;

            case MenuState.Credits:
                creditsScreenEvent.Invoke();
                break;

            case MenuState.Skins:
                skinsScreenEvent.Invoke();
                break;

            case MenuState.Exit:
                exitScreenEvent.Invoke();
                break;
        }
    }

    public void ChangeLevel(int addOrSubtractOne)
    {
        levelToSelect += addOrSubtractOne;
        levelToSelect = Mathf.Clamp(levelToSelect, 1, 3);
        PlayerPrefs.SetInt("levelToSelect", levelToSelect);
    }

    public void LoadLevelSelection()
    {
        if (PlayerPrefs.GetInt("levelToSelect") == 0)
        {
            PlayerPrefs.SetInt("levelToSelect", 1);
        }
        else
        {
            levelToSelect = PlayerPrefs.GetInt("levelToSelect");
        }

        if (PlayerPrefs.GetInt("didTutorial") == 0)
        {
            //ask about tutorial
        }
    }

    public void SelectLevel()
    {
        if (!selectedLevel)
        {
            selectedLevel = true;
        }
        else
        {
            selectedLevel = false;
        }
        levelSelectAnimator.SetBool("selectedLevel", selectedLevel);
    }

    public void PlayGame()
    {
        MusicManager.Instance.StopAllLoopingMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        MusicManager.Instance.StopAllLoopingMusic();
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        MusicManager.Instance.StopAllLoopingMusic();
        Application.Quit();

    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("titleScreenActivated", 0);
    }
}

public enum MenuState 
{
    Title,
    Main,
    Play,
    Levels,
    Options,
    Controls,
    Audio,
    Skins,
    Credits,
    Exit
}

