using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxObject;
    [SerializeField] private AudioSource sfxObjectSpacialBlend;

    private Dictionary<string, AudioSource> activeLoopAudioSources = new Dictionary<string, AudioSource>();

    [SerializeField] SoundEffect[] soundEffects;
    [SerializeField] SoundEffectSpacialBlend[] soundEffectsSpacialBlend;
    [SerializeField] SoundEffectRandomPitch[] soundEffectsRandomPitch;

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
        if (activeLoopAudioSources.ContainsKey(soundEffectName) && activeLoopAudioSources[soundEffectName].isPlaying)
        {
            return; // If the looped audio source for this sound effect is already playing, do nothing.
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
