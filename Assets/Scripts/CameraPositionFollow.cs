using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionFollow : MonoBehaviour
{
    public Transform playerPosition;

    void Update()
    {
        float posX = playerPosition.transform.position.x + 50;
        float posY = playerPosition.transform.position.y + 45f;
        float posZ = playerPosition.transform.position.z + -50;

        transform.position = new Vector3(posX, posY, posZ);
    }
}
