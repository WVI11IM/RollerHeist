using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class UITransitionManager : MonoBehaviour
{
    [HideInInspector] public CinemachineVirtualCamera currentCamera;
  

    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        if(currentCamera != null) currentCamera.Priority--;
        currentCamera = target;
        currentCamera.Priority++;
    }
}

