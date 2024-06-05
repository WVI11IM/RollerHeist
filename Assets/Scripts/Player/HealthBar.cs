using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{  
    public Image healthMeter;
    public float maxHealth = 100f;
    public float health;
    public float invincibilityDuration = 2f; // Duração da invencibilidade em segundos

    private bool isDead;
    public bool isInvincible = false;
    private float invincibilityTimer = 0f;

    public GameManager gameManager;
    MovementTest2 playerMovement;
    PaintballShoot playerShoot;
    public Animator animator;

    public SkinnedMeshRenderer sMR;
    public Material hurtMaterial;
    public Material invincibleMaterial;
    Material currentSkin;


    void Start()
    {
        health = maxHealth;
        playerMovement = GetComponent<MovementTest2>();
        playerShoot = GetComponent<PaintballShoot>();
        StartCoroutine(LateStart(0.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentSkin = sMR.material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            maxHealth = 10000;
            health = maxHealth;
        }

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }

        float healthSliderValue = health / maxHealth;
        healthMeter.fillAmount = Mathf.Lerp(0.4f, 0.6f, healthSliderValue);
        
        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("isFainted", true);
            animator.SetTrigger("fainted");
            playerMovement.enabled = false;
            playerShoot.enabled = false;
            animator.SetLayerWeight(1, 0);
            gameManager.UpdateGameState(GameState.Lose);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible && !isDead) // Só aplica o dano se não estiver invencível
        {
            animator.SetTrigger("flinched");
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);
            if (health > 0) StartCoroutine(HurtSkinChange());
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    IEnumerator HurtSkinChange()
    {
        Debug.Log("isWorking");
        sMR.material = hurtMaterial;
        yield return new WaitForSeconds(invincibilityDuration / 8);
        sMR.material = invincibleMaterial;
        yield return new WaitForSeconds(invincibilityDuration / 8);

        float firstPhaseDuration = invincibilityDuration / 2;
        float secondPhaseDuration = invincibilityDuration / 2;

        for (float t = 0; t < firstPhaseDuration; t += invincibilityDuration / 8)
        {
            sMR.material = (sMR.material == currentSkin) ? invincibleMaterial : currentSkin;
            yield return new WaitForSeconds(invincibilityDuration / 8);
        }

        for (float t = 0; t < secondPhaseDuration; t += invincibilityDuration / 16)
        {
            sMR.material = (sMR.material == currentSkin) ? invincibleMaterial : currentSkin;
            yield return new WaitForSeconds(invincibilityDuration / 16);
        }

        sMR.material = currentSkin;
    }
}
