using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLanguageChange : MonoBehaviour
{
    private TextMeshProUGUI textToTranslate;
    [TextArea(2, 50)]
    [SerializeField] string textPT;
    [TextArea(2, 50)]
    [SerializeField] string textEN;

    void Start()
    {
        textToTranslate = GetComponent<TextMeshProUGUI>();

        switch (PlayerPrefs.GetInt("languageInt"))
        {
            case 0: //Portugu�s
                textToTranslate.text = textPT;
                break;
            case 1: //Ingl�s
                textToTranslate.text = textEN;
                break;
        }
    }
}
