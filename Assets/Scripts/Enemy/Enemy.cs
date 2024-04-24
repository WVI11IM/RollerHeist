using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyType;
    public NavMeshAgent enemy;
    public Transform player;

    private enum State
    {
        PATROL,
        LOOK,
        CHASE,
        ATTACK,
        STUNNED
    }

    private State enemyState;

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        if(dist > enemyType.detectionDist)
        {
            enemyState = State.LOOK;
        }

        switch (enemyState)
        {
            case State.PATROL:
                break;
            case State.LOOK:
                break;
            case State.CHASE:
                FollowPlayer();
                break;
            case State.ATTACK:
                break;
            case State.STUNNED:
                break;

        }
    }

    public void FollowPlayer()
    {
        if (EnemyManager.Instance.canFollow == true)
        {
            enemy.SetDestination(player.position);
        }
    }
}
