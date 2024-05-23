using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    public bool onRail;
    [SerializeField] float grindSpeed;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;

    [Header("Scripts")]
    [HideInInspector] RailScript currentRailScript;
    Rigidbody playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (onRail)
        {
            MovePlayerAlongRail();
        }
    }

    private void Update()
    {
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail)
        {
            float progress = elapsedTime / timeForFullSpline;

            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }

            float nextTimeNormalised;
            if (currentRailScript.normalDir)
                nextTimeNormalised = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalised = (elapsedTime - Time.deltaTime) / timeForFullSpline;

            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            transform.position = worldPos + (transform.up * heightOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

            if (currentRailScript.normalDir)
                elapsedTime += Time.deltaTime;
            else
                elapsedTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rail"))
        {
            onRail = true;
            currentRailScript = collision.gameObject.GetComponent<RailScript>();

            if (currentRailScript == null)
            {
                Debug.LogError("PlayerGrind: No RailScript found on collided object " + collision.gameObject.name);
                onRail = false;
                return;
            }

            CalculateAndSetRailPosition();
        }
    }

    void CalculateAndSetRailPosition()
    {
        if (currentRailScript == null)
        {
            Debug.LogError("PlayerGrind: currentRailScript is null in CalculateAndSetRailPosition.");
            return;
        }

        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;
        Vector3 splinePoint;
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        currentRailScript.CalculateDirection(forward, transform.forward);
        transform.position = splinePoint + (transform.up * heightOffset);
    }

    void ThrowOffRail()
    {
        onRail = false;
        currentRailScript = null;
        transform.position += transform.forward * 1;
    }
}