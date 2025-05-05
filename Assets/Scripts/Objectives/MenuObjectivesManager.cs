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
            switch (PlayerPrefs.GetInt("languageInt"))
            {
                case 0: //Português
                    mainObjectiveUIScript.objectiveText.text = "Roube a peça principal!";
                    break;
                case 1: //Inglês
                    mainObjectiveUIScript.objectiveText.text = "Steal the main piece!";
                    break;
            }
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
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Colete todas as peças secundárias";
                                break;
                            case 1: //Inglês
                                missionText = "Collect all secondary pieces";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.SpeedRun:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Complete a fase em " + currentObjectiveList[i].number + " segundos";
                                break;
                            case 1: //Inglês
                                missionText = "Complete the level in " + currentObjectiveList[i].number + " seconds";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.NoDamage:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Não sofra nenhum dano";
                                break;
                            case 1: //Inglês
                                missionText = "Don't receive any damage";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.Pacifist:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Não dê nenhum dano contra os guardas";
                                break;
                            case 1: //Inglês
                                missionText = "Don't deal any damage to the guards";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.Tricks:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Faça " + currentObjectiveList[i].number + " truques";
                                break;
                            case 1: //Inglês
                                missionText = "Perform " + currentObjectiveList[i].number + " tricks";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.Kills:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Derrote " + currentObjectiveList[i].number + " guardas";
                                break;
                            case 1: //Inglês
                                missionText = "Defeat " + currentObjectiveList[i].number + " guards";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.GlassBreaks:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Quebre " + currentObjectiveList[i].number + " painéis de vidro";
                                break;
                            case 1: //Inglês
                                missionText = "Break " + currentObjectiveList[i].number + " glass panels";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.NoGlassBreaks:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Não quebre nenhum painél de vidro";
                                break;
                            case 1: //Inglês
                                missionText = "Don't break any glass panels";
                                break;
                        }
                        break;
                    case OnMenuObjectiveType.RailTime:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Português
                                missionText = "Percorra os trilhos por " + currentObjectiveList[i].number + " segundos";
                                break;
                            case 1: //Inglês
                                missionText = "Ride the rails for " + currentObjectiveList[i].number + " seconds";
                                break;
                        }
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