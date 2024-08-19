using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialColliderTrigger : MonoBehaviour
{
    BoxCollider triggerCollider;
    [SerializeField] TutorialManager tutorialManager;

    private void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
    }
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.TutorialNextStep();
        }
    }
}
