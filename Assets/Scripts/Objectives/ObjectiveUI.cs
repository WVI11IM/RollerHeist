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
    Image box;

    public TextMeshProUGUI objectiveText;

    void Start()
    {
        box = GetComponent<Image>();
        CompleteObjective();
    }

    void Update()
    {
        CompleteInScene();

        if (GameManager.Instance.state == GameState.Win)
        {
            CompleteObjective();
        }
    }

    public void CompleteObjective()
    {
        if (wasCompleted)
        {
            star.enabled = true;
        }
        else
        {
            star.enabled = false;
        }
    }

    public void CompleteInScene()
    {
        if (isCompleted)
        {
            box.color = Color.green;
        }
        else
        {
            
        }
    }
}
