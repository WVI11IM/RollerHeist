using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float lifetime = 3f;
    public GameObject Marca; // Refer�ncia ao Prefab da marca de tinta

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colidir com algo que n�o seja o jogador
        if (!other.gameObject.CompareTag("Player") && !other.CompareTag("Spawner") && !other.CompareTag("AimHitLayer"))
        {
            // Destroi o projetil
            Destroy(gameObject);

            // Se o objeto atingido for uma parede
            if (other.gameObject.CompareTag("Parede"))
            {
                // Instancia a marca de tinta na posi��o do impacto
                Instantiate(Marca, transform.position, Quaternion.identity);
            }
        }
    }
}