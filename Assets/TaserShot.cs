using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaserShot : MonoBehaviour
{
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colidir com algo que não seja o jogador
        if (!other.gameObject.CompareTag("Enemy") && !other.CompareTag("Spawner") && !other.CompareTag("AimHitLayer"))
        {

            if (other.gameObject.CompareTag("Player"))
            {

                HealthBar playerHealth = other.GetComponent<HealthBar>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20);
                }

                Destroy(gameObject);
            }
            // Destroi o projetil
            else Destroy(gameObject);
        }
    }
}
