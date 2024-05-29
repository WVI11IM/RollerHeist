using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicScript : MonoBehaviour
{
    public void PlayMusicLoop(string soundEffectName)
    {
        MusicManager.Instance.PlayMusicLoop(soundEffectName);
    }
}
