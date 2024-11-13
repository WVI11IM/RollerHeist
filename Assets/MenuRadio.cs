using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRadio : MonoBehaviour
{
    Animator animator;
    public MainMenu menuManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("volumeIsZero", true);
    }

    void Update()
    {
        if(menuManager.menuState == MenuState.Audio)
        {
            if(PlayerPrefs.GetFloat("MasterVolume") * PlayerPrefs.GetFloat("MusicVolume") < 0.01f)
            {
                animator.SetBool("volumeIsZero", true);
            }
            else animator.SetBool("volumeIsZero", false);
        }
        else animator.SetBool("volumeIsZero", true);
    }
}
