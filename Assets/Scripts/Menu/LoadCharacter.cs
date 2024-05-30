using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs; // Prefabs dos personagens
    public Transform spawnPoint; // Ponto de spawn do personagem

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);
        if (selectedCharacter >= 0 && selectedCharacter < characterPrefabs.Length)
        {
            GameObject prefab = characterPrefabs[selectedCharacter];
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation); // Instancia no ponto de spawn definido
        }
        else
        {
            Debug.LogError("Invalid character selected: " + selectedCharacter);
        }
    }
}
