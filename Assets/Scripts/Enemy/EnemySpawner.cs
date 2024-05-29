using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyManager.Instance.enemySpawners.Remove(gameObject);
            //isAdded = false;
            Debug.Log("Remove from list");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyManager.Instance.enemySpawners.Add(gameObject);
            //isAdded = true;
            Debug.Log("Add from list");
        }
    }
}
