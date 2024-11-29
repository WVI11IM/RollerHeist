using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncreaseScaleWithInput : MonoBehaviour
{
    public TextMeshPro floorText;
    Vector3 normalScale;
    public Color normalColor;
    Vector3 biggerScale;
    public Color biggerColor;
    public KeyCode keyCode;

    void Start()
    {
        normalScale = transform.localScale;
        biggerScale = normalScale * 1.1f;
    }

    // Update is called once per frame
    void Update()
    {
        //32 for space
        //97 for A
        //100 for D

        if (Input.GetKey(keyCode))
        {
            transform.localScale = biggerScale;
            floorText.color = biggerColor;
        }
        else
        {
            transform.localScale = normalScale;
            floorText.color = normalColor;
        }
    }
}
