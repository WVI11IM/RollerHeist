using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public EnemySO enemyType;
    public NavMeshAgent enemy;
    [HideInInspector] public Transform player;
    public float health;

    private enum State
    {
        PATROL,
        CHASE,
        ATTACK,
        STUNNED,
        FAINTED
    }

    private State enemyState;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = enemyType.health;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (health <= 0)
        {
            enemyState = State.FAINTED;
        }
        else
        {
            if (enemyState == State.CHASE)
            {
                if (dist > enemyType.chaseDist)
                {
                    enemyState = State.PATROL;
                }
                if (dist <= enemyType.conflictDist)
                {
                    enemyState = State.ATTACK;
                }
            }
            else if (dist <= enemyType.detectionDist)
            {
                enemyState = State.CHASE;
            }
            else
            {
                enemyState = State.PATROL;
            }
        }


        switch (enemyState)
        {
            case State.PATROL:
                PatrolArea();
                break;
            case State.CHASE:
                ChasePlayer();
                break;
            case State.ATTACK:
                AttackPlayer();
                break;
            case State.STUNNED:
                Stun();
                break;
            case State.FAINTED:
                Faint();
                break;

        }
    }

    public void PatrolArea()
    {
        //Patrulhar área com velocidade lenta
        enemy.isStopped = false;
        enemy.speed = enemyType.walkSpeed;

    }
    public void ChasePlayer()
    {
        //Perseguir jogador com velocidade alta
        enemy.isStopped = false;
        enemy.SetDestination(player.position);
        enemy.speed = enemyType.runSpeed;
    }
    public void AttackPlayer()
    {
        //Parar e atacar na direção do jogador
        enemy.speed = 0;
    }
    public void Stun()
    {
        //Paralisar
        enemy.isStopped = true;
    }
    public void Faint()
    {
        //Morrer
        enemy.isStopped = true;
    }

    private void OnDrawGizmos()
    {
        float radius = enemyType.detectionDist;
        int segments = 24; // Number of segments to approximate the circle

        // Calculate the center of the circle parallel to the floor
        Vector3 center = transform.position;
        center.y = transform.position.y - (transform.localScale.y / 2); // Adjust the y-coordinate to be at the bottom of the GameObject

        // Draw the circle using Gizmos.DrawLine
        Vector3 previousPoint = Vector3.zero;
        for (int i = 0; i <= 24; i++)
        {
            float angle = (i / (float)segments) * 360.0f;
            Vector3 point = center + new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0.0f, Mathf.Cos(angle * Mathf.Deg2Rad) * radius);
            if (i > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(previousPoint, point);
            }
            previousPoint = point;
        }
    }

    public float damage = 10f;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            HealthBar playerHealth = collider.gameObject.GetComponent<HealthBar>();
            Debug.Log(playerHealth);
            if (playerHealth != null)
            {
                playerHealth.takeDamage(damage);
            }
        }
        else if (collider.gameObject.CompareTag("Projetil"))
        {
            Destroy(gameObject); // Destroi o inimigo quando entra em contato com o projetil
        }
    }
}