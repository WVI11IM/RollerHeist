using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        //enemy.SetDestination(player.position);
        FollowPlayer();
    }

    public void FollowPlayer()
    {
        if (EnemyManager.Instance.canFollow == true)
        {
            enemy.SetDestination(player.position);
        }
    }
}
