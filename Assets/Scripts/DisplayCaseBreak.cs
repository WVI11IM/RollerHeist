using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisplayCaseBreak : MonoBehaviour
{
    public GameObject normalDisplayCase;
    public GameObject brokenDisplayCase;
    public GameObject[] brokenGlasses;
    public bool isBroken = false;
    public float destructionDelay = 15f; // Time in seconds before both glass objects are destroyed
    public float fadeDuration = 30f; // Duration over which transparency will fade to zero
    public float minAlpha = 0.001f; // Minimum alpha before destroying the object

    public UnityEvent onBreakEvent;

    private List<Material> glassMaterials = new List<Material>();

    void Start()
    {
        brokenDisplayCase.SetActive(false);
        normalDisplayCase.SetActive(true);

        for (int i = 0; i < brokenGlasses.Length; i++)
        {
            // Get materials of all children of brokenGlass for transparency control
            foreach (Transform child in brokenGlasses[i].transform)
            {
                if (child.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    // Create a new material instance for each child to control individually
                    Material newMat = renderer.material;
                    glassMaterials.Add(newMat);
                }
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
        if ((other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<Rigidbody>().velocity.magnitude >= other.gameObject.GetComponent<MovementTest2>().maxMoveSpeed / 2) &&
            !isBroken)
        {
            brokenDisplayCase.SetActive(true);
            normalDisplayCase.SetActive(false);
            isBroken = true;
            StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
        }

        else if (other.gameObject.CompareTag("Projetil") &&
            !isBroken)
        {
            Debug.Log("entered projectile");
            if (other.GetComponent<Projetil>() != null)
            {
                other.GetComponent<Projetil>().ParticlesAndDestroy();
                brokenDisplayCase.SetActive(true);
                normalDisplayCase.SetActive(false);
                isBroken = true;
                StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Projetil") &&
            !isBroken)
        {
            Debug.Log("entered projectile");
            if (other.GetComponent<Projetil>() != null)
            {
                other.GetComponent<Projetil>().ParticlesAndDestroy();
                brokenDisplayCase.SetActive(true);
                normalDisplayCase.SetActive(false);
                isBroken = true;
                StartCoroutine(DestroyGlassAfterDelay(destructionDelay));
            }
        }
    }

    private IEnumerator DestroyGlassAfterDelay(float delay)
    {
        onBreakEvent.Invoke();
        int vidroInteger = Random.Range(0, 2) + 1;
        SFXManager.Instance.PlaySFXRandomPitch("glass" + vidroInteger);
        yield return new WaitForSeconds(delay);
        DestroyGlass();
    }

    private void DestroyGlass()
    {
        Destroy(brokenDisplayCase);
        Destroy(normalDisplayCase);
        Destroy(gameObject);
    }
}