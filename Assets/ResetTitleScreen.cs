using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("titleScreenActivated") == 1)
        {
            PlayerPrefs.SetInt("titleScreenActivated", 0);
        }
    }
}
