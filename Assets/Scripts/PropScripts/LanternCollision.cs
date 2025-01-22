using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternCollision : MonoBehaviour
{
    public Material unlitLantern;
    public MeshRenderer meshRenderer;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            meshRenderer.material = unlitLantern;
        }
    }
}
