using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPalette : MonoBehaviour
{
    SkinnedMeshRenderer sMR;
    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        sMR = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            sMR.material = materials[0];
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            sMR.material = materials[1];
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            sMR.material = materials[2];
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            sMR.material = materials[3];
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            sMR.material = materials[4];
        }
    }
}
