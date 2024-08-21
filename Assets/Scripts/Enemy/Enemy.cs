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
    [HideInInspector] public MovementTest2 playerMovement;
    [HideInInspector] public float health;
    [SerializeField] GameObject batton;
    [SerializeField] GameObject taser;
    public bool isAttacking = false;
    public bool isShooting = false;
    public bool isStunned = false;
    public bool hasFainted = false;
    private bool canDamage = true;

    [SerializeField] Animator animator;
    [SerializeField] SkinnedMeshRenderer[] sMR;
    [SerializeField] Material hurtMaterial;
    Material currentMaterial;

    public enum State
    {
        PATROL,
        CHASE,
        ATTACK,
        STUNNED,
        FAINTED
    }

    public State enemyState;

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
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementTest2>();
        health = enemyType.health;

        enemyState = State.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.speed == 0f && enemyState == State.ATTACK)
        {
            RotateTowardsTarget();
        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isShooting", isShooting);
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
        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);

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
        enemy.updateRotation = true;
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
        isShooting = false;
        if (enemyType.enemyName == "Guard")
        {
            batton.SetActive(true);
            taser.SetActive(false);
        }
        else if (enemyType.enemyName == "Taser")
        {
            batton.SetActive(false);
            taser.SetActive(true);
        }

        //Perseguir jogador com velocidade alta
        enemy.isStopped = false;

        if (!playerMovement.isGrounded)
        {
            enemy.SetDestination(player.position);
        }
        else
        {
            enemy.SetDestination(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        enemy.speed = enemyType.runSpeed;
        enemy.updateRotation = true;
    }
    public void AttackPlayer()
    {
        isAttacking = true;
        enemy.speed = 0;
        enemy.updateRotation = false;

        if (enemyType.enemyName == "Guard")
        {
            GiveDamage();
            isShooting = false;
            batton.SetActive(true);
            taser.SetActive(false);
        }
        else if (enemyType.enemyName == "Taser")
        {
            isShooting = true;
            batton.SetActive(false);
            taser.SetActive(true);
        }
    }
    public void Stun()
    {
        isAttacking = false;
        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);
        enemy.isStopped = true;
        if (!isStunned)
        {
            ObjectiveManager.Instance.hasGivenDamage = true;
            StartCoroutine(Stunned());
        }
        //Paralisar
    }
    public void Faint()
    {
        isAttacking = false;
        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);
        enemy.isStopped = true;
        if (!hasFainted)
        {
            ObjectiveManager.Instance.EnemyDefeated();
            animator.SetTrigger("hasFainted");
            hasFainted = true;
            canDamage = false;
            StartCoroutine(FaintRed());
            EnemyManager.Instance.currentEnemySpawned--;
            Destroy(gameObject, 5);
        }
    }

    /*
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
    */

    public float damage = 10f;

    private void OnTriggerEnter(Collider collider)
    {
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
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto");
        isStunned = true;
        canDamage = false;
        for(int i = 0; i < sMR.Length; i++)
        {
            currentMaterial = sMR[i].material;
            sMR[i].material = hurtMaterial;
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < sMR.Length; i++)
        {
            sMR[i].material = currentMaterial;
        }
        yield return new WaitForSeconds(0.35f);
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
        if (playerHealth != null && !playerHealth.isInvincible && !playerHealth.isDead && canDamage && !isStunned)
        {
            playerHealth.TakeDamage(damage);
            if (enemyType.enemyName == "Guard")
            {
                SFXManager.Instance.PlaySFXRandomPitch("cassetete");
            }
            else if (enemyType.enemyName == "Taser")
            {
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Keep the rotation on the horizontal plane
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * enemy.angularSpeed / 2);
        }
    }

    IEnumerator FaintRed()
    {
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto");
        SFXManager.Instance.PlaySFXRandomPitch("guardaDerrotado");
        for (int i = 0; i < sMR.Length; i++)
        {
            currentMaterial = sMR[i].material;
            sMR[i].material = hurtMaterial;
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < sMR.Length; i++)
        {
            sMR[i].material = currentMaterial;
        }
    }
}