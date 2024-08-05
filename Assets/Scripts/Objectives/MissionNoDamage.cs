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
            for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
            {
                if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.NoDamage)
                {
                    ObjectiveManager.Instance.objectiveList[i].isCompleted = true;
                }
            }
        }
        else
        {
            isCompleted = false;
        }
    }
}
