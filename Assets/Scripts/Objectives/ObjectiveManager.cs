using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public List<Objective> objectiveList;

    private void Awake()
    {
        Instance = this;
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
    SpeedRun,
    NoDamage,
    Pacifist,
    Tricks,
    Kills,
    GlassBreaks,    //"feito"
    NoGlassBreaks,
    RailTime
}