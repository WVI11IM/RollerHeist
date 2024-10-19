using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLevelFeedback : MonoBehaviour
{
    public LayerMask groundLayer;
    private float raycastDistance = 28f;
    public float minDistance = 2.75f;
    public Transform floorLevelIcon;

    public float distanceToFloor;

    private void Update()
    {

        Vector3 localDown = Vector3.down; // Local downward direction
        Vector3 worldDown = transform.TransformDirection(localDown); // Transform to world space
        // Cast a ray downwards from the player's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, worldDown, out hit, raycastDistance, groundLayer))
        {
            float distanceToGround = hit.distance;
            distanceToFloor = distanceToGround;
            if (distanceToGround > minDistance)
            {
                floorLevelIcon.gameObject.SetActive(true);
                floorLevelIcon.localPosition = new Vector3(0, -distanceToGround + 0.1f, 0);
            }
            else
            {
                floorLevelIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            floorLevelIcon.gameObject.SetActive(false);
        }
    }
}
