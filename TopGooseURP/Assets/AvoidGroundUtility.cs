using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class AvoidGroundUtility : MonoBehaviour, IUtility
{
    Rigidbody rb;
    Autopilot ap;
    FlightController fc;

    RaycastHit hit;

    [SerializeField]
    float sphereRadius;
    [SerializeField]
    float searchDistance;
    [SerializeField]
    float distanceFromGround;
    [SerializeField]
    LayerMask layerMask;

    float currentHitDistance, maxHitDistance;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        ap = GetComponent<Autopilot>();
        fc = GetComponent<FlightController>();
    }

    public float Evaluate()
    {
        maxHitDistance = searchDistance * rb.velocity.magnitude / 17 /* Get speed drag limit from fc instead of 17? */;

        if (Physics.SphereCast(rb.position, sphereRadius, rb.velocity.normalized, out hit, maxHitDistance, layerMask))
        {
            Debug.Log("Spherecast");
            currentHitDistance = hit.distance;
            return 2f;
        }
        else if (Physics.SphereCast(rb.position - rb.velocity.normalized * sphereRadius * 2, sphereRadius, rb.velocity.normalized, out hit, maxHitDistance, layerMask))
        {
            Debug.Log("Secondary Spherecast");
            currentHitDistance = hit.distance - sphereRadius;
            rb.velocity -= Vector3.down;
            return 2f;
        }
        else
        {
            currentHitDistance = maxHitDistance;
            return 0f;
        }
    }

    public void Execute()
    {

        ////Closer to the ground -> Lower throttle
        //fc.SetThrottleInput(Math.Clamp(currentHitDistance * 2 / maxDistance, 0.25f, 1));

        Debug.Log(Vector3.Dot(rb.velocity, hit.normal));
        if (Vector3.Dot(rb.velocity, hit.normal) < -0.75f)
        {
            //Debug.Log($"hit: {currentHitDistance}");
            //Debug.Log($"throttle: {fc.Throttle}");

            fc.SetThrottleInput(Math.Clamp(currentHitDistance / maxHitDistance, 0.5f, 1));
            Debug.Log($"--------------------------------------NEW THROTTLE: {fc.Throttle}");
            //Debug.Log(Vector3.Dot(rb.velocity, hit.normal));
        }

        Vector3 oldTarget = rb.position + rb.velocity.normalized * currentHitDistance;

        Vector3 newTarget = oldTarget + Vector3.Cross(hit.normal, Vector3.Cross(rb.velocity, hit.normal)).normalized * (maxHitDistance - currentHitDistance) + hit.normal * distanceFromGround;

        Debug.DrawLine(newTarget, oldTarget, Color.green);

        ap.RunAutopilot(newTarget, out float pitch, out float yaw, out float roll);
        fc.SetControlInput(new Vector3(pitch, yaw, roll));

        Debug.Log("Executing AvoidGroundUtil");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        try
        {
            Debug.DrawLine(rb.position, rb.position + rb.velocity.normalized * currentHitDistance, Color.magenta);
            Gizmos.DrawWireSphere(rb.position + rb.velocity.normalized * currentHitDistance, sphereRadius);
        }
        catch (NullReferenceException)
        {

        }
        catch (UnassignedReferenceException)
        {

        }
    }
}
