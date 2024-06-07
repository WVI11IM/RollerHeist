using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXScript : MonoBehaviour
{
    public void PlaySFX(string soundEffectName)
    {
        SFXManager.Instance.PlaySFX(soundEffectName);
    }

    public void PlaySFXRandomPitch(string soundEffectName)
    {
        SFXManager.Instance.PlaySFXRandomPitch(soundEffectName);
    }

    public void PlayUISFX(string soundEffectName)
    {
        SFXManager.Instance.PlayUISFX(soundEffectName);
    }
}
