using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemySO : ScriptableObject
{
    [Tooltip("O nome do inimigo.")]
    public string enemyName;
    [Tooltip("A velocidade lenta de patrulha do inimigo.")]
    public float walkSpeed;
    [Tooltip("A velocidade rápida de perseguição do inimigo.")]
    public float runSpeed;
    [Tooltip("O valor máximo de vida do inimigo.")]
    public float health;
    [Tooltip("A distância de detecção entre o inimigo e o jogador.")]
    public float detectionDist;
    [Tooltip("A distância máxima na qual o inimigo continuará perseguindo o jogador.")]
    public float chaseDist;
    [Tooltip("A distância mínima na qual o jogador tenha que estar próximo ao inimigo para que ele inicie um ataque")]
    public float conflictDist;
}
