using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public List<Objective> objectiveList;

    public int defeatedEnemies = 0;
    public bool hasGivenDamage = false;

    public float railTime = 0;
    public bool isGrinding = false;

    public int tricksNumber = 0;

    public bool hasTakenDamage = false;

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
    }
    private void Update()
    {
        if (isGrinding)
        {
            railTime += Time.deltaTime;
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