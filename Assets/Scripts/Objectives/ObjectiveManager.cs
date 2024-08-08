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
    private string missionText;

    [Header("LEVEL DATA")]
    public int defeatedEnemies = 0;
    public bool hasGivenDamage = false;
    public float railTime = 0;
    public bool isGrinding = false;
    public int tricksNumber = 0;
    public bool hasTakenDamage = false;

    [Header("COLLECTABLES DATA")]
    public TextMeshProUGUI bigItem;
    public TextMeshProUGUI smallItem;
    public float bigItemCollected;
    public float smallItensCollected;
    private float smallItensToCollect;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        defeatedEnemies = 0;
        railTime = 0;
        isGrinding = false;
        tricksNumber = 0;
        hasTakenDamage = false;

        bigItemCollected = 0;
        smallItensCollected = 0;
        GameObject[] smallItens = GameObject.FindGameObjectsWithTag("SmallItem");
        smallItensToCollect = smallItens.Length;

        GameObject mainObjectiveUIObject = objectiveUI;
        //mainObjectiveUIObject.transform.SetSiblingIndex(0);
        ObjectiveUI mainObjectiveUIScript = mainObjectiveUIObject.GetComponent<ObjectiveUI>();
        mainObjectiveUIScript.objectiveText.text = "Roube a peça principal!";
        Instantiate(mainObjectiveUIObject, objectiveUIList);
        mainObjectiveUIObject.transform.SetAsFirstSibling();

        for (int i = 0; i < objectiveList.Count; i++)
        {
            //string missionText;

            switch (objectiveList[i].objectiveType)
            {
                case ObjectiveType.SecondPieces:
                    missionText = "Colete todas as peças secundárias";
                    Instantiate(missionSecondPieces);
                    break;
                case ObjectiveType.SpeedRun:
                    Instantiate(missionSpeedRun);
                    missionText = "Complete em " + objectiveList[i].number + " segundos";
                    break;
                case ObjectiveType.NoDamage:
                    Instantiate(missionNoDamage);
                    missionText = "Não sofra nenhum dano";
                    break;
                case ObjectiveType.Pacifist:
                    Instantiate(missionPacifist);
                    missionText = "Não dê nenhum dano";
                    break;
                case ObjectiveType.Tricks:
                    Instantiate(missionTricks);
                    break;
                case ObjectiveType.Kills:
                    Instantiate(missionKills);
                    break;
                case ObjectiveType.GlassBreaks:
                    Instantiate(missionGlassBreaks);
                    missionText = "Quebre " + objectiveList[i].number + " painéis de vidros";
                    break;
                case ObjectiveType.NoGlassBreaks:
                    Instantiate(missionNoGlassBreaks);
                    missionText = "Quebre 0 painéis de vidros";
                    break;
                case ObjectiveType.RailTime:
                    Instantiate(missionRailTime);
                    break;
                default:
                    break;
            }

            GameObject objectiveUIObject = objectiveUI;
            ObjectiveUI objectiveUIScript = objectiveUIObject.GetComponent<ObjectiveUI>();
            objectiveUIScript.objectiveText.text = missionText;
            Instantiate(objectiveUIObject, objectiveUIList);
            mainObjectiveUIObject.transform.SetSiblingIndex(i + 1);
        }

        Debug.Log("Level1Mission1 = " + PlayerPrefs.GetInt("Level1Mission1"));
        Debug.Log("Level1Mission2 = " + PlayerPrefs.GetInt("Level1Mission2"));
        Debug.Log("Level1Mission3 = " + PlayerPrefs.GetInt("Level1Mission3"));
        Debug.Log("Level1Mission4 = " + PlayerPrefs.GetInt("Level1Mission4"));
        Debug.Log("Level1Mission5 = " + PlayerPrefs.GetInt("Level1Mission5"));
    }

    private void Update()
    {
        if (isGrinding)
        {
            railTime += Time.deltaTime;
        }

        if (GameManager.Instance.state == GameState.Invade)
        {
            GotItem();
        }
        SetCollectables();

        if(GameManager.Instance.state == GameState.Win)
        {
            PlayerPrefs.SetInt("Level" + GameManager.Instance.levelNumber + "Mission1", 1);

            for (int i = 0; i < objectiveList.Count; i++)
            {
                if (objectiveList[i].isCompleted)
                {
                    PlayerPrefs.SetInt("Level" + GameManager.Instance.levelNumber + "Mission" + (i + 2), 1);
                }
            }
            Debug.Log("Level1Mission1 = " + PlayerPrefs.GetInt("Level1Mission1"));
            Debug.Log("Level1Mission2 = " + PlayerPrefs.GetInt("Level1Mission2"));
            Debug.Log("Level1Mission3 = " + PlayerPrefs.GetInt("Level1Mission3"));
            Debug.Log("Level1Mission4 = " + PlayerPrefs.GetInt("Level1Mission4"));
            Debug.Log("Level1Mission5 = " + PlayerPrefs.GetInt("Level1Mission5"));
        }

        //TEST
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    PlayerPrefs.SetInt("Level" + i + "Mission" + j, 0);
                }
            }
        }
    }

    void SetCollectables()
    {
        bigItem.text = bigItemCollected + "/1";
        smallItem.text = smallItensCollected + "/" + smallItensToCollect;
    }

    public void GotItem()
    {
        if (bigItemCollected >= 1)
        {
            GameManager.Instance.UpdateGameState(GameState.Escape);
        }
    }

    public void EnemyDefeated()
    {
        defeatedEnemies++;
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
    SecondPieces,   //"feito"
    SpeedRun,   //"feito"
    NoDamage,   //"feito"
    Pacifist,   //"feito"
    Tricks,   //"feito"
    Kills,  //"feito"
    GlassBreaks,    //"feito"
    NoGlassBreaks,  //"feito"
    RailTime   //"feito"
}