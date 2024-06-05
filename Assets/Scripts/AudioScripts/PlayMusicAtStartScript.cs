using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicAtStartScript : MonoBehaviour
{
    public string musicName;

    private void Start()
    {
        AudioMixerManager.Instance.LoadVolumes();
        MusicManager.Instance.PlayMusicLoop(musicName);
    }
}
