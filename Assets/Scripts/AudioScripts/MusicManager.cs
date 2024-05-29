using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicObject;
    [SerializeField] Music[] musics;

    private Dictionary<string, AudioSource> activeLoopAudioSources = new Dictionary<string, AudioSource>();

    public static MusicManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayMusicLoop(string musicName)
    {
        if (activeLoopAudioSources.ContainsKey(musicName) && activeLoopAudioSources[musicName].isPlaying)
        {
            return; // If the looped audio source for this sound effect is already playing, do nothing.
        }

        Music music = Array.Find(musics, e => e.musicName == musicName);
        if (music != null)
        {
            AudioSource audioSource = Instantiate(musicObject, transform.position, Quaternion.identity);
            audioSource.clip = music.soundClip;
            audioSource.volume = music.volumeModifier;
            audioSource.loop = true;
            audioSource.Play();

            activeLoopAudioSources[musicName] = audioSource;
        }
        else
        {
            Debug.LogWarning("Music not found: " + musicName);
        }
    }

    public void StopMusicLoop(string soundEffectName)
    {
        if (activeLoopAudioSources.ContainsKey(soundEffectName))
        {
            AudioSource audioSource = activeLoopAudioSources[soundEffectName];
            audioSource.loop = false;
            audioSource.Stop();
            Destroy(audioSource.gameObject);
            activeLoopAudioSources.Remove(soundEffectName);
        }
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
