using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marca : MonoBehaviour
{
    public float tempoParaDesaparecer = 5f; // Tempo em segundos antes da marca de tinta desaparecer

    void Start()
    {
        // Inicia a coroutine para destruir a marca de tinta após o tempo especificado
        StartCoroutine(DestruirMarcaDeTinta());
    }

    IEnumerator DestruirMarcaDeTinta()
    {
        // Aguarda o tempo especificado
        yield return new WaitForSeconds(tempoParaDesaparecer);

        // Destroi a marca de tinta após o tempo especificado
        Destroy(gameObject);
    }
}