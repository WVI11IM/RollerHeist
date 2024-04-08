using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionFollow : MonoBehaviour
{
    public Transform playerPosition;

    void Update()
    {
        float posX = playerPosition.transform.position.x + 20;
        float posY = playerPosition.transform.position.y + 20f;
        float posZ = playerPosition.transform.position.z + -20;

        transform.position = new Vector3(posX, posY, posZ);
    }
}
