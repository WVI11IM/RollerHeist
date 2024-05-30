using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPalette : MonoBehaviour
{
    public Material[] materials;

    SkinnedMeshRenderer sMR;

    void Start()
    {
        sMR = GetComponent<SkinnedMeshRenderer>();
        Debug.Log("SwitchPalette: SkinnedMeshRenderer atribuído corretamente.");
        
        int index = PlayerPrefs.GetInt("selectedCharacter");
        ChangePalette(index);
    }

    public void ChangePalette(int index)
    {
        if (index >= 0 && index < materials.Length)
        {
            sMR.material = materials[index];
            Debug.Log("SwitchPalette: Paleta alterada para índice " + index);
        }
        else
        {
            Debug.LogWarning("SwitchPalette: Índice de paleta inválido: " + index);
        }
    }
}