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

    public Image star;

    public TextMeshProUGUI objectiveText;

    void Start()
    {
        CompleteObjective();
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.Win)
        {
            CompleteObjective();
        }
    }

    public void CompleteObjective()
    {
        //Image star = GetComponent<Image>();
        if (wasCompleted)
        {
            star.enabled = true;
        }
        else
        {
            star.enabled = false;
        }
    }
}
