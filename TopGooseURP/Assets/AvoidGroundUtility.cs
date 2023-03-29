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
            Debug.Log("Spherecast");
            currentHitDistance = hit.distance;
            return 2f;
        }
        else if (Physics.Raycast(rb.position, Vector3.down, out hit, sphereRadius, layerMask))
        {
            Debug.Log("Raycast down");
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
        

        Vector3 newTarget = rb.position + Vector3.Cross(hit.normal, Vector3.Cross(rb.velocity, hit.normal)) + hit.normal * 2;
        Debug.DrawLine(newTarget, rb.position, Color.green);

        Random rand = new Random();

        if (Physics.SphereCast(rb.position, sphereRadius, newTarget, out hit, Vector3.Distance(rb.position, newTarget)) || rand.Next(2) < 1)
        {
            Vector3[] potentialTargets = new Vector3[4];

            potentialTargets[0] = rb.position + (rb.transform.forward * 2 + rb.transform.right) / 3 * rb.velocity.magnitude;
            potentialTargets[1] = rb.position + (rb.transform.forward * 2 - rb.transform.right) / 3 * rb.velocity.magnitude;
            potentialTargets[2] = rb.position + (rb.transform.forward * 2 + rb.transform.up) / 3 * rb.velocity.magnitude;
            potentialTargets[3] = rb.position + (rb.transform.forward * 2 - rb.transform.up) / 3 * rb.velocity.magnitude;

            for (int i = 0; i < potentialTargets.Length; i++)
            {
                Debug.DrawLine(potentialTargets[i], rb.position, Color.yellow);

                // Räcker inte med raycast, testa spherecast och kanske overlapsphere ifall spherecastdistans är 0
                if (Physics.Raycast(rb.position, potentialTargets[i], Vector3.Distance(rb.position, potentialTargets[i])))
                    newTarget = potentialTargets[i];

                Debug.Log($"Chose secondary target #{i}");
            }
        }
        else
            Debug.Log("Chose primary target");

        ap.RunAutopilot(newTarget, out float pitch, out float yaw, out float roll);
        fc.SetControlInput(new Vector3(pitch, yaw, roll));

        Debug.Log("Executing AvoidGroundUtil");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Debug.DrawLine(rb.position, rb.position + rb.velocity.normalized * currentHitDistance, Color.magenta);
        Gizmos.DrawWireSphere(rb.position + rb.velocity.normalized * currentHitDistance, sphereRadius);
    }
}
