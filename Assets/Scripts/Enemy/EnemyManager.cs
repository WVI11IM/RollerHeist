using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool canFollow = true;
    public bool canSpawn = true;

    public List<GameObject> enemySpawners;

    public float maxEnemySpawn = 25;
    public float currentEnemySpawned = 0;

    [Header("Spawn's Frequency")]
    [Tooltip("This will be changed in script, no need to attribute a value :D")]
    public float currentSpawnCooldown = 0;
    public float startSpawnCooldown = 10;
    public float oneItemSpawnCooldown = 8;
    public float twoItemSpawnCooldown = 7;
    public float threeItemSpawnCooldown = 6;
    public float mainItemSpawnCooldown = 5;

    //private List<GameObject> enemiesOnScene;

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
        canSpawn = true;
        //enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
    }

    private void Update()
    {
        //enemiesOnScene = GameObject.FindGameObjectsWithTag("Enemy");

        ChangeSpawnFrequency();

        if (canSpawn && currentEnemySpawned < maxEnemySpawn)
        {
            StartCoroutine(EnemySpawner());
        }
    }

    IEnumerator EnemySpawner()
    {
        currentEnemySpawned++;
        canSpawn = false;

        int i = Random.Range(0, enemySpawners.Count);

        Instantiate(Resources.Load<GameObject>("ENEMY"), enemySpawners[i].transform);

        yield return new WaitForSeconds(currentSpawnCooldown);
        canSpawn = true;
    }

    private void ChangeSpawnFrequency()
    {
        if (CollectableManager.instance.bigItemCollected == 1)
        {
            currentSpawnCooldown = mainItemSpawnCooldown;
        }
        else if (CollectableManager.instance.bigItemCollected == 0)
        {
            switch (CollectableManager.instance.smallItensCollected)
            {
                case 0:
                    currentSpawnCooldown = startSpawnCooldown;
                    break;
                case 1:
                    currentSpawnCooldown = oneItemSpawnCooldown;
                    break;
                case 2:
                    currentSpawnCooldown = twoItemSpawnCooldown;
                    break;
                case 3:
                    currentSpawnCooldown = threeItemSpawnCooldown;
                    break;
            }
        }
    }
}
