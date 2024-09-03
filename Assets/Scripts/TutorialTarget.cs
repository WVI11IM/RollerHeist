using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projetil"))
        {
            TutorialManager.Instance.tutorialTargetCounter++;
            Destroy(gameObject);
        }
    }
}
