using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenShow : MonoBehaviour
{
    public int levelNumber;
    public int missionNumber;
    Image image;

    public Sprite notComplete;
    public Sprite complete;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();

        if (PlayerPrefs.GetInt("Level" + levelNumber + "Mission" + missionNumber) == 1)
        {
            image.sprite = complete;
        }
        else image.sprite = notComplete;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
