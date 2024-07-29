using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSpeedRun : MonoBehaviour
{
    private float timeToBeat;

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
}
