using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AvoidGroundUtility : MonoBehaviour, IUtility
{
    Rigidbody rb;
    Autopilot ap;
    FlightController fc;

    RaycastHit hit;

    [SerializeField]
    float sphereRadius;
    [SerializeField]
    float maxDistance;
    [SerializeField]
    LayerMask layerMask;

    float currentHitDistance;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        ap = GetComponent<Autopilot>();
        fc = GetComponent<FlightController>();
    }

    public float Evaluate()
    {
        if (Physics.SphereCast(rb.position, sphereRadius, rb.velocity.normalized, out hit, maxDistance, layerMask))
        {
            //Debug.Log("Spherecast");
            currentHitDistance = hit.distance;
            return 2f;
        }
        else if (Physics.Raycast(rb.position, Vector3.down, out hit, sphereRadius, layerMask))
        {
            //Debug.Log("Raycast down");
            currentHitDistance = 0;
            rb.velocity -= Vector3.down;
            return 2f;
        }
        else
        {
            currentHitDistance = maxDistance;
            return 0f;
        }
    }

    public void Execute()
    {
        Vector3 newTarget = rb.position + Vector3.Cross(hit.normal, Vector3.Cross(rb.velocity, hit.normal)) * 2;

        Debug.DrawLine(newTarget, rb.position, Color.green);

        ap.RunAutopilot(newTarget, out float pitch, out float yaw, out float roll);
        fc.SetControlInput(new Vector3(pitch, yaw, roll));

        //Debug.Log("Executing AvoidGroundUtil");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Debug.DrawLine(rb.position, rb.position + rb.velocity.normalized * currentHitDistance, Color.magenta);
        Gizmos.DrawWireSphere(rb.position + rb.velocity.normalized * currentHitDistance, sphereRadius);
    }
}
