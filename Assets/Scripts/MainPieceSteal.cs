using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPieceSteal : MonoBehaviour
{
    public GameObject normalMainPiece;
    public GameObject stolenTreasure;
    public GameObject stolenTreasureOnPlayer;
    private bool isStolen = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed

    // Start is called before the first frame update
    void Start()
    {
        stolenTreasure.SetActive(false);
        normalMainPiece.SetActive(true);
        stolenTreasureOnPlayer.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isStolen)
        {
            stolenTreasure.SetActive(true);
            normalMainPiece.SetActive(false);
            stolenTreasureOnPlayer.SetActive(true);
            isStolen = true;
        }
    }
}