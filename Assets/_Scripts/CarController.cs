using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;
    public float brakeForce = 300f;
    public float speed = 600f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Transform _centerOfMass;

    public List<Wheel> wheels;

    private float moveInput;
    private float steerInput;

    private Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass.localPosition;
    }

    private void Update()
    {
        GetInputs();
        AnimateWheels();
    }

    private void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * speed * maxAcceleration * Time.deltaTime;
        }
    }

    private void Steer()
    {
        foreach (var wheel in wheels)
        {
            if(wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    private void Brake()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            foreach(var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = brakeAcceleration * brakeForce * Time.deltaTime;
            }

        }
        else
        {
            foreach(var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }
    }

    private void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;

            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

}
