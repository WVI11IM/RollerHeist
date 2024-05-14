using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXScript : MonoBehaviour
{
    public void PlaySFX(string soundEffectName)
    {
        SFXManager.Instance.PlaySFX(soundEffectName);
    }
}
