using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{   
    public MenuState menuState;
    public int levelToSelect = 1;
    public int numberOfLevels = 2;
    public int missionTokens = 0;
    public int levelMedals = 0;
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
    public UnityEvent tutorialScreenEvent;
  
    UITransitionManager uiTransitionManager;
    public CinemachineVirtualCamera titleCamera, mainCamera;
    public Animator levelSelectAnimator;
    public Animator menuOptionsAnimator;
    public TextMeshProUGUI tokenText;

    private void Start()
    {
        CheckMissionTokens();
        CheckLevelMedals();

        uiTransitionManager = GetComponent<UITransitionManager>();
        AudioMixerManager.Instance.LoadVolumes();
        AudioMixerManager.Instance.UpdateSliders();
        MusicManager.Instance.PlayMusicLoop("beanbag");
        if (PlayerPrefs.GetInt("titleScreenActivated") == 0)
        {
            ChangeMenuState("Title");
            PlayerPrefs.SetInt("titleScreenActivated", 1);
            PlayerPrefs.SetInt("justPlayedLevel", 0);
            PlayerPrefs.SetInt("justPlayedTutorial", 0);
        }
        else
        {
            if(PlayerPrefs.GetInt("justPlayedTutorial") == 1)
            {
                ChangeMenuState("Play");
                PlayerPrefs.SetInt("justPlayedLevel", 0);
                PlayerPrefs.SetInt("justPlayedTutorial", 0);
            }
            else if (PlayerPrefs.GetInt("justPlayedLevel") == 1)
            {
                ChangeMenuState("Levels");
                PlayerPrefs.SetInt("justPlayedLevel", 0);
                PlayerPrefs.SetInt("justPlayedTutorial", 0);
            }
            else
            {
                ChangeMenuState("Main");
                PlayerPrefs.SetInt("justPlayedLevel", 0);
                PlayerPrefs.SetInt("justPlayedTutorial", 0);
            }
        }
    }

    private void Update()
    {
        if (menuState == MenuState.Levels)
        {
            levelToSelect = PlayerPrefs.GetInt("levelToSelect");
            levelSelectAnimator.SetInteger("level", levelToSelect);
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

            case "Tutorial":
                menuState = MenuState.Tutorial;
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

            case MenuState.Tutorial:
                tutorialScreenEvent.Invoke();
                break;
        }
    }

    public void TutorialCheck()
    {
        if (missionTokens == 0 && PlayerPrefs.GetInt("didTutorial") == 0)
        {
            ChangeMenuState("Tutorial");
        }
        else ChangeMenuState("Play");
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
            //switch case for level sprites
        }
        else
        {
            selectedLevel = false;
        }
        levelSelectAnimator.SetBool("selectedLevel", selectedLevel);
    }

    public void PlayButton()
    {
        if (selectedLevel)
        {
            switch (levelToSelect)
            {
                case 1:
                    LoadScene("Level1");
                    break;
                case 2:
                    LoadScene("Level2");
                    break;
                case 3:
                    break;
            }
        }
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

    private void CheckMissionTokens()
    {
        missionTokens = 0;
        for(int i = 1; i <= numberOfLevels; i++)
        {
            for(int j = 1; j <= 5; j++)
            {
                if(PlayerPrefs.GetInt("Level" + i + "Mission" + j) == 1)
                {
                    missionTokens++;
                }
            }
        }
        tokenText.text = missionTokens.ToString();
    }

    public void CheckLevelMedals()
    {
        levelMedals = 0;
        for (int i = 1; i <= numberOfLevels; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i + "Mission6") == 1)
            {
                levelMedals++;
            }
        }
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
    Exit,
    Tutorial
}

