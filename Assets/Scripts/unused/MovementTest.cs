using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float moveSpeed = 5;

    public float rotSpeed = 5;
    private float rot;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");

        rot += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);

        Vector3 direction = new Vector3(0, 0, vertical);

        direction = transform.TransformDirection(direction);
        rb.AddForce(direction * moveSpeed);
    }
}
