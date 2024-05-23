using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailScript : MonoBehaviour
{
    public bool normalDir;
    public SplineContainer railSpline;
    public float totalSplineLength;

    private void Awake()
    {
        // Initialize railSpline in Awake
        railSpline = GetComponent<SplineContainer>();
        if (railSpline == null)
        {
            Debug.LogError("RailScript: SplineContainer component not found on " + gameObject.name);
        }
        else
        {
            totalSplineLength = railSpline.CalculateLength();
            Debug.Log("RailScript: Total spline length calculated as " + totalSplineLength);
        }
    }

    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        return transform.TransformPoint(localPoint);
    }

    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        return transform.InverseTransformPoint(worldPoint);
    }

    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        worldPosOnSpline = Vector3.zero; // Initialize out parameter

        if (railSpline == null)
        {
            Debug.LogError("RailScript: railSpline is null when trying to calculate target rail point.");
            return 0f;
        }

        float3 nearestPoint;
        float time;
        SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(playerPos), out nearestPoint, out time);
        worldPosOnSpline = LocalToWorldConversion(nearestPoint);
        return time;
    }

    public void CalculateDirection(float3 railForward, Vector3 playerForward)
    {
        float angle = Vector3.Angle(railForward, playerForward.normalized);
        normalDir = angle <= 90f;
    }
}