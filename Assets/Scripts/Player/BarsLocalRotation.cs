using UnityEngine;

public class BarsLocalRotation : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // Get the parent's rotation
        Quaternion parentRotation = transform.parent.rotation;

        // Apply the inverse of the parent's rotation to the Canvas
        rectTransform.localRotation = Quaternion.Inverse(parentRotation);
    }
}

