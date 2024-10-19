using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCollisionSound : MonoBehaviour
{
    public string[] soundName;
    public float soundCooldown = 1f;
    private bool canPlaySound = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canPlaySound)
        {
            if (soundName.Length == 1)
            {
                SFXManager.Instance.PlaySFXRandomPitch(soundName[0]);
            }
            else
            {
                int randomNumber = Random.Range(0, soundName.Length);
                SFXManager.Instance.PlaySFXRandomPitch(soundName[randomNumber]);
            }

            StartCoroutine(SoundCooldownCoroutine());
        }
    }

    private IEnumerator SoundCooldownCoroutine()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(soundCooldown);
        canPlaySound = true;
    }
}
