using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    public GameObject normalGlass;
    public GameObject brokenGlass;
    public bool isBroken = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed
    public float fadeDuration = 30f; // Duration over which transparency will fade to zero
    public float minAlpha = 0.001f; // Minimum alpha before destroying the object

    private List<Material> glassMaterials = new List<Material>();

    void Start()
    {
        brokenGlass.SetActive(false);
        normalGlass.SetActive(true);

        // Get materials of all children of brokenGlass for transparency control
        foreach (Transform child in brokenGlass.transform)
        {
            if (child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                // Create a new material instance for each child to control individually
                Material newMat = renderer.material;
                glassMaterials.Add(newMat);
            }
        }
    }

    void Update()
    {
        if (isBroken)
        {
            bool allChildrenBelowMinAlpha = true;

            foreach (Material mat in glassMaterials)
            {
                Color color = mat.color;
                color.a -= Time.deltaTime / fadeDuration; // Reduce alpha over time
                mat.color = color;

                if (mat.color.a > minAlpha)
                {
                    allChildrenBelowMinAlpha = false;
                }
            }

            if (allChildrenBelowMinAlpha)
            {
                DestroyGlass();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Projetil") || (other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<Rigidbody>().velocity.magnitude >=
            other.gameObject.GetComponent<MovementTest2>().maxMoveSpeed * 2 / 3)) && !isBroken)
        {
            brokenGlass.SetActive(true);
            normalGlass.SetActive(false);
            isBroken = true;
            StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
        }
    }

    private IEnumerator DestroyGlassAfterDelay(float delay)
    {
        int vidroInteger = Random.Range(0, 2) + 1;
        SFXManager.Instance.PlaySFXRandomPitch("glass" + vidroInteger);
        yield return new WaitForSeconds(delay);
        DestroyGlass();
    }

    private void DestroyGlass()
    {
        Destroy(brokenGlass);
        Destroy(normalGlass);
        Destroy(gameObject);
    }
}