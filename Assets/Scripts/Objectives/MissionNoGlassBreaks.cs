using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNoGlassBreaks : MonoBehaviour
{
    public int totalGlassScene;

    public bool isCompleted = false;

    private GameObject[] glassPanels;
    public List<GameObject> glassPanelsList;

    //public int brokenGlassPanels = 0;
    //public int glassPanelsToBreak;

    void Start()
    {
        for (int i = 0; i < ObjectiveManager.Instance.objectiveList.Count; i++)
        {
            if (ObjectiveManager.Instance.objectiveList[i].objectiveType == ObjectiveType.NoGlassBreaks)
            {
                //glassPanelsToBreak = ObjectiveManager.Instance.objectiveList[i].number;

                //retorna array de todos os vidros na cena.
                glassPanels = GameObject.FindGameObjectsWithTag("Glass");

                //adiciona os vidros do array numa lista
                for (int a = 0; a < glassPanels.Length; a++)
                {
                    glassPanelsList.Add(glassPanels[a]);
                }

                totalGlassScene = glassPanels.Length;
            }
        }
    }

    private void Update()
    {
        //if (GameManager.Instance.state == GameState.Win)
        //{
        VerifyMission();
        //}
    }

    void VerifyMission()
    {
        for (int i = 0; i < glassPanelsList.Count; i++)
        {
            if (glassPanelsList[i].GetComponent<GlassBreak>().isBroken)
            {
                glassPanelsList.Remove(glassPanelsList[i]);
                //brokenGlassPanels++;
                Debug.Log("glass was break");
            }
        }

        //brokenGlassPanels = totalGlassScene - glassPanelsList.Count;

        if (glassPanelsList.Count == totalGlassScene)
        {
            isCompleted = true;
        }
        else isCompleted = false;
    }
}
