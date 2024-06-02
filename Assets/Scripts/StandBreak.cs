using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandBreak : MonoBehaviour
{
    public GameObject normalStand;
    public GameObject brokenStand;
    private bool isBroken = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed

    // Start is called before the first frame update
    void Start()
    {
        brokenStand.SetActive(false);
        normalStand.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isBroken)
        {
            brokenStand.SetActive(true);
            normalStand.SetActive(false);
            isBroken = true;
            StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
        }
    }

    private IEnumerator DestroyGlassAfterDelay(float delay)
    {
        int vidroInteger = Random.Range(2, 4) + 1;
        SFXManager.Instance.PlaySFXRandomPitch("glass" + vidroInteger);
        yield return new WaitForSeconds(delay);
        DestroyGlass();
    }

    private void DestroyGlass()
    {
        Destroy(brokenStand);
        Destroy(normalStand);
        Destroy(gameObject);
    }
}