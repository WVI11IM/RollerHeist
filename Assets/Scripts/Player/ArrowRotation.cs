using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    // Reference to the player's transform
    public Transform playerTransform;

    // Variable to store the previous player rotation
    private Quaternion previousPlayerRotation;

    void Start()
    {
        // Initialize previousPlayerRotation with the initial player rotation
        if (playerTransform != null)
        {
            previousPlayerRotation = playerTransform.rotation;
        }
    }

    void Update()
    {
        // Check if playerTransform is assigned
        if (playerTransform != null)
        {
            // Calculate the rotation difference between previous and current frames
            Quaternion rotationDifference = playerTransform.rotation * Quaternion.Inverse(previousPlayerRotation);

            // Convert the rotation difference to an angle
            float angle = Quaternion.Angle(Quaternion.identity, rotationDifference);

            // Update previousPlayerRotation for the next frame
            previousPlayerRotation = playerTransform.rotation;

            transform.localRotation = rotationDifference;
        }
    }
}