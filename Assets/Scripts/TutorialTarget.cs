using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    public GameObject targetParticles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projetil"))
        {
            int randomValue = Random.Range(1, 3);
            SFXManager.Instance.PlaySFXRandomPitch("alvo"+randomValue);
            TutorialManager.Instance.tutorialTargetCounter++;
            Instantiate(targetParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
