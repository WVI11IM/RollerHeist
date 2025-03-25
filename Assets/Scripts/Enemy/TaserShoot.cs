using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaserShoot : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer laser;
    public GameObject taserBulletPrefab;
    public float bulletSpeed = 40f;
    public float laserDistance = 50f; // Maximum distance the laser can reach
    public LayerMask laserLayerMask;

    [SerializeField] private Enemy enemy;

    private bool isLaserActive = false;
    private float laserAnimationDuration = 1;
    private float laserTimer = 0f;

    void Start()
    {
        if (laser != null)
        {
            laser.positionCount = 2;
            laser.enabled = false;
        }
    }

    void Update()
    {
        if (!enemy.isAttacking)
        {
            isLaserActive = false;
            Laser(0);
        }

        if (isLaserActive)
        {
            CastLaser();
            UpdateLaserProperties();
        }
    }

    public void CastLaser()
    {
        RaycastHit hit;
        Vector3 laserEndPoint = firePoint.position + (firePoint.forward * laserDistance);

        // Perform the raycast with the specified layer mask
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, laserDistance, laserLayerMask))
        {
            laserEndPoint = hit.point;
        }

        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, laserEndPoint);
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(taserBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletSpeed;
    }

    public void Laser(int isActive)
    {
        if(isActive == 1)
        {
            isLaserActive = true;
            laserTimer = 0f;
        }
        else isLaserActive = false;

        if (laser != null)
        {
            if (isActive == 1)
            {
                CastLaser();
                laser.enabled = true;
            }
            else laser.enabled = false;
        }
    }

    private void UpdateLaserProperties()
    {
        if (laser == null) return;

        laserTimer += Time.deltaTime;
        float t = Mathf.Pow(Mathf.PingPong(laserTimer / laserAnimationDuration, 1f), 3);

        float laserWidth = Mathf.Lerp(0.5f, 0.1f, t);
        laser.startWidth = laserWidth;
        laser.endWidth = laserWidth;

        Color startColor = new Vector4 (1, 0, 0, 0.25f);
        Color endColor = new Vector4(1, 1, 1, 1);
        laser.startColor = Color.Lerp(startColor, endColor, t);
        laser.endColor = Color.Lerp(startColor, endColor, t);
    }
}