using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenUnlock : MonoBehaviour
{
    public MainMenu mainMenu;
    public int minimumTokens;
    public Button[] buttonsToLock;
    public TextMeshProUGUI minTokensText;

    void Start()
    {
        minTokensText.text = minimumTokens.ToString();

        if (mainMenu.missionTokens < minimumTokens)
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
