using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float lifetime = 3f;  // Lifetime of the projectile
    public GameObject Marca;      // Reference to the paint mark prefab
    public GameObject impactParticles; // Reference to the impact particle prefab
    private Rigidbody rb;         // Reference to the Rigidbody component

    private void Start()
    {
        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If collides with something that is not the player, spawner, aim hit layer, or glass
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Spawner") &&
            !collision.gameObject.CompareTag("AimHitLayer") && !collision.gameObject.CompareTag("Glass"))
        {
            ParticlesAndDestroy();
        }
    }

    public void ParticlesAndDestroy()
    {
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}