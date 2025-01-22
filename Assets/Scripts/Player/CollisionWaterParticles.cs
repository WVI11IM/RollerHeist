using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionWaterParticles : MonoBehaviour
{
    public ParticleSystem waterParticle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
            waterParticle.Play();
    }
}
