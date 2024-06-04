using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public static AudioMixerManager Instance;

    private const string MasterVolumeKey = "MasterVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicVolumeKey = "MusicVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadVolumes();
        UpdateSliders();
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f); // Avoid log(0)
        PlayerPrefs.SetFloat(MasterVolumeKey, level);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f); // Avoid log(0)
        PlayerPrefs.SetFloat(SFXVolumeKey, level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f); // Avoid log(0)
        PlayerPrefs.SetFloat(MusicVolumeKey, level);
    }

    private void LoadVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        SetMasterVolume(masterVolume);
        SetSFXVolume(sfxVolume);
        SetMusicVolume(musicVolume);
    }

    public void UpdateSliders()
    {
        Slider masterVolumeSlider = GameObject.Find("MasterVolumeSlider")?.GetComponent<Slider>();
        Slider sfxVolumeSlider = GameObject.Find("SFXVolumeSlider")?.GetComponent<Slider>();
        Slider musicVolumeSlider = GameObject.Find("MusicVolumeSlider")?.GetComponent<Slider>();

        if (masterVolumeSlider != null) masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    }

    public void OnMasterVolumeChanged(float level)
    {
        SetMasterVolume(level);
    }

    public void OnSFXVolumeChanged(float level)
    {
        SetSFXVolume(level);
    }

    public void OnMusicVolumeChanged(float level)
    {
        SetMusicVolume(level);
    }
}