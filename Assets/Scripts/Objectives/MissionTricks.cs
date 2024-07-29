using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTricks : MonoBehaviour
{
    public bool isCompleted = false;
    public int tricksToPerform = 0;

    void Start()
    {
        for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
        {
            if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.Tricks)
            {
                tricksToPerform = ObjectiveManager.Instance.objectiveList[i].number;
            }
        }
    }

    private void Update()
    {
        VerifyMission();
    }

    void VerifyMission()
    {
        if (ObjectiveManager.Instance.tricksNumber >= tricksToPerform)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
    }
}
