using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiro : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float spawnOffset = 1f; 

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;

            
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            
            Vector3 shootDirection = GetShootDirection();

            
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = shootDirection * projectileSpeed;
        }
    }

    Vector3 GetShootDirection()
    {
        
        MovementTest2 movementScript = GetComponent<MovementTest2>();
        if (movementScript != null)
        {
           
            return movementScript.transform.forward;
        }
        else
        {
            
            return transform.forward;
        }
    }
}
