using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] charactersSitting;
    public int selectedCharacter = 0;

    void Start()
    {
      selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);
      characters[selectedCharacter].SetActive(true);
      charactersSitting[selectedCharacter].SetActive(true);
      gameObject.SetActive(false);
    }

    public void NextCharacter()
    {
        charactersSitting[selectedCharacter].SetActive(false);
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
        charactersSitting[selectedCharacter].SetActive(true);
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
    }

    public void PreviousCharacter()
    {
        
        characters[selectedCharacter].SetActive(false);
        charactersSitting[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
        charactersSitting[selectedCharacter].SetActive(true);
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        Debug.Log("CharacterSelection: Jogo iniciado com personagem selecionado e paleta alterada.");
    }
}