using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicObjectPrefab;
    [SerializeField] private Music[] musics;

    private Dictionary<string, AudioSource> activeLoopAudioSources = new Dictionary<string, AudioSource>();

    public static MusicManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusicLoop(string musicName)
    {
        if (activeLoopAudioSources.ContainsKey(musicName) && activeLoopAudioSources[musicName].isPlaying)
        {
            return; // If the music is already playing, do nothing.
        }

        Music music = Array.Find(musics, e => e.musicName == musicName);
        if (music != null)
        {
            // Check if the music is already playing on another AudioSource
            foreach (var aS in activeLoopAudioSources.Values)
            {
                if (aS.clip == music.soundClip && aS.isPlaying)
                {
                    activeLoopAudioSources[musicName] = aS;
                    return; // If the music is already playing, do nothing.
                }
            }

            AudioSource audioSource = Instantiate(musicObjectPrefab, transform.position, Quaternion.identity);
            audioSource.clip = music.soundClip;
            audioSource.volume = music.volumeModifier;
            audioSource.loop = true;
            audioSource.ignoreListenerPause = true; // Ensure the audio continues playing when time scale is 0
            audioSource.Play();

            // Ensure the AudioSource persists between scenes
            DontDestroyOnLoad(audioSource.gameObject);

            activeLoopAudioSources[musicName] = audioSource;
        }
        else
        {
            Debug.LogWarning("Music not found: " + musicName);
        }
    }

    public void StopMusicLoop(string musicName)
    {
        if (activeLoopAudioSources.ContainsKey(musicName))
        {
            AudioSource audioSource = activeLoopAudioSources[musicName];
            audioSource.loop = false;
            audioSource.Stop();
            Destroy(audioSource.gameObject);
            activeLoopAudioSources.Remove(musicName);
        }
    }

    // Optional: Stop all looping music (useful when loading a new scene)
    public void StopAllLoopingMusic()
    {
        foreach (var audioSource in activeLoopAudioSources.Values)
        {
            audioSource.loop = false;
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
        activeLoopAudioSources.Clear();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Optionally handle anything specific when a scene loads
        // Example: Stop all music when a new scene loads
        StopAllLoopingMusic();
    }
}

[System.Serializable]
public class Music
{
    public string musicName;
    public AudioClip soundClip;
    [Range(0.0f, 1.0f)]
    public float volumeModifier = 1f;
}