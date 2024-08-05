using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSecondPieces : MonoBehaviour
{
    public int totalSecondaryPiecesScene;
    public bool isCompleted = false;
    private GameObject[] secondaryPieces;

    void Start()
    {
        secondaryPieces = GameObject.FindGameObjectsWithTag("SmallItem");

        totalSecondaryPiecesScene = secondaryPieces.Length;
    }

    private void Update()
    {
        VerifyMission();
    }

    void VerifyMission()
    {
        if (ObjectiveManager.Instance.smallItensCollected >= totalSecondaryPiecesScene)
        {
            isCompleted = true;
            for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
            {
                if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.SecondPieces)
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
