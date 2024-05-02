using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{  
   public Slider healthSlider;
   public float maxHealth = 100f;
   public float health;


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
     
   }
    
   public void takeDamage(float damage)
   {
     health -= damage;

     health = Mathf.Clamp(health, 0f, maxHealth);
   }
}
