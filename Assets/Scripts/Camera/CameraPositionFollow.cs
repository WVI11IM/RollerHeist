using UnityEngine;

public class CameraPositionFollow : MonoBehaviour
{
    public Transform playerPosition;
    public float smoothSpeed = 0.125f; // Adjust the smoothness factor
    private Vector3 initialCameraPosition;

    private void Start()
    {
        initialCameraPosition = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        // Calculate the desired position with an offset
        Vector3 desiredPosition = playerPosition.position + initialCameraPosition;

        // Smoothly move the camera towards the desired position using damping
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}