using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class CollectableManager : MonoBehaviour
{
    [Header("Collectables Text References")]
    public TextMeshProUGUI bigItem;
    public TextMeshProUGUI smallItem;

    public float bigItemCollected;
    public float smallItensCollected;
    private float smallItensToCollect;

    public static CollectableManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    void Start()
    {
        bigItemCollected = 0;
        smallItensCollected = 0;

        GameObject[] smallItens = GameObject.FindGameObjectsWithTag("SmallItem");
        smallItensToCollect = smallItens.Length;
    }

    void Update()
    {
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
}
