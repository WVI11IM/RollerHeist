using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /*
    [SerializeField] private bool isAdded = true;

    private void Start()
    {
        isAdded = true;
    }

    void Update()
    {
        if (Physics.CheckSphere(gameObject.transform.position, EnemyManager.Instance.spawnerRangeDetectPlayer, EnemyManager.Instance.spawnerDetectPlayer))
        {
            if (isAdded)
            {
                EnemyManager.Instance.enemySpawners.Remove(gameObject);
                isAdded = false;
                Debug.Log("Remove from list");
            }
        }
        else
        {
            if (!isAdded)
            {
                EnemyManager.Instance.enemySpawners.Add(gameObject);
                isAdded = true;
                Debug.Log("Add from list");
            }
        }
    }
    */

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

    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyManager.Instance.spawnerRangeDetectPlayer);
    }
    */
}
