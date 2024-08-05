using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("LEVEL OBJECTIVE LIST")]
    public List<Objective> objectiveList;

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