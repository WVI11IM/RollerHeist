using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float lifetime = 3f;
    public GameObject Marca; // Referência ao Prefab da marca de tinta
    public GameObject impactParticles;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colidir com algo que não seja o jogador
        if (!other.gameObject.CompareTag("Player") && !other.CompareTag("Spawner") && !other.CompareTag("AimHitLayer"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (!enemy.hasFainted)
                {
                    Instantiate(impactParticles, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            // Destroi o projetil
            else
            {
                Instantiate(impactParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}