using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    //estrelinha
    public bool wasCompleted;
    //conclusão dentro de fase
    public bool isCompleted;

    public Image wasCompleteIcon;
    public Image isCompleteIcon;

    public TextMeshProUGUI objectiveText;

    void Start()
    {
        CompleteObjective();
    }

    void Update()
    {
        CompleteInScene();
        if(GameManager.Instance != null)
        {
            if (GameManager.Instance.state == GameState.Win)
            {
                CompleteObjective();
            }
        }
    }

    public void CompleteObjective()
    {
        //Se o objetivo for completo já uma única vez, a estrela já é adquirida, porém aparece translúcida.
        if (wasCompleted)
        {
            wasCompleteIcon.enabled = true;
        }
        else
        {
            wasCompleteIcon.enabled = false;
        }
    }

    public void CompleteInScene()
    {
        //Se o objetivo for completo dentro da fase, a estrela é adquirida.
        if (isCompleted)
        {
            isCompleteIcon.enabled = true;
        }
        else
        {
            isCompleteIcon.enabled = false;
        }
    }
}
