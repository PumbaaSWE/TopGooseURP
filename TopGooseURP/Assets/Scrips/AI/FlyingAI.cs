using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class FlyingAI : MonoBehaviour, IUtility
{
    private Rigidbody targetRigidbody; //to be dynamically allocated
    private Transform targetTransform; //to be dynamically allocated
    [Tooltip("Manually set a target for this AI")]
    [SerializeField] private GameObject targetGameObject; //Use target interface/sqript instead
    private bool movingTarget = false;

    [Tooltip("Will not chase beyond this distance")]
    [SerializeField] float maxInterceptDistance = 5000;
    [Tooltip("Will fire guns beyond this distance")]
    [SerializeField] float maxGunRange = 50;
    [Tooltip("Will try to tay on this distance form target")]
    [SerializeField] float preferredDistance = 10;

    //Boom and Zoom tactics
    [SerializeField]private float zoomAltitude = 10;
    [SerializeField]private float zoomAltitudeSafe = 3; //try stay X m above alt. to not loose it in turns
    [SerializeField]private float zoomBoxExtents = 3; //how close we are when staring to boom
    [SerializeField]private float climbRate = .5f; //half height per distance
    [SerializeField] private float maxCrank = 15; //max turning befor aborting
    bool zoomin = false; // if we aint zoomin' we boomin'

    [SerializeField][Range(0.0f, 1.0f)] float ramming = 0;
    [SerializeField][Range(0.0f, 1.0f)] float guns = 0;
    [SerializeField][Range(0.0f, 45.0f)] float gunsConeToFire = 1.0f;
    private float gunsAlignToFire;
    [SerializeField][Range(0.0f, 1.0f)] float minHeat = 0.3f;
    [SerializeField][Range(0.0f, 1.0f)] float maxHeat = 0.9f;
    private bool gunsOverheat;

    private Vector3 ramTarget; // vector to ram target
    private Vector3 gunSolutionTarget; // vector to get guns on target target
    private Vector3 flyTarget = Vector3.zero; // actual fly towards target
    [SerializeField] private float bulletSpeed = 800; //to be allocated from Guns attached later

    private Rigidbody selfRigidbody;
    private Autopilot autopilot;
    private FlightController controller;
    private Gun[] gunArray;

    public bool showDebugInfo;

    private Vector3 vecToTarget;
    private float distToTarget;

    void Start()
    {
        selfRigidbody = GetComponent<Rigidbody>();
        autopilot = GetComponent<Autopilot>();
        controller = GetComponent<FlightController>();
        controller.SetThrottleInput(1.0f);
        SetTarget(targetGameObject);
        gunArray = GetComponentsInChildren<Gun>();
        gunsAlignToFire = Mathf.Cos(gunsConeToFire * Mathf.Deg2Rad);

    }

    public void SetTarget(GameObject gameObject)
    {
        targetGameObject = gameObject;
        if (gameObject.TryGetComponent(out Rigidbody rigidbody))
        {
            targetRigidbody = rigidbody;
            movingTarget = true;
        }
        else
        {
            movingTarget = false;
        }
        targetTransform = gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void UpdateGuns()
    {
        if(gunArray == null) return;
        bool fire = false;

        //too far for guns
        if (distToTarget > maxGunRange) return;

        Vector3 fwd = transform.forward;
        Vector3 dirToAim = (flyTarget - transform.position).normalized;
        if (Vector3.Dot(fwd, dirToAim) > gunsAlignToFire && !gunsOverheat)
        {
            fire = true;
        }

        for (int i = 0; i < gunArray.Length; i++)
        {
            gunArray[i].Fire = fire;
            if (gunArray[i].Heat > maxHeat) gunsOverheat = true;
            else if (gunArray[i].Heat < minHeat) gunsOverheat = false;
        }
    }

    private void UpdateAutopilot()
    {
        autopilot.RunAutopilot(flyTarget, out float pitch, out float yaw, out float roll);
        Vector3 input = new(pitch, yaw, roll);
        controller.SetControlInput(input);
    }

    private void TrackTarget()
    {
        //return if no target
        if (targetGameObject == null || !targetGameObject.activeSelf) return;

        Vector3 tPos = targetTransform.position;
        Vector3 tVel = movingTarget?targetRigidbody.velocity:Vector3.zero;
        float pSpeed = controller.LocalVelocity.z;


        //similar heading? #Stop the hardcoding!
        //if(Vector3.Dot( transform.forward, targetTransform.forward) > .8f)
        //{
        //    float speedError = tVel.magnitude - pSpeed;

        //    //to match velovity and hold distance to target, can use much refinement to not be so choppy wth the throttle!
        //    controller.SetThrottleInput(controller.Throttle + speedError + distanceError);
        //}

        float distanceError = Mathf.Clamp((distToTarget - preferredDistance) / preferredDistance, 0.5f, 1f);
        controller.SetThrottleInput(distanceError);

        /*
         * Will blend between three aiming points, 
         * directly at target("dumb follow"), 
         * to cause collision(ram),
         * and to get a gun soulution(aim guns to hit a moving target)
         */
        flyTarget = tPos;
        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, pSpeed, out ramTarget, out float tti))
        {
            flyTarget = Vector3.Lerp(flyTarget, ramTarget, ramming);
            Debug.Assert(tti >= 0, "tti: " + tti);
        }
        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, bulletSpeed, out gunSolutionTarget, out float _))
        {
            flyTarget = Vector3.Lerp(flyTarget, gunSolutionTarget, guns);
        }

    }

    private void DoZoom(float dt)
    {
        //flyTarget
        //Compute where we want to go
        Vector3 desiredFlyTarget = targetTransform.position + Vector3.up * (zoomAltitude + zoomAltitudeSafe);
        flyTarget = desiredFlyTarget;

        //clamp climbrate
        if ((flyTarget.y - transform.position.y) > Mathf.Sin(climbRate))
        {
            flyTarget.Set(flyTarget.x, transform.position.y + Mathf.Sin(climbRate), flyTarget.z);
        }

        //find direction without rurning too hard
        flyTarget = Vector3.RotateTowards(transform.forward, flyTarget, maxCrank * Mathf.Deg2Rad, 0);

        if(transform.position.y > targetTransform.position.z + zoomAltitude)
        {
            //we are at the desired altitude
            //see if we are close enough "latitude"-wise... is that the right word?
            float x = transform.position.y;
            float y = transform.position.y;
            float z = transform.position.y;
            //if ()
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo == true)
        {
            Color oldColor = Gizmos.color;

           
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(flyTarget, 10f);
            

            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(gunSolutionTarget, 10f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ramTarget, 5f);


            Gizmos.color = oldColor;
        }
    }

    public float Evaluate()
    {
        if (!targetGameObject) return 0;

        vecToTarget = targetTransform.position - transform.position;
        distToTarget = vecToTarget.magnitude;


        return 1-((distToTarget+1) / maxInterceptDistance);
    }

    public void Execute()
    {
        TrackTarget();
        UpdateAutopilot();
        UpdateGuns();
        Debug.Log("Executing FlyingAI");
    }
}
