using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContadorBalas : MonoBehaviour
{
    public int balasMaximas = 30;
    public float tempoDeRecarga = 5f;

    private int balasRestantes;
    private float tempoInicioRecarga;
    private bool emRecarga;

    public TMP_Text textoBalas;

    void Start()
    {
        balasRestantes = balasMaximas;
        AtualizarTextoBalas();
    }

    void Update()
    {
        if (!emRecarga && Input.GetKeyDown(KeyCode.Return)) // Modificado para a tecla Enter
        {
            if (balasRestantes > 0)
            {
                balasRestantes--;
                AtualizarTextoBalas();
            }
            else
            {
                IniciarRecarga();
            }
        }

        if (emRecarga && Time.time >= tempoInicioRecarga + tempoDeRecarga)
        {
            balasRestantes = balasMaximas;
            emRecarga = false;
            AtualizarTextoBalas();
        }
    }

    private void IniciarRecarga()
    {
        tempoInicioRecarga = Time.time;
        emRecarga = true;
        textoBalas.text = "Recarregando";
    }

    private void AtualizarTextoBalas()
    {
        textoBalas.text = balasRestantes > 0 ? "Balas: " + balasRestantes : "Balas: ";
    }
}

