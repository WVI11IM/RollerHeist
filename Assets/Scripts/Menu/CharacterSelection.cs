using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{   

    public SkinnedMeshRenderer[] sMRs;
    public Material[] characterSkins;
    public int selectedCharacter = 0;

    void Start()
    {
      selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);
      ChangePalette(selectedCharacter);
    }
     
    public void ChangePalette(int index)
    {
        if (index >= 0 && index < characterSkins.Length)
        {
            for (int i = 0; i < sMRs.Length; i++)
            {
            sMRs[i].material = characterSkins[index];
            }
            PlayerPrefs.SetInt("selectedCharacter",index);
            selectedCharacter = index;
        }
        else
        {
            Debug.LogWarning("SwitchPalette: Índice de paleta inválido: " + index);
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        Debug.Log("CharacterSelection: Jogo iniciado com personagem selecionado e paleta alterada.");
    }
}