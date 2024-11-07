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
    [HideInInspector] public HealthBar playerHealth;
    [HideInInspector] public FloorLevelFeedback playerFloorLevelFeedback;
    [HideInInspector] public float health;
    [SerializeField] GameObject batton;
    [SerializeField] GameObject taser;
    [SerializeField] GameObject segway;
    [SerializeField] GameObject segwayAfterFaint;
    public bool isAttacking = false;
    public bool isShooting = false;
    public bool isStunned = false;
    public bool isDefending = false;
    public bool isBlocking = false;
    public bool hasShieldBashed = false;
    public bool hasFainted = false;
    private bool canDamage = true;
    public float damage = 10f;
    public float lookAngle = 0.5f;

    [SerializeField] Animator animator;
    [SerializeField] SkinnedMeshRenderer[] sMR;
    [SerializeField] Material hurtMaterial;
    Material currentMaterial;

    public LayerMask obstacleLayer;

    public enum State
    {
        PATROL,
        CHASE,
        ATTACK,
        DEFEND,
        STUNNED,
        BLOCK,
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
        enemy.avoidancePriority = Random.Range(25, 100);

        switch (enemyType.enemyName)
        {
            case "Guard":
                animator.SetInteger("enemyType", 0);
                break;
            case "Taser":
                animator.SetInteger("enemyType", 1);
                break;
            case "Mallcop":
                animator.SetInteger("enemyType", 2);
                break;
            case "Shield":
                animator.SetInteger("enemyType", 3);
                break;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementTest2>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthBar>();
        playerFloorLevelFeedback = GameObject.FindGameObjectWithTag("Player").GetComponent<FloorLevelFeedback>();
        health = enemyType.health;

        enemyState = State.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.speed == 0f && (enemyState == State.ATTACK || (enemyType.enemyName == "Shield" && enemyState == State.DEFEND)))
        {
            RotateTowardsTarget();
        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDefending", isDefending);
        animator.SetBool("isShooting", isShooting);
        animator.SetBool("isStunned", isStunned);
        animator.SetBool("isBlocking", isBlocking);
        animator.SetFloat("speed", enemy.velocity.magnitude / enemyType.runSpeed);

        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;

        float angle = Vector3.SignedAngle(forward, directionToPlayer, Vector3.up);

        float clampedAngle = Mathf.Clamp(angle, -45f, 45f);
        lookAngle = (clampedAngle + 45f) / 90f;

        animator.SetFloat("lookAngle", lookAngle);

        float dist = Vector3.Distance(player.position, transform.position);

        if (health <= 0)
        {
            enemyState = State.FAINTED;
        }
        else
        {
            if(enemyState != State.STUNNED)
            {
                switch (enemyState)
                {
                    case State.CHASE:
                        if (dist > enemyType.chaseDist)
                        {
                            enemyState = State.PATROL;
                        }
                        else if (dist <= enemyType.conflictDist)
                        {
                            if (player.position.y < transform.position.y + 3 && player.position.y > transform.position.y - 3)
                            {
                                if(enemyType.enemyName == "Taser")
                                {
                                    if (CanSeePlayer())
                                    {
                                        enemyState = State.ATTACK;
                                    }
                                }
                                else enemyState = State.ATTACK;
                            }
                        }
                        if (enemyType.enemyName == "Shield")
                        {
                            if (dist <= enemyType.detectionDist / 3) enemyState = State.DEFEND;
                        }
                        break;
                    case State.PATROL:
                        if (dist <= enemyType.detectionDist)
                        {
                            if (enemyType.enemyName == "Shield" && (dist <= enemyType.detectionDist / 3))
                            {
                                enemyState = State.DEFEND;
                            }
                            else
                            {
                                enemyState = State.CHASE;
                            }
                        }
                        break;
                    case State.ATTACK:
                        if (dist > enemyType.conflictDist)
                        {
                            if (enemyType.enemyName == "Shield" && (dist <= enemyType.detectionDist / 3))
                            {
                                enemyState = State.DEFEND;
                            }
                            else
                            {
                                enemyState = State.CHASE;
                            }
                        }
                        else if (enemyType.enemyName == "Taser")
                        {
                            if (!CanSeePlayer())
                            {
                                enemyState = State.CHASE;
                            }
                        }
                        break;
                    case State.DEFEND:
                        if (dist <= enemyType.conflictDist)
                        {
                            if (player.position.y < transform.position.y + 3 && player.position.y > transform.position.y - 3)
                            {
                                enemyState = State.ATTACK;
                            }

                        }
                        if (dist > enemyType.detectionDist / 3)
                        {
                            enemyState = State.CHASE;
                        }
                        break;
                    default:
                        enemyState = State.PATROL;
                        break;
                }
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
            case State.DEFEND:
                Defend();
                break;
            case State.STUNNED:
                Stun();
                break;
            case State.BLOCK:
                Block();
                break;
            case State.FAINTED:
                Faint();
                break;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 adjustedPlayerPosition = player.position + new Vector3(0, 2, 0);
        Vector3 directionToPlayer = (adjustedPlayerPosition - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, obstacleLayer))
        {
            return false;
        }
        return true;
    }

    public void PatrolArea()
    {
        animator.SetLayerWeight(1, 0);

        isAttacking = false;
        isDefending = false;
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
        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }
    public void ChasePlayer()
    {
        animator.SetLayerWeight(1, 1);

        isAttacking = false;
        isDefending = false;
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
        else if(enemyType.enemyName == "Mallcop")
        {
            batton.SetActive(false);
            taser.SetActive(false);
        }
        else if (enemyType.enemyName == "Shield")
        {
            batton.SetActive(false);
            taser.SetActive(false);
        }

        //Perseguir jogador com velocidade alta
        enemy.isStopped = false;

        if (playerFloorLevelFeedback.distanceToFloor < 5f)
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
        animator.SetLayerWeight(1, 1);

        isAttacking = true;
        isDefending = false;
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
        if (enemyType.enemyName == "Mallcop")
        {
            GiveDamage();
            isShooting = false;
            batton.SetActive(false);
            taser.SetActive(false);
        }
        if (enemyType.enemyName == "Shield")
        {
            Vector3 direction3D = (player.transform.position - transform.position).normalized;
            playerMovement.ShieldPush(new Vector2(direction3D.x, direction3D.z));
            if (!playerHealth.isInvincible && !hasShieldBashed)
            {
                animator.SetTrigger("shieldBashed");
                hasShieldBashed = true;
                GiveDamage();
            }
            isShooting = false;
            batton.SetActive(false);
            taser.SetActive(false);

            StartCoroutine(ResetShieldBash());
        }
    }

    public void Defend()
    {
        animator.SetLayerWeight(1, 1);

        isAttacking = false;
        isDefending = true;
        enemy.speed = 0f;
        enemy.updateRotation = false;
        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);
    }

    public void Stun()
    {
        isAttacking = false;
        isDefending = false;
        isShooting = false;
        enemy.updateRotation = false;
        batton.SetActive(false);
        taser.SetActive(false);
        enemy.isStopped = true;
        if (!isStunned)
        {
            ObjectiveManager.Instance.hasGivenDamage = true;
            StartCoroutine(Stunned());
        }
    }

    public void Block()
    {
        animator.SetLayerWeight(1, 1);

        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);
        enemy.isStopped = true;
        if (!isBlocking)
        {
            StartCoroutine(Blocked());
        }
    }
    public void Faint()
    {
        animator.SetLayerWeight(1, 0);

        isAttacking = false;
        isDefending = false;
        isShooting = false;
        batton.SetActive(false);
        taser.SetActive(false);
        enemy.isStopped = true;
        if (enemyType.enemyName == "Mallcop")
        {
            segway.SetActive(false);
            Rigidbody rb = segwayAfterFaint.GetComponent<Rigidbody>();
            segwayAfterFaint.SetActive(true);
            rb.AddForce(segwayAfterFaint.transform.up * -1f);
        }
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Projetil"))
        {
            if (!hasFainted)
            {
                Rigidbody bulletRb = collider.gameObject.GetComponent<Rigidbody>();
                Vector3 bulletDirection = bulletRb.velocity.normalized;
                Vector3 enemyForward = transform.forward;
                float angle = Vector3.Angle(enemyForward, -bulletDirection);
                Projetil projetil = collider.gameObject.GetComponent<Projetil>();
                projetil.ParticlesAndDestroy();
                if (enemyType.enemyName == "Shield")
                {
                    if (angle <= 70f)
                    {
                        Block();
                    }
                    else
                    {
                        TakeDamage();
                    }
                }
                else
                {
                    TakeDamage();
                }
            }
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
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto1");
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto2");
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

    private IEnumerator Blocked()
    {
        SFXManager.Instance.PlaySFXRandomPitch("");
        isBlocking = true;
        yield return new WaitForSeconds(0.25f);
        isBlocking = false;
        if (!hasFainted)
        {
            canDamage = true;
        }
        if (enemyState == State.STUNNED)
        {
            enemyState = State.PATROL;
        }
    }

    private IEnumerator ResetShieldBash()
    {
        yield return new WaitUntil(() => !playerHealth.isInvincible);
        animator.ResetTrigger("shieldBashed");
        hasShieldBashed = false;
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
            else if (enemyType.enemyName == "Mallcop")
            {
                SFXManager.Instance.PlaySFXRandomPitch("cassetete");
            }
            else if (enemyType.enemyName == "Shield")
            {
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            if(enemyType.enemyName == "Shield" && enemyState == State.DEFEND)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 0.8f);
            }
            else transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * enemy.angularSpeed / 2);
        }
    }

    IEnumerator FaintRed()
    {
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto1");
        SFXManager.Instance.PlaySFXRandomPitch("tiroAcerto2");
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