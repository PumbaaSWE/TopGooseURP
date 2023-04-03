using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FlappyWings : MonoBehaviour
{
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private float flapSpeed = 45;
    [SerializeField] private float maxFlap = 90;


    [SerializeField] private SimpleFlight controller;
    [SerializeField] private Transform tail;
    [SerializeField] private float strengthPitch = 45;
    [SerializeField] private float strengthRoll = 1;

    bool flap;
    float angle;

    internal void Flap(bool flap)
    {
        this.flap = flap;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out controller))
        {
            //do smth
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.LocalVelocity.z <= 0) return;
        angle += flapSpeed * Time.deltaTime;

        if (angle > maxFlap / 2) 
        {
            flapSpeed = -flapSpeed; 
            angle = maxFlap / 2;
        }

        if (angle < -maxFlap / 2)
        {
            flapSpeed = -flapSpeed;
            angle = -maxFlap / 2;
        }
        left.localEulerAngles = new Vector3 (0,0, angle);
        right.localEulerAngles = new Vector3(0, 0, -angle);

        float pitch = controller.Steering.x * strengthPitch;
        float roll = controller.Steering.z * strengthRoll;

        Vector3 steer = new(pitch, 0, roll);
        tail.localEulerAngles = steer;
    }
}
