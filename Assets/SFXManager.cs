using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxObject;
    [SerializeField] private AudioSource sfxObjectSpacialBlend;

    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    [SerializeField] SoundEffect[] soundEffects;
    [SerializeField] SoundEffectSpacialBlend[] soundEffectsSpacialBlend;
    
    public static SFXManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
        SoundEffect effect = Array.Find(soundEffects, e => e.soundEffectName == soundEffectName);
        if (effect != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);
            audioSource.clip = effect.soundClip;
            audioSource.volume = effect.volumeModifier;
            audioSource.loop = true;
            audioSource.time = UnityEngine.Random.Range(0f, audioSource.clip.length);
            audioSource.Play();

            activeAudioSources.Add(audioSource);
        }
        else
        {
            Debug.LogWarning("Sound effect not found: " + soundEffectName);
        }
    }

    public void StopSFXLoop(string soundEffectName)
    {
        for (int i = activeAudioSources.Count - 1; i >= 0; i--)
        {
            AudioSource audioSource = activeAudioSources[i];
            if (audioSource.clip.name == soundEffectName && audioSource.isPlaying)
            {
                audioSource.loop = false;
                audioSource.Stop();
                Destroy(audioSource.gameObject);
                activeAudioSources.RemoveAt(i);
            }
        }
    }
}

[System.Serializable]
public class SoundEffect
{
    public string soundEffectName;
    public AudioClip soundClip;
    public float volumeModifier = 1f;
}

[System.Serializable]
public class SoundEffectSpacialBlend
{
    public string soundEffectName;
    public AudioClip soundClip;
    public float volumeModifier = 1f;
    public float minDist = 0.5f;
    public float maxDist = 5f;
}
