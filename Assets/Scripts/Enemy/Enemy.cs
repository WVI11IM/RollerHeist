using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [Header("ENEMY PROPERTIES")]
    public EnemySO enemyType;
    public NavMeshAgent enemy;
    [HideInInspector] public Transform player;
    public float health;
    public bool isAttacking = false;
    public bool isStunned = false;
    public bool hasFainted = false;
    private bool canDamage = true;

    [SerializeField] Animator animator;

    private enum State
    {
        PATROL,
        CHASE,
        ATTACK,
        STUNNED,
        FAINTED
    }

    private State enemyState;

    [Space]
    [Header("PATROL SETTINGS")]
    public float patrolRadius;
    public float waitTimeAtPatrolPoint;
    private Vector3 patrolTarget;
    private float waitTimer;
    private bool hasPatrolTarget = false;
    private float stuckTimer = 0f;
    private const float stuckTimeout = 5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = enemyType.health;

        enemyState = State.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isStunned", isStunned);
        animator.SetFloat("speed", enemy.velocity.magnitude / enemyType.runSpeed);

        float dist = Vector3.Distance(player.position, transform.position);

        if (health <= 0)
        {
            enemyState = State.FAINTED;
        }
        else
        {
            switch (enemyState)
            {
                case State.CHASE:
                    if (dist > enemyType.chaseDist)
                    {
                        enemyState = State.PATROL;
                    }
                    if (dist <= enemyType.conflictDist)
                    {
                        if (player.position.y < transform.position.y + 3 && player.position.y > transform.position.y - 3) 
                        {
                            enemyState = State.ATTACK;
                        }
                    }
                    break;
                case State.PATROL:
                    if (dist <= enemyType.detectionDist)
                    {
                        enemyState = State.CHASE;
                    }
                    break;
                case State.ATTACK:
                    if (dist > enemyType.conflictDist)
                    {
                        enemyState = State.CHASE;
                    }
                    break;
                case State.STUNNED:
                    break;
                default:
                    enemyState = State.PATROL;
                    break;
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
        isAttacking = false;

        // Set a random patrol target if not already set or if close to the current target
        if (!hasPatrolTarget || Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            patrolTarget = GetRandomPatrolPoint();
            hasPatrolTarget = true;
            stuckTimer = 0f;  // Reset stuck timer when a new target is set
        }

        // Move towards the random patrol target
        enemy.isStopped = false;
        enemy.speed = enemyType.walkSpeed;
        enemy.SetDestination(patrolTarget);

        // Check if the enemy has reached the patrol target
        if (!enemy.pathPending && enemy.remainingDistance < 0.5f)
        {
            // Wait at the patrol target
            if (waitTimer >= waitTimeAtPatrolPoint)
            {
                patrolTarget = GetRandomPatrolPoint();
                waitTimer = 0f;
            }
            else
            {
                waitTimer += Time.deltaTime;
                enemy.isStopped = true;
            }
        }

        // Check if the enemy is stuck (not moving towards the target)
        if (enemy.velocity.magnitude < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTimeout)
            {
                patrolTarget = GetRandomPatrolPoint();
                stuckTimer = 0f;
                waitTimer = 0f;
                hasPatrolTarget = true;
            }
        }
        else
        {
            stuckTimer = 0f;  // Reset stuck timer if the enemy is moving
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        int maxAttempts = 30;  // Limit the number of attempts to find a valid point

        for (int i = 0; i < maxAttempts; i++)
        {
            randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // If no valid point is found after maxAttempts, return the current position
        return transform.position;
    }
    public void ChasePlayer()
    {
        isAttacking = false;

        //Perseguir jogador com velocidade alta
        enemy.isStopped = false;
        enemy.SetDestination(player.position);
        enemy.speed = enemyType.runSpeed;
    }
    public void AttackPlayer()
    {
        isAttacking = true;
        GiveDamage();
        enemy.speed = 0;
    }
    public void Stun()
    {
        isAttacking = false;
        enemy.isStopped = true;
        if (!isStunned)
        {
            StartCoroutine(Stunned());
        }
        //Paralisar
    }
    public void Faint()
    {
        isAttacking = false;
        enemy.isStopped = true;
        if (!hasFainted)
        {
            animator.SetTrigger("hasFainted");
            hasFainted = true;
            canDamage = false;
            Destroy(gameObject, 5);
        }
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
        /*
        if (collider.gameObject.CompareTag("Player"))
        {
            HealthBar playerHealth = collider.gameObject.GetComponent<HealthBar>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else
        */
        if (collider.gameObject.CompareTag("Projetil"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health -= 1;

        if (health <= 0)
        {
            enemyState = State.FAINTED;
        }
        else
        {
            enemyState = State.STUNNED;
        }
    }

    private IEnumerator Stunned()
    {
        isStunned = true;
        canDamage = false;
        yield return new WaitForSeconds(0.5f);
        isStunned = false;
        if (!hasFainted)
        {
            canDamage = true;
        }

        if (enemyState == State.STUNNED) // Ensure the state is still stunned before changing
        {
            enemyState = State.PATROL;
        }
    }

    public void GiveDamage()
    {
        HealthBar playerHealth = player.GetComponent<HealthBar>();
        if (playerHealth != null && !playerHealth.isInvincible && canDamage && !isStunned)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}