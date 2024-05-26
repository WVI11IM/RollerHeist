using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    public GameObject normalGlass;
    public GameObject brokenGlass;
    private bool isBroken = false;
    public float scaleReductionFactor = 1.01f; // Factor by which to reduce the scale
    public float destructionDelay = 5f; // Time in seconds before both glass objects are destroyed
    public float minScale = 0.1f; // Minimum scale before destroying the object

    // Start is called before the first frame update
    void Start()
    {
        brokenGlass.SetActive(false);
        normalGlass.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBroken)
        {
            bool allChildrenBelowMinScale = true;

            foreach (Transform child in brokenGlass.transform)
            {
                GameObject childGameObject = child.gameObject;
                childGameObject.transform.localScale *= 1 - (Time.deltaTime * (scaleReductionFactor - 1));

                if (childGameObject.transform.localScale.x > minScale || childGameObject.transform.localScale.y > minScale || childGameObject.transform.localScale.z > minScale)
                {
                    allChildrenBelowMinScale = false;
                }
            }

            if (allChildrenBelowMinScale)
            {
                DestroyGlass();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Projetil"))
        {
            brokenGlass.SetActive(true);
            normalGlass.SetActive(false);
            isBroken = true;
            StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
        }
    }

    private IEnumerator DestroyGlassAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyGlass();
    }

    private void DestroyGlass()
    {
        Destroy(brokenGlass);
        Destroy(normalGlass);
        Destroy(gameObject); // Destroy this script's GameObject
    }
}