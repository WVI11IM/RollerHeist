using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    //estrelinha
    public bool wasCompleted;
    //conclus�o dentro de fase
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
        //Se o objetivo for completo j� uma �nica vez, a estrela j� � adquirida.
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
        //Se o objetivo for completo dentro da fase, a caixa fica verde.
        if (isCompleted)
        {
            box.color = Color.green;
        }
        else
        {
            //Manter cor de caixa padr�o.
        }
    }
}
