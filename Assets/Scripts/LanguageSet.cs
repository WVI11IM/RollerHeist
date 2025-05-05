using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageSet : MonoBehaviour
{
    public Language language;

    void Awake()
    {

        switch (language)
        {
            case Language.PT: //Português
                PlayerPrefs.SetInt("languageInt", 0);
                break;
            case Language.EN: //Inglês
                PlayerPrefs.SetInt("languageInt", 1);
                break;
        }
    }

    [System.Serializable]
    public enum Language
    {
        PT,
        EN
    }
}
