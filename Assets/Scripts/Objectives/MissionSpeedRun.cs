using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSpeedRun : MonoBehaviour
{
    public bool isCompleted = false;
    public float timeToBeat;

    void Start()
    {
        for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
        {
            if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.SpeedRun)
            {
                timeToBeat = ObjectiveManager.Instance.objectiveList[i].number;
            }
        }
    }

    private void Update()
    {
        VerifyMission();
    }

    void VerifyMission()
    {
        if (GameManager.Instance.state == GameState.Win && Timer.Instance.elapsedTime <= timeToBeat)
        {
            isCompleted = true;
            for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
            {
                if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.SpeedRun)
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
