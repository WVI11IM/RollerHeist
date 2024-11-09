using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedalUnlock : MonoBehaviour
{
    public MainMenu mainMenu;
    public int minimumMedals;
    public Button[] buttonsToLock;
    public TextMeshProUGUI minMedalsText;

    void Start()
    {
        minMedalsText.text = minimumMedals.ToString();

        if (mainMenu.levelMedals < minimumMedals)
        {
            for(int i = 0; i < buttonsToLock.Length; i++)
            {
                buttonsToLock[i].interactable = false;
            }
            gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < buttonsToLock.Length; i++)
            {
                buttonsToLock[i].interactable = true;
            }
            gameObject.SetActive(false);
        }
    }
}
