using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    float angle;

    // Start is called before the first frame update
    void Start()
    {
         if(TryGetComponent<SimpleFlight>(out controller))
        {
            //do smth
        }
    }

    // Update is called once per frame
    void Update()
    {
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
