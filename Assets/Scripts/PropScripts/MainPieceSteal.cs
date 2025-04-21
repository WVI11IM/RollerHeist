using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainPieceSteal : MonoBehaviour
{
    public GameObject normalMainPiece;
    public GameObject stolenTreasure;
    public GameObject stolenTreasureOnPlayer;
    public NavMeshObstacle navMeshObstacle;
    private bool isStolen = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed

    // Start is called before the first frame update
    void Start()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = true;
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
            navMeshObstacle.enabled = false;
            isStolen = true;
        }
    }
}