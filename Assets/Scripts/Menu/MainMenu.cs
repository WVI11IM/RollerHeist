using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Cinemachine;

public class MainMenu : MonoBehaviour
{   
    public MenuState menuState;
    public UnityEvent titleScreenEvent;
    public UnityEvent mainScreenEvent;
    public UnityEvent playScreenEvent;
    public UnityEvent optionsScreenEvent;
    public UnityEvent creditsScreenEvent;
    public UnityEvent controlsScreenEvent;
    public UnityEvent audioScreenEvent;
    public UnityEvent exitScreenEvent;

    UITransitionManager uiTransitionManager;
    public CinemachineVirtualCamera titleCamera, mainCamera;

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
            //titleCamera.Priority++;
        }
        else
        {
            ChangeMenuState("Main");
            //mainCamera.Priority++;
        }

    }

    public void ChangeMenuState(string state)
    {
      switch(state)
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
      
      switch(menuState)
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

        case MenuState.Options:
        optionsScreenEvent.Invoke();
        break;

        case MenuState.Controls:
        controlsScreenEvent.Invoke();
        break;
        
        case MenuState.Audio:
        audioScreenEvent.Invoke();
        break;

        case MenuState.Credits:
        creditsScreenEvent.Invoke();
        break;

        case MenuState.Exit:
        exitScreenEvent.Invoke();
        break;
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
  Options,
  Controls,
  Audio,
  Skins,
  Credits,
  Exit
}

