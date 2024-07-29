using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNoDamage : MonoBehaviour
{
    public bool isCompleted = false;
    public bool hasTakenDamage = false;

    void Start()
    {
        isCompleted = false;
        hasTakenDamage = false;
}

    void Update()
    {
        if (ObjectiveManager.Instance.hasTakenDamage)
        {
            hasTakenDamage = true;
        }

        if (GameManager.Instance.state == GameState.Win && !hasTakenDamage)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
    }
}
