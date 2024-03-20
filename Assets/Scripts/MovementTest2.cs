using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest2 : MonoBehaviour
{
    public float maxMoveSpeed = 5;
    public float moveSpeed;

    public float maxRotSpeed = 5;
    public float rotSpeed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
        //if (rotSpeed > maxRotSpeed) rotSpeed = maxRotSpeed;

        float vertical = Input.GetAxis("Vertical");

        rotSpeed += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotSpeed, 0);

        Vector3 direction = new Vector3(0, 0, vertical);

        direction = transform.TransformDirection(direction);
        rb.AddForce(direction * moveSpeed);

        Debug.Log("Horizontal: " + Input.GetAxis("Horizontal"));
        Debug.Log("Vertical: " + Input.GetAxis("Vertical"));
    }
}
