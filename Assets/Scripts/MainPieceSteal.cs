using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPieceSteal : MonoBehaviour
{
    public GameObject normalMainPiece;
    public GameObject stolenTreasure;
    private bool isStolen = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed

    // Start is called before the first frame update
    void Start()
    {
        stolenTreasure.SetActive(false);
        normalMainPiece.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isStolen)
        {
            stolenTreasure.SetActive(true);
            normalMainPiece.SetActive(false);
            isStolen = true;
            StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
        }
    }

    private IEnumerator DestroyGlassAfterDelay(float delay)
    {
        //int vidroInteger = Random.Range(2, 4) + 1;
        //SFXManager.Instance.PlaySFXRandomPitch("glass" + vidroInteger);
        yield return new WaitForSeconds(delay);
        DestroyTreasure();
    }

    private void DestroyTreasure()
    {
        Destroy(stolenTreasure);
        Destroy(normalMainPiece);
        Destroy(gameObject);
    }
}