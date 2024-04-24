using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool canFollow = false;

    public static EnemyManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
