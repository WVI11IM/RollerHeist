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
    public Animator animator;

    void Start()
    {
        health = maxHealth;
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
        healthMeter.fillAmount = Mathf.Lerp(0.39f, 0.61f, healthSliderValue);
         /*
        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }
         */
        
        if (health <= 0 && !isDead)
        {
            isDead = true;
            gameObject.SetActive(false);
            gameManager.gameOver();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible) // Só aplica o dano se não estiver invencível
        {
            animator.SetTrigger("flinched");
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    } 
}
