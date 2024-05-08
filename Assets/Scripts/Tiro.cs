using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiro : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float spawnOffset = 1f;
    public float tempoDeRecarga = 5f; // Tempo de recarga após 30 tiros
    private int tirosDisparados = 0; // Contador de tiros disparados
    private bool emRecarga = false; // Flag indicando se o jogador está em recarga
    private float tempoInicioRecarga; // Tempo de início da recarga

    void Update()
    {
        if (!emRecarga && Input.GetKeyDown(KeyCode.Return))
        {
            Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Vector3 shootDirection = GetShootDirection();
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = shootDirection * projectileSpeed;

            // Incrementa o contador de tiros disparados
            tirosDisparados++;

            // Verifica se o jogador atingiu o limite de tiros
            if (tirosDisparados >= 30)
            {
                // Inicia a recarga
                IniciarRecarga();
            }
        }

        // Verifica se o jogador está em recarga
        if (emRecarga)
        {
            // Verifica se o tempo de recarga passou
            if (Time.time >= tempoInicioRecarga + tempoDeRecarga)
            {
                // Reinicia o contador de tiros e sai do estado de recarga
                tirosDisparados = 0;
                emRecarga = false;
            }
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

    private void IniciarRecarga()
    {
        // Inicia a recarga
        emRecarga = true;
        tempoInicioRecarga = Time.time;
    }
}
