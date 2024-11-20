using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuObjectivesManager : MonoBehaviour
{
    [Header("LEVEL OBJECTIVE LIST")]
    public List<OnMenuObjective> level1ObjectiveList;
    public List<OnMenuObjective> level2ObjectiveList;

    [Header("OBJECTIVES GAME OBJECTS")]
    public GameObject objectiveUI;
    public Transform[] levelObjectiveUILists;
    private string missionText;


    private void Start()
    {
        List<OnMenuObjective>[] levelObjectiveLists = { level1ObjectiveList, level2ObjectiveList };

        for (int n = 0; n < levelObjectiveUILists.Length; n++)
        {
            if (n >= levelObjectiveLists.Length) break;

            // Select the current level's objective list
            List<OnMenuObjective> currentObjectiveList = levelObjectiveLists[n];

            //Define UI da missão principal.
            GameObject mainObjectiveUIObject = objectiveUI;
            ObjectiveUI mainObjectiveUIScript = mainObjectiveUIObject.GetComponent<ObjectiveUI>();
            mainObjectiveUIScript.objectiveText.text = "Roube a peça principal!";
            if (PlayerPrefs.GetInt("Level" + (n + 1) + "Mission1") != 0)
            {
                mainObjectiveUIScript.isCompleted = true;
            }
            else
            {
                mainObjectiveUIScript.wasCompleted = false;
                mainObjectiveUIScript.isCompleted = false;
            }
            mainObjectiveUIScript.CompleteInScene();
            Instantiate(mainObjectiveUIObject, levelObjectiveUILists[n]);
            mainObjectiveUIObject.transform.SetAsFirstSibling();

            //Define UI dos outros objetivos da fase.
            for (int i = 0; i < currentObjectiveList.Count; i++)
            {
                switch (currentObjectiveList[i].objectiveType)
                {
                    case OnMenuObjectiveType.SecondPieces:
                        missionText = "Colete todas as peças secundárias";
                        break;
                    case OnMenuObjectiveType.SpeedRun:
                        missionText = "Complete a fase em " + currentObjectiveList[i].number + " segundos";
                        break;
                    case OnMenuObjectiveType.NoDamage:
                        missionText = "Não sofra nenhum dano";
                        break;
                    case OnMenuObjectiveType.Pacifist:
                        missionText = "Não dê nenhum dano contra os guardas";
                        break;
                    case OnMenuObjectiveType.Tricks:
                        missionText = "Faça " + currentObjectiveList[i].number + " truques";
                        break;
                    case OnMenuObjectiveType.Kills:
                        missionText = "Derrote " + currentObjectiveList[i].number + " guardas";
                        break;
                    case OnMenuObjectiveType.GlassBreaks:
                        missionText = "Quebre " + currentObjectiveList[i].number + " painéis de vidro";
                        break;
                    case OnMenuObjectiveType.NoGlassBreaks:
                        missionText = "Não quebre nenhum painél de vidro";
                        break;
                    case OnMenuObjectiveType.RailTime:
                        missionText = "Percorra os trilhos por " + currentObjectiveList[i].number + " segundos";
                        break;
                    default:
                        break;
                }

                GameObject objectiveUIObject = objectiveUI;
                ObjectiveUI objectiveUIScript = objectiveUIObject.GetComponent<ObjectiveUI>();
                objectiveUIScript.objectiveText.text = missionText;
                if (PlayerPrefs.GetInt("Level" + (n + 1) + "Mission" + (i + 2)) != 0)
                {
                    objectiveUIScript.isCompleted = true;
                }
                else
                {
                    objectiveUIScript.wasCompleted = false;
                    objectiveUIScript.isCompleted = false;
                }

                    Instantiate(objectiveUIObject, levelObjectiveUILists[n]);
                mainObjectiveUIObject.transform.SetSiblingIndex(i + 1);
            }

            for (int i = 0; i < levelObjectiveUILists[n].childCount; i++)
            {
                ObjectiveUI script = levelObjectiveUILists[n].GetChild(i).GetComponent<ObjectiveUI>();
                script.CompleteInScene();
            }
        }
    }
}

[System.Serializable]
public class OnMenuObjective
{
    public OnMenuObjectiveType objectiveType;
    public int number;
    public bool isCompleted;
}

public enum OnMenuObjectiveType
{
    SecondPieces,
    SpeedRun,
    NoDamage,
    Pacifist,
    Tricks,
    Kills,
    GlassBreaks,
    NoGlassBreaks,
    RailTime
}