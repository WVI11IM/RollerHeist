using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

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
    public TextMeshProUGUI medalText;
    public GameObject medalInfoButton;
    public TextMeshProUGUI levelTimeRecordText;
    public TextMeshProUGUI museumName;
    public Button titleButtonToSelect;
    public Button mainMenuButtonToSelect;
    public Button tutorialButtonToSelect;
    public Button playButtonToSelect;
    public Button[] levelButtonsToSelect;

    private SceneLoader sceneLoader;

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
            titleButtonToSelect.Select();
            PlayerPrefs.SetInt("titleScreenActivated", 1);
            PlayerPrefs.SetInt("justPlayedLevel", 0);
            PlayerPrefs.SetInt("justPlayedTutorial", 0);
        }
        else
        {
            if(PlayerPrefs.GetInt("justPlayedTutorial") == 1)
            {
                ChangeMenuState("Play");
                playButtonToSelect.Select();
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
                mainMenuButtonToSelect.Select();
                PlayerPrefs.SetInt("justPlayedLevel", 0);
                PlayerPrefs.SetInt("justPlayedTutorial", 0);
            }
        }

        sceneLoader = GetComponent<SceneLoader>();
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
                AudioMixerManager.Instance.ChangeMusicSnapshot("Normal", 0.25f);
                optionsScreenEvent.Invoke();
                break;

            case MenuState.Controls:
                controlsScreenEvent.Invoke();
                menuOptionsAnimator.SetInteger("menuState", 1);
                break;

            case MenuState.Audio:
                AudioMixerManager.Instance.ChangeMusicSnapshot("Radio", 0.25f);
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
            tutorialButtonToSelect.Select();
            SFXManager.Instance.PlayUISFX("woosh2");
        }
        else
        {
            ChangeMenuState("Play");
            playButtonToSelect.Select();
        }
    }

    public void ChangeLevel(int addOrSubtractOne)
    {
        levelToSelect += addOrSubtractOne;
        levelToSelect = Mathf.Clamp(levelToSelect, 1, numberOfLevels + 1);
        if(levelToSelect >= 1 && levelToSelect <= 3)
        {
            SelectLevelButton();
        }
        PlayerPrefs.SetInt("levelToSelect", levelToSelect);

        UpdateHighscoreText();
        UpdateMuseumName();
    }

    public void UpdateHighscoreText()
    {
        if (PlayerPrefs.GetFloat("Level" + levelToSelect + "HighScore", 6039.999f) < 6039.999f)
        {
            int minutes = Mathf.FloorToInt((PlayerPrefs.GetFloat("Level" + levelToSelect + "HighScore", 0)) / 60);
            int seconds = Mathf.FloorToInt((PlayerPrefs.GetFloat("Level" + levelToSelect + "HighScore", 0)) % 60);
            int miliseconds = (int)((PlayerPrefs.GetFloat("Level" + levelToSelect + "HighScore", 0)) * 1000) % 1000;
            switch (PlayerPrefs.GetInt("languageInt"))
            {
                case 0: //Português
                    levelTimeRecordText.text = "Recorde: " + minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
                    break;
                case 1: //Inglês
                    levelTimeRecordText.text = "Best time: " + minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
                    break;
            }
        }
        else
        {
            switch (PlayerPrefs.GetInt("languageInt"))
            {
                case 0: //Português
                    levelTimeRecordText.text = "Recorde: --:--:---";
                    break;
                case 1: //Inglês
                    levelTimeRecordText.text = "Best time: --:--:---";
                    break;
            }
        }
    }

    public void UpdateMuseumName()
    {
        switch (levelToSelect)
        {
            case 1:
                switch (PlayerPrefs.GetInt("languageInt"))
                {
                    case 0: //Português
                        museumName.text = "Museu de História Natural";
                        break;
                    case 1: //Inglês
                        museumName.text = "Natural History Museum";
                        break;
                }
                break;
            case 2:
                switch (PlayerPrefs.GetInt("languageInt"))
                {
                    case 0: //Português
                        museumName.text = "Museu de Cultura Oriental";
                        break;
                    case 1: //Inglês
                        museumName.text = "Museum of Asian Cultures";
                        break;
                }
                break;
            case 3:
                museumName.text = "?????";
                break;
        }
    }

    public void LoadLevelSelection()
    {
        if (PlayerPrefs.GetInt("levelToSelect") == 0)
        {
            PlayerPrefs.SetInt("levelToSelect", 1);
        }
        else
        {
            //levelToSelect = PlayerPrefs.GetInt("levelToSelect");
            //FOR DEMO, BRING PLAYERS BACK TO LEVEL 1 SELECTION
            //
            levelToSelect = 1;
            PlayerPrefs.SetInt("levelToSelect", 1);
            //
            SelectLevelButton();
        }

        UpdateHighscoreText();
        UpdateMuseumName();
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


    public void SelectLevelButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SelectLevelButtonAfterTime());
    }

    private IEnumerator SelectLevelButtonAfterTime()
    {
        yield return new WaitForSeconds(0.2f);
        levelButtonsToSelect[levelToSelect - 1].Select();
    }

    public void PlayButton()
    {
        if (selectedLevel)
        {
            switch (levelToSelect)
            {
                case 1:
                    sceneLoader.LoadScene("Level1-NewLevelTest");
                    break;
                case 2:
                    sceneLoader.LoadScene("Level2-NewLevelTest");
                    break;
                case 3:
                    break;
            }
        }
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
        medalText.text = levelMedals + " / " + numberOfLevels;
        if(levelMedals >= 2)
        {
            medalInfoButton.SetActive(false);
        }
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        LoadScene("Intro");

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

