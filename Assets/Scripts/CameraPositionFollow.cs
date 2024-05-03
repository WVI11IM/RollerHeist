using UnityEngine;

public class CameraPositionFollow : MonoBehaviour
{
    public Transform playerPosition;
    public float smoothSpeed = 0.125f; // Adjust the smoothness factor

    void FixedUpdate()
    {
        // Calculate the desired position with an offset
        Vector3 desiredPosition = playerPosition.position + new Vector3(25f, 25f, -25f);

        // Smoothly move the camera towards the desired position using damping
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}