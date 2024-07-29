using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionRailTime : MonoBehaviour
{
    public bool isCompleted = false;

    public int railTimeToSurpass = 0;

    void Start()
    {
        isCompleted = false;
        for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
        {
            if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.RailTime)
            {
                railTimeToSurpass = ObjectiveManager.Instance.objectiveList[i].number;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ObjectiveManager.Instance.railTime >= railTimeToSurpass)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
    }
}
