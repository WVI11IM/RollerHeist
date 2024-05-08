using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiro : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 50f;
    public float spawnOffset = 2f;
    public float cooldownEntreTiros = 0.2f; // Tempo de espera entre cada tiro
    public float tempoDeRecarga = 2.5f; // Tempo de recarga ap�s 20 tiros
    private int tirosDisparados = 0; // Contador de tiros disparados
    private bool emRecarga = false; // Flag indicando se o jogador est� em recarga
    private float tempoUltimoTiro; // Tempo do �ltimo tiro

    void Update()
    {
        if (!emRecarga && Input.GetMouseButton(0)) // Verifica se o bot�o esquerdo do mouse est� sendo segurado
        {
            // Verifica se o cooldown entre tiros foi atingido
            if (Time.time >= tempoUltimoTiro + cooldownEntreTiros)
            {
                // Realiza o tiro
                Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                Vector3 shootDirection = GetShootDirection();
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.velocity = shootDirection * projectileSpeed;

                // Incrementa o contador de tiros disparados
                tirosDisparados++;
                tempoUltimoTiro = Time.time; // Atualiza o tempo do �ltimo tiro

                // Verifica se o jogador atingiu o limite de tiros
                if (tirosDisparados >= 20)
                {
                    // Inicia a recarga
                    IniciarRecarga();
                }
            }
        }

        // Verifica se o jogador est� em recarga
        if (emRecarga)
        {
            // Verifica se o tempo de recarga passou
            if (Time.time >= tempoUltimoTiro + tempoDeRecarga)
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
        tempoUltimoTiro = Time.time; // Atualiza o tempo do �ltimo tiro
    }
}