using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    private void Awake()
    {
        TargetsController ui = GetComponentInParent<TargetsController>();
        if (ui == null)
        {
            ui = GameObject.Find("Canvas").GetComponent<TargetsController>();
        }

        if (ui == null) Debug.LogError("No UIController component found");

        ui.AddTargetIndicator(this.gameObject);
    }
}
