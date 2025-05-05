using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("LEVEL OBJECTIVE LIST")]
    public List<Objective> objectiveList;

    [Header("OBJECTIVES GAME OBJECTS")]
    [SerializeField] GameObject missionSecondPieces;
    [SerializeField] GameObject missionSpeedRun;
    [SerializeField] GameObject missionNoDamage;
    [SerializeField] GameObject missionPacifist;
    [SerializeField] GameObject missionTricks;
    [SerializeField] GameObject missionKills;
    [SerializeField] GameObject missionGlassBreaks;
    [SerializeField] GameObject missionNoGlassBreaks;
    [SerializeField] GameObject missionRailTime;
    public GameObject objectiveUI;
    public Transform objectiveUIList;
    public GameObject pauseObjectiveList;
    public Transform pauseObjectiveUIList;
    private string missionText;
    public GameObject medal;

    [Header("LEVEL DATA")]
    public int defeatedEnemies = 0;
    public bool hasGivenDamage = false;
    public float railTime = 0;
    public bool isGrinding = false;
    public int tricksNumber = 0;
    public bool hasTakenDamage = false;

    [Header("COLLECTABLES DATA")]
    public TextMeshProUGUI[] bigItemTexts;
    public TextMeshProUGUI[] smallItemTexts;
    public float bigItemCollected;
    public float smallItensCollected;
    private float smallItensToCollect;

    [Header("QUEST PANEL ANIMATOR")]
    public Animator questPanelAnimator;
    private bool questPanelIsOn = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Reseta os dados ao in�cio da fase.
        defeatedEnemies = 0;
        railTime = 0;
        isGrinding = false;
        tricksNumber = 0;
        hasTakenDamage = false;
        if (GameManager.Instance.levelNumber != 0 || GameManager.Instance.state == GameState.None)
        {
            if (pauseObjectiveUIList != null && objectiveList.Count > 0)
            {
                pauseObjectiveList.SetActive(true);
            }
            else pauseObjectiveList.SetActive(false);

            //Procura todos os itens secund�rios.
            bigItemCollected = 0;
            smallItensCollected = 0;
            GameObject[] smallItens = GameObject.FindGameObjectsWithTag("SmallItem");
            smallItensToCollect = smallItens.Length;

            //Define UI da miss�o principal.
            GameObject mainObjectiveUIObject = objectiveUI;
            ObjectiveUI mainObjectiveUIScript = mainObjectiveUIObject.GetComponent<ObjectiveUI>();
            switch (PlayerPrefs.GetInt("languageInt"))
            {
                case 0: //Portugu�s
                    mainObjectiveUIScript.objectiveText.text = "Roube a pe�a principal!";
                    break;
                case 1: //Ingl�s
                    mainObjectiveUIScript.objectiveText.text = "Steal the main piece!";
                    break;
            }
            if (PlayerPrefs.GetInt("Level" + GameManager.Instance.levelNumber + "Mission1") != 0)
            {
                mainObjectiveUIScript.wasCompleted = true;
            }
            else mainObjectiveUIScript.wasCompleted = false;
            mainObjectiveUIScript.CompleteObjective();
            Instantiate(mainObjectiveUIObject, objectiveUIList);
            if (pauseObjectiveUIList != null && objectiveList.Count > 0)
            {
                Instantiate(mainObjectiveUIObject, pauseObjectiveUIList);
            }
            mainObjectiveUIObject.transform.SetAsFirstSibling();

            //Define UI dos outros objetivos da fase.
            for (int i = 0; i < objectiveList.Count; i++)
            {
                switch (objectiveList[i].objectiveType)
                {
                    case ObjectiveType.SecondPieces:
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Colete todas as pe�as secund�rias";
                                break;
                            case 1: //Ingl�s
                                missionText = "Collect all secondary pieces";
                                break;
                        }
                        Instantiate(missionSecondPieces);
                        break;
                    case ObjectiveType.SpeedRun:
                        Instantiate(missionSpeedRun);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Complete a fase em " + objectiveList[i].number + " segundos";
                                break;
                            case 1: //Ingl�s
                                missionText = "Complete the level in " + objectiveList[i].number + " seconds";
                                break;
                        }
                        break;
                    case ObjectiveType.NoDamage:
                        Instantiate(missionNoDamage);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "N�o sofra nenhum dano";
                                break;
                            case 1: //Ingl�s
                                missionText = "Don't receive any damage";
                                break;
                        }
                        break;
                    case ObjectiveType.Pacifist:
                        Instantiate(missionPacifist);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "N�o d� nenhum dano contra os guardas";
                                break;
                            case 1: //Ingl�s
                                missionText = "Don't deal any damage to the guards";
                                break;
                        }
                        break;
                    case ObjectiveType.Tricks:
                        Instantiate(missionTricks);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Fa�a " + objectiveList[i].number + " truques";
                                break;
                            case 1: //Ingl�s
                                missionText = "Perform " + objectiveList[i].number + " tricks";
                                break;
                        }
                        break;
                    case ObjectiveType.Kills:
                        Instantiate(missionKills);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Derrote " + objectiveList[i].number + " guardas";
                                break;
                            case 1: //Ingl�s
                                missionText = "Defeat " + objectiveList[i].number + " guards";
                                break;
                        }
                        break;
                    case ObjectiveType.GlassBreaks:
                        Instantiate(missionGlassBreaks);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Quebre " + objectiveList[i].number + " pain�is de vidro";
                                break;
                            case 1: //Ingl�s
                                missionText = "Break " + objectiveList[i].number + " glass panels";
                                break;
                        }
                        break;
                    case ObjectiveType.NoGlassBreaks:
                        Instantiate(missionNoGlassBreaks);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "N�o quebre nenhum pain�l de vidro";
                                break;
                            case 1: //Ingl�s
                                missionText = "Don't break any glass panels";
                                break;
                        }
                        break;
                    case ObjectiveType.RailTime:
                        Instantiate(missionRailTime);
                        switch (PlayerPrefs.GetInt("languageInt"))
                        {
                            case 0: //Portugu�s
                                missionText = "Percorra os trilhos por " + objectiveList[i].number + " segundos";
                                break;
                            case 1: //Ingl�s
                                missionText = "Ride the rails for " + objectiveList[i].number + " seconds";
                                break;
                        }
                        break;
                    default:
                        break;
                }

                GameObject objectiveUIObject = objectiveUI;
                ObjectiveUI objectiveUIScript = objectiveUIObject.GetComponent<ObjectiveUI>();
                objectiveUIScript.objectiveText.text = missionText;
                if (PlayerPrefs.GetInt("Level" + GameManager.Instance.levelNumber + "Mission" + (i + 2)) != 0)
                {
                    objectiveUIScript.wasCompleted = true;
                }
                else objectiveUIScript.wasCompleted = false;
                Instantiate(objectiveUIObject, objectiveUIList);
                if (pauseObjectiveUIList != null && objectiveList.Count > 0)
                {
                    Instantiate(objectiveUIObject, pauseObjectiveUIList);
                }
                mainObjectiveUIObject.transform.SetSiblingIndex(i + 1);
            }

            for (int i = 0; i < objectiveUIList.childCount; i++)
            {
                ObjectiveUI script = objectiveUIList.GetChild(i).GetComponent<ObjectiveUI>();
                script.CompleteObjective();
            }

            if (pauseObjectiveUIList != null && objectiveList.Count > 0)
            {
                for (int i = 0; i < pauseObjectiveUIList.childCount; i++)
                {
                    ObjectiveUI script = pauseObjectiveUIList.GetChild(i).GetComponent<ObjectiveUI>();
                    script.CompleteObjective();
                }
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.levelNumber != 0 || GameManager.Instance.state == GameState.None)
        {
            //Calcula o tempo percorrido nos trilhos.
            if (isGrinding)
            {
                railTime += Time.deltaTime;
            }

            //Verifica se o player coletou o item principal para mudar o estado do jogo.
            if (GameManager.Instance.state == GameState.Invade)
            {
                GotItem();
            }
            //SetCollectables();

            //Caso a fase seja conclu�da, os dados de player prefs s�o atualizados dependendo de quais miss�es forem conclu�das.
            if (GameManager.Instance.state == GameState.Win)
            {
                PlayerPrefs.SetInt("Level" + GameManager.Instance.levelNumber + "Mission1", 1);

                ObjectiveUI mainMissionScript = objectiveUIList.GetChild(0).GetComponent<ObjectiveUI>();

                if (PlayerPrefs.GetInt("Level" + GameManager.Instance.levelNumber + "Mission1") != 0)
                {
                    mainMissionScript.isCompleted = true;
                    mainMissionScript.wasCompleted = true;
                }
                else
                {
                    mainMissionScript.isCompleted = false;
                    mainMissionScript.wasCompleted = false;
                }

                mainMissionScript.CompleteObjective();

                for (int i = 0; i < objectiveList.Count; i++)
                {
                    if (objectiveList[i].isCompleted)
                    {
                        PlayerPrefs.SetInt("Level" + GameManager.Instance.levelNumber + "Mission" + (i + 2), 1);
                        ObjectiveUI script = objectiveUIList.GetChild(i + 1).GetComponent<ObjectiveUI>();
                        if (PlayerPrefs.GetInt("Level" + GameManager.Instance.levelNumber + "Mission" + (i + 2)) != 0)
                        {
                            script.wasCompleted = true;
                        }
                        else script.wasCompleted = false;
                        script.CompleteObjective();
                    }
                }

                if (mainMissionScript.isCompleted == true)
                {
                    int completedObjectives = 0;
                    for (int i = 0; i < objectiveList.Count; i++)
                    {
                        if (objectiveList[i].isCompleted)
                        {
                            completedObjectives += 1;
                        }
                    }
                    if (completedObjectives >= 4)
                    {
                        PlayerPrefs.SetInt("Level" + GameManager.Instance.levelNumber + "Mission6", 1);
                        if (medal != null) medal.SetActive(true);
                    }
                    else if (medal != null) medal.SetActive(false);
                }
            }

            for (int i = 0; i < objectiveList.Count; i++)
            {
                ObjectiveUI script = objectiveUIList.GetChild(i + 1).GetComponent<ObjectiveUI>();
                if (objectiveList[i].isCompleted && !script.isCompleted)
                {
                    script.isCompleted = true;
                }
                else if (!objectiveList[i].isCompleted)
                {
                    script.isCompleted = false;
                }
            }
        }

        questPanelAnimator.SetBool("isOn", questPanelIsOn);
    }

    void SetCollectables()
    {
        //Texto dos itens coletados.
        for (int i = 0; i < bigItemTexts.Length; i++)
        {
            bigItemTexts[i].text = bigItemCollected + "/1";
        }
        for (int i = 0; i < smallItemTexts.Length; i++)
        {
            smallItemTexts[i].text = smallItensCollected + "/" + smallItensToCollect;
        }
    }

    public void GotItem()
    {
        //Se o item principal for coletado, o estado do jogo passa a ser ESCAPE.
        if (bigItemCollected >= 1)
        {
            GameManager.Instance.UpdateGameState(GameState.Escape);
        }
    }

    public void EnemyDefeated()
    {
        defeatedEnemies++;
    }

    public void QuestPanel(bool isActive)
    {
        questPanelIsOn = isActive;
    }

    public void CallQuestPanel()
    {
        StartCoroutine(ShowQuestPanel());
    }
    IEnumerator ShowQuestPanel()
    {
        QuestPanel(true);
        yield return new WaitForSeconds(2f);
        QuestPanel(false);
    }

    public void QuestPanelMainItemNumber()
    {
        StartCoroutine(IncreaseMainItemNumber());
    }

    IEnumerator IncreaseMainItemNumber()
    {
        QuestPanel(true);
        yield return new WaitForSeconds(0.25f);
        questPanelAnimator.SetTrigger("gotMainItem");
        yield return new WaitForSeconds(0.25f);
        SetCollectables();
        yield return new WaitForSeconds(1.5f);
        QuestPanel(false);
    }
    public void QuestPanelSecondaryItemNumber()
    {
        StartCoroutine(IncreaseSecondaryItemNumber());
    }
    IEnumerator IncreaseSecondaryItemNumber()
    {
        QuestPanel(true);
        yield return new WaitForSeconds(0.25f);
        questPanelAnimator.SetTrigger("gotSecondaryItem");
        yield return new WaitForSeconds(0.25f);
        SetCollectables();
        yield return new WaitForSeconds(1.5f);
        QuestPanel(false);
    }
}

[System.Serializable]
public class Objective
{
    public ObjectiveType objectiveType;
    public int number;
    public bool isCompleted;
}

public enum ObjectiveType
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