using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPacifist : MonoBehaviour
{
    public bool isCompleted = false;

    public bool hasGivenDamage = false;

    void Start()
    {
        isCompleted = false;
        hasGivenDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ObjectiveManager.Instance.hasGivenDamage)
        {
            hasGivenDamage = true;
        }

        if(GameManager.Instance.state == GameState.Win && !hasGivenDamage)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
    }
}
