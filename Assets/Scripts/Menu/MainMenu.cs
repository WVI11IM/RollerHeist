using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    public UnityEvent titleScreenEvent;
    public UnityEvent menuScreenEvent;


   


    private void Start()
    {
        AudioMixerManager.Instance.LoadVolumes();
        AudioMixerManager.Instance.UpdateSliders();
        MusicManager.Instance.PlayMusicLoop("beanbag");
        if (PlayerPrefs.GetInt("titleScreenActivated") == 0)
        {
            titleScreenEvent.Invoke();
            PlayerPrefs.SetInt("titleScreenActivated", 1);
        }
        else
        {
            menuScreenEvent.Invoke();
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
