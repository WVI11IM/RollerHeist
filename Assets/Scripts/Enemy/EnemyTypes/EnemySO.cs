using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public float walkSpeed;
    public float runSpeed;
    public float health;
    public float detectionDist;
    public float detectionAngle;
    public float detectionReactionTime;
    public float chaseDist;
    public float conflictDist;
}
