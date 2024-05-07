using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float lifetime = 3f; 

    void Start()
    {
        
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aaa");


        if (!other.gameObject.CompareTag("Player"))
        {

            Destroy(gameObject);
        }
    }
}
