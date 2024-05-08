using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{  
   public Slider healthSlider;
   public float maxHealth = 100f;
   public float health;

   private bool isDead;

   public GameManager gameManager; 

   void Start()
   {
     health = maxHealth;
   }

   void Update()
   {
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
     health -= damage;
     health = Mathf.Clamp(health, 0f, maxHealth);
   }

  
}
