using System;
using UnityEngine;

public class AvoidGroundUtility : MonoBehaviour, IUtility
{
    private Rigidbody rb;
    private Autopilot ap;
    private FlightController fc;

    private RaycastHit hit;

    [SerializeField]
    float sphereRadius = 2;
    [SerializeField]
    float searchDistance = 2;
    [SerializeField]
    float distanceFromGround = 2.5f;
    [SerializeField]
    float upFromGround = 1.0f;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    private bool useAdvancedAutopilot;

    private float currentHitDistance, maxHitDistance;

    private float speed;
    private Vector3 direction;

    private Vector3 target, tangent; //class scoped here because debug thingy
    private bool groundStrike; // debug stuff


    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        ap = GetComponent<Autopilot>();
        fc = GetComponent<FlightController>();
    }

    public float Evaluate()
    {
        speed = rb.velocity.magnitude;
        direction = rb.velocity.normalized;
        maxHitDistance = searchDistance * speed;
        
        if(Physics.SphereCast(rb.position, sphereRadius, direction, out hit, maxHitDistance, layerMask))
        {
            //Debug.Log("Spherecast");
            //currentHitDistance = hit.distance;
            maxHitDistance = distanceFromGround;
            currentHitDistance = 0;// hit.distance - distanceFromGround;
            groundStrike = true;
            return 2f;
        }
        if (Physics.Raycast(rb.position, Vector3.down, out hit, distanceFromGround, layerMask))
        {
            //Debug.Log("Secondary Spherecast");
            maxHitDistance = distanceFromGround;
            currentHitDistance = 0;// hit.distance - distanceFromGround;
            //rb.velocity += Vector3.up;
            //rb.AddForce(Vector3.up, ForceMode.VelocityChange);
            groundStrike = true;
            return 2f;
        }

        currentHitDistance = maxHitDistance;
        groundStrike = false;
        return 0f;
        
    }

    public void Exit() { }

    public void Execute()
    {

        ////Closer to the ground -> Lower throttle
        //fc.SetThrottleInput(Math.Clamp(currentHitDistance * 2 / maxDistance, 0.25f, 1));

        //Debug.Log(Vector3.Dot(rb.velocity, hit.normal));
        //if (Vector3.Dot(rb.velocity, hit.normal) < -0.75f)
        //{
        //    //Debug.Log($"hit: {currentHitDistance}");
        //    //Debug.Log($"throttle: {fc.Throttle}");

        //    fc.SetThrottleInput(Math.Clamp(currentHitDistance / maxHitDistance, 0.5f, 1));
        //    //Debug.Log($"--------------------------------------NEW THROTTLE: {fc.Throttle}");
        //    //Debug.Log(Vector3.Dot(rb.velocity, hit.normal));
        //}

        //Vector3 oldTarget = rb.position + rb.velocity.normalized * currentHitDistance;
        /*Vector3*/ tangent = Vector3.Cross(hit.normal, Vector3.Cross(direction, hit.normal));

        /*Vector3*/ target = hit.point + Vector3.up * upFromGround + hit.normal * sphereRadius + tangent * (maxHitDistance - currentHitDistance);
        // Vector3.S
        //Debug.DrawLine(newTarget, transform.position, Color.green);

        ap.RunAutopilot2(target, out float pitch, out float yaw, out float roll);
        fc.SetControlInput(new Vector3(pitch, yaw, roll));

        //Debug.Log("Executing AvoidGroundUtil " + new Vector3(pitch, yaw, roll));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        try
        {
            if (groundStrike)
            {
                Gizmos.DrawLine(transform.position, hit.point + hit.normal * distanceFromGround);
                Gizmos.DrawLine(hit.point + hit.normal * distanceFromGround, target);
                Gizmos.DrawWireSphere(target, sphereRadius);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hit.point, hit.point + tangent);
            }
        }
        catch (NullReferenceException)
        {

        }
        catch (UnassignedReferenceException)
        {

        }
    }
}
