using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float MotorForce, SteerForce, BrakeForce;

    public WheelCollider Wheel_FR_L, Wheel_FR_R, RE_L, RE_R;

    void Update()
    {
        float v = Input.GetAxis("Vertical") * MotorForce;
        float h = Input.GetAxis("Horizontal") * SteerForce;

        Wheel_FR_L.motorTorque = v;
        Wheel_FR_R.motorTorque = v;

        RE_L.steerAngle = h;
        RE_R.steerAngle = h;


        //Freiar o personagem
        if (Input.GetKey(KeyCode.Space))
        {
            RE_L.brakeTorque = BrakeForce;
            RE_R.brakeTorque = BrakeForce;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RE_L.brakeTorque = 0;
            RE_R.brakeTorque = 0;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            RE_L.brakeTorque = BrakeForce;
            RE_R.brakeTorque = BrakeForce;
        }
        else
        {
            RE_L.brakeTorque = 0;
            RE_R.brakeTorque = 0;
        }
    }
}
