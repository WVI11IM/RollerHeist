using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallcopWaterParticles : MonoBehaviour
{
    public ParticleSystem[] waterParticles;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            foreach (ParticleSystem particle in waterParticles)
            {
                var waterEmissionModule = particle.emission;
                waterEmissionModule.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            foreach (ParticleSystem particle in waterParticles)
            {
                var waterEmissionModule = particle.emission;
                waterEmissionModule.enabled = false;
            }
        }
    }
}