using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool canFollow = true;

    public bool canSpawn = false;
    [SerializeField] private List<GameObject> enemySpawners;
    public float spawnCoolDown = 5;

    public static EnemyManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        canSpawn = false;
        //enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
    }

    private void Update()
    {
        if (canSpawn)
        {
            StartCoroutine(EnemySpawner());
        }
    }

    IEnumerator EnemySpawner()
    {
        canSpawn = false;

        int i = Random.Range(0, enemySpawners.Count);

        Instantiate(Resources.Load<GameObject>("ENEMY"), enemySpawners[i].transform);

        yield return new WaitForSeconds(spawnCoolDown);
        canSpawn = true;
    }

}
