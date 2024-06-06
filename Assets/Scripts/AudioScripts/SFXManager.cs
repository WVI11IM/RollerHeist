using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxObject;

    private Dictionary<string, AudioSource> activeLoopAudioSources = new Dictionary<string, AudioSource>();

    [SerializeField] SoundEffect[] soundEffects;
    [SerializeField] SoundEffectSpacialBlend[] soundEffectsSpacialBlend;
    [SerializeField] SoundEffectRandomPitch[] soundEffectsRandomPitch;
    [SerializeField] SoundEffect[] uiSoundEffects;

    public static SFXManager Instance;

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
        }
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
        StopAllLoopingSFX();
    }

    public void PlaySFX(string soundEffectName)
    {
        SoundEffect effect = Array.Find(soundEffects, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            Debug.LogWarning("Sound effect not found: " + soundEffectName);
        }
    }

    public void PlaySFXSpacialBlend(string soundEffectName, Transform spawnTransform)
    {
        SoundEffectSpacialBlend effect = Array.Find(soundEffectsSpacialBlend, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = effect.minDist;
            audioSource.maxDistance = effect.maxDist;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            Debug.LogWarning("Sound effect not found: " + soundEffectName);
        }
    }

    public void PlaySFXRandomPitch(string soundEffectName)
    {
        SoundEffectRandomPitch effect = Array.Find(soundEffectsRandomPitch, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            float randomPitch = UnityEngine.Random.Range(effect.minPitch, effect.maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            Debug.LogWarning("Sound effect not found: " + soundEffectName);
        }
    }

    public void PlaySFXLoop(string soundEffectName)
    {
        if (activeLoopAudioSources.ContainsKey(soundEffectName))
        {
            return;
        }

        SoundEffect effect = Array.Find(soundEffects, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            audioSource.loop = true;
            audioSource.time = UnityEngine.Random.Range(0f, audioSource.clip.length);
            audioSource.Play();

            activeLoopAudioSources[soundEffectName] = audioSource;
        }
        else
        {
            Debug.LogWarning("Sound effect not found: " + soundEffectName);
        }
    }

    public void StopSFXLoop(string soundEffectName)
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

    public void StopAllLoopingSFX()
    {
        foreach (var entry in activeLoopAudioSources)
        {
            AudioSource audioSource = entry.Value;
            audioSource.loop = false;
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
        activeLoopAudioSources.Clear();
    }

    public void PauseLoopingSFX()
    {
        foreach (var entry in activeLoopAudioSources)
        {
            entry.Value.Pause();
        }
    }

    public void ResumeLoopingSFX()
    {
        foreach (var entry in activeLoopAudioSources)
        {
            entry.Value.UnPause();
        }
    }

    public void PlayUISFX(string soundEffectName)
    {
        SoundEffect effect = Array.Find(uiSoundEffects, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            audioSource.ignoreListenerPause = true; // Ensure the sound continues playing when time scale is 0
            audioSource.Play();
            DontDestroyOnLoad(audioSource.gameObject);
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            Debug.LogWarning("UI Sound effect not found: " + soundEffectName);
        }
    }
}

[System.Serializable]
public class SoundEffect
{
    public string soundEffectName;
    public AudioClip soundClip;
    [Range(0.0f, 1.0f)]
    public float volumeModifier = 1f;
}

[System.Serializable]
public class SoundEffectSpacialBlend
{
    public string soundEffectName;
    public AudioClip soundClip;
    [Range(0.0f, 1.0f)]
    public float volumeModifier = 1f;
    public float minDist = 0.5f;
    public float maxDist = 5f;
}
[System.Serializable]
public class SoundEffectRandomPitch
{
    public string soundEffectName;
    public AudioClip soundClip;
    [Range(0.0f, 1.0f)]
    public float volumeModifier = 1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
}
