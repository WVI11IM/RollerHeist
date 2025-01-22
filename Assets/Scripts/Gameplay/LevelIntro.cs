using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelIntro : MonoBehaviour
{
    public UnityEvent onStartEvent;
    public UnityEvent onTriggerEnterEvent;

    private void Start()
    {
        onStartEvent.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnterEvent.Invoke();
        }
    }
}