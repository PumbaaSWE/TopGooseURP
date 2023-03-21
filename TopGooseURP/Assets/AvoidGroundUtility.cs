using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AvoidGroundUtility : MonoBehaviour, IUtility
{
    Rigidbody rb;
    FlightController fc;
    Autopilot ap;

    Vector3 futurePos;

    public float Evaluate()
    {
        rb = GetComponent<Rigidbody>();

        futurePos = rb.position + rb.velocity;

        float sphereRadius = 4;
        Debug.DrawLine(futurePos, rb.position, Color.magenta);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x, futurePos.y + sphereRadius, futurePos.z), Color.white);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x, futurePos.y - sphereRadius, futurePos.z), Color.white);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x - sphereRadius, futurePos.y, futurePos.z), Color.white);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x + sphereRadius, futurePos.y, futurePos.z), Color.white);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x, futurePos.y, futurePos.z - sphereRadius), Color.white);
        Debug.DrawLine(futurePos, new Vector3(futurePos.x, futurePos.y, futurePos.z + sphereRadius), Color.white);

        if (Physics.CheckSphere(futurePos, sphereRadius))
            return 2f;

        return 0f;
    }

    public void Execute()
    {
        ap = GetComponent<Autopilot>();
        fc = GetComponent<FlightController>();

        Vector3 newTarget = rb.position + new Vector3(rb.velocity.x, -rb.velocity.y, rb.velocity.z);
        Debug.DrawLine(newTarget, rb.position, Color.green);
        ap.RunAutopilot(new Vector3(futurePos.x, futurePos.y + 2, futurePos.z), out float pitch, out float yaw, out float roll);
        fc.SetControlInput(new Vector3(pitch, yaw, roll));

        Debug.Log("Executing AvoidGroundUtil");
    }
}
