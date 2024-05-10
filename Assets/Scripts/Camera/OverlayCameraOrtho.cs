using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayCameraOrtho : MonoBehaviour
{
    Camera mainCamera;
    Camera overlayCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        overlayCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        overlayCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
