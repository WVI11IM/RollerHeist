using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionKills : MonoBehaviour
{
    public bool isCompleted = false;
    public int enemiesToDefeat;

    void Start()
    {
        for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
        {
            if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.Kills)
            {
                enemiesToDefeat = ObjectiveManager.Instance.objectiveList[i].number;
            }
        }
    }

    private void Update()
    {
        VerifyMission();
    }

    void VerifyMission()
    {
        if(ObjectiveManager.Instance.defeatedEnemies >= enemiesToDefeat)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
    }
}
