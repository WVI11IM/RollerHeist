using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContadorBalas : MonoBehaviour
{
    public int balasMaximas = 20;
    public float tempoDeRecarga = 2.5f;
    public float cooldownEntreTiros = 0.2f; // Tempo de espera entre cada tiro
    private int balasRestantes;
    private float tempoInicioRecarga;
    private bool emRecarga;
    private float tempoUltimoTiro; // Tempo do último tiro

    public TMP_Text textoBalas;

    void Start()
    {
        balasRestantes = balasMaximas;
        AtualizarTextoBalas();
    }

    void Update()
    {
        if (!emRecarga && Input.GetMouseButton(0)) // Modificado para o botão esquerdo do mouse
        {
            // Verifica se o cooldown entre tiros foi atingido
            if (Time.time >= tempoUltimoTiro + cooldownEntreTiros)
            {
                if (balasRestantes > 0)
                {
                    balasRestantes--;
                    AtualizarTextoBalas();
                    tempoUltimoTiro = Time.time; // Atualiza o tempo do último tiro
                }
                else
                {
                    IniciarRecarga();
                }
            }
        }

        if (emRecarga && Time.time >= tempoInicioRecarga + tempoDeRecarga)
        {
            balasRestantes = balasMaximas;
            emRecarga = false;
            AtualizarTextoBalas();
        }

        // Verifica se o contador de balas chegou a 0 e inicia a recarga
        if (balasRestantes == 0 && !emRecarga)
        {
            IniciarRecarga();
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