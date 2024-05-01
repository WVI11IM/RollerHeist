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
    [Tooltip("A velocidade r�pida de persegui��o do inimigo.")]
    public float runSpeed;
    [Tooltip("O valor m�ximo de vida do inimigo.")]
    public float health;
    [Tooltip("A dist�ncia de detec��o entre o inimigo e o jogador.")]
    public float detectionDist;
    [Tooltip("A dist�ncia m�xima na qual o inimigo continuar� perseguindo o jogador.")]
    public float chaseDist;
    [Tooltip("A dist�ncia m�nima na qual o jogador tenha que estar pr�ximo ao inimigo para que ele inicie um ataque")]
    public float conflictDist;
}
