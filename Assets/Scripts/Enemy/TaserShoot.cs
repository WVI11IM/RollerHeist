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
}