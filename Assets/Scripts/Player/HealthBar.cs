using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{  
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float health;
    public float invincibilityDuration = 2f; // Duração da invencibilidade em segundos

    private bool isDead;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    public GameManager gameManager; 

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }

        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }
        
        if (health <= 0 && !isDead)
        {
            isDead = true;
            gameObject.SetActive(false);
            gameManager.gameOver();
        }
    }

    public void takeDamage(float damage)
    {
        if (!isInvincible) // Só aplica o dano se não estiver invencível
        {
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    } 
}
