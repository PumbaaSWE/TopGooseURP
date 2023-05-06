using System;
using UnityEngine;

public class FlyingAI : MonoBehaviour, IUtility
{
    private Rigidbody targetRigidbody; //to be dynamically allocated
    private Transform targetTransform; //to be dynamically allocated
    [Tooltip("Manually set a target for this AI")]
    [SerializeField] private GameObject targetGameObject; //Use target interface/sqript instead
    private bool movingTarget = false;
    [Space]
    [Tooltip("Will not chase beyond this distance")]
    [SerializeField] float maxInterceptDistance = 1000;
    [Tooltip("Will fire guns beyond this distance")]
    [SerializeField] float maxGunRange = 50;
    [Tooltip("Will try to tay on this distance form target")]
    [SerializeField] float preferredDistance = 10;
    [Space]
    [Header("Booming and Zooming")]
    //Boom and Zoom tactics
    [SerializeField]private float zoomAltitude = 25;
    [SerializeField]private float zoomAltitudeSafe = 3; //try stay X m above alt. to not loose it in turns
    [SerializeField]private float zoomBoxExtents = 3; //how close we are when staring to boom
    [SerializeField] private float zoomDirection = 45; //how close we are when staring to boom
    [SerializeField]private float climbRate = .5f; //half height per distance
    //[SerializeField] private float maxCrank = 15; //max turning befor aborting
    [SerializeField] private float boomFireDist = 5; //when to start fireing
    //[SerializeField] private float boomCrank = 25; //when to start fireing
    private bool zoomin = false; // if we aint zoomin' we boomin'


    
    [Space]
    [Header("Ground Avoiding")]
    [SerializeField] private float radius = 3;
    [SerializeField] private float bumbModifier = 2;
    [SerializeField] private float rangeModifier = 2;
    [SerializeField] private LayerMask groundLayer;

    [Space]
    [Header("Shooting")]
    [SerializeField][Range(0.0f, 1.0f)] float ramming = 0;
    [SerializeField][Range(0.0f, 1.0f)] float guns = 0;
    [SerializeField][Range(0.0f, 45.0f)] float gunsConeToFire = 1.0f;
    private float gunsAlignToFire;
    [SerializeField][Range(0.0f, 1.0f)] float minHeat = 0.3f;
    [SerializeField][Range(0.0f, 1.0f)] float maxHeat = 0.9f;
    private bool gunsOverheat;
    [SerializeField] private LayerMask targetLayer;
    private Vector3 ramTarget; // vector to ram target
    private Vector3 gunSolutionTarget; // vector to get guns on target target
    private Vector3 flyTarget = Vector3.zero; // actual fly towards target
    [SerializeField] private float bulletSpeed = 800; //to be allocated from Guns attached later


    private Rigidbody selfRigidbody;
    private Autopilot autopilot;
    private FlightController controller;
    private AIActor actor;
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
        if(targetGameObject != null)
            SetTarget(targetGameObject);
        gunArray = GetComponentsInChildren<Gun>();
        gunsAlignToFire = Mathf.Cos(gunsConeToFire * Mathf.Deg2Rad);
        actor = GetComponent<AIActor>();
        if(gunArray != null && gunArray.Length > 0)
            bulletSpeed = gunArray[0].BulletSpeed;

    }
    private void OnValidate()
    {
        gunsAlignToFire = Mathf.Cos(gunsConeToFire * Mathf.Deg2Rad);
        //bulletSpeed = gunArray[0].BulletSpeed;
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
        Vector3 dirToAim = (flyTarget - transform.position).normalized; //should not be flyTarget as it's not always in gun solution

        Vector3 tPos = targetTransform.position;
        Vector3 tVel = movingTarget ? targetRigidbody.velocity : Vector3.zero;
        //if (TargetingMath.ComputeVector(tPos, tVel, transform.position, bulletSpeed, out Vector3 vector))
        //{
        //    if (Vector3.Dot(fwd, vector.normalized) > gunsAlignToFire && !gunsOverheat)
        //    {
        //        fire = true;
        //    }
        //    Debug.DrawRay(transform.position, vector);
        //}

        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, bulletSpeed, out Vector3 impact, out float _))
        {
            dirToAim = (impact - transform.position).normalized;
            if (Vector3.Dot(fwd, dirToAim) > gunsAlignToFire && !gunsOverheat)
            {
                fire = true;
            }
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
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, controller.LocalVelocity.z * rangeModifier, groundLayer))
        {
            flyTarget = transform.position + transform.forward + (hit.normal+Vector3.up) * bumbModifier;
        }
        else if (Physics.SphereCast(transform.position, radius, flyTarget - transform.position, out hit, controller.LocalVelocity.z * rangeModifier, groundLayer))
        //else if (Physics.Linecast(transform.position, flyTarget, out hit, groundLayer))
        {
            flyTarget = hit.point + (hit.normal + Vector3.up) * bumbModifier;
        }
        if (Physics.CheckSphere(transform.position+transform.forward+Vector3.down, radius, groundLayer))
        {
            flyTarget = transform.position + transform.forward + Vector3.up * radius;
        }


        if (Physics.Raycast(flyTarget+Vector3.up*100, Vector3.down, out hit, 200, groundLayer))
        {
            if(hit.point.y > flyTarget.y)
            {
                flyTarget.y = hit.point.y + radius;
            }
        }
        
        
        autopilot.RunAutopilot(flyTarget, out float pitch, out float yaw, out float roll);
        Vector3 input = new(pitch, yaw, roll);
        controller.SetControlInput(input);
    }

    private void DoChase(float dt)
    {
        //Debug.Log("DoChase");
        //return if no target
        if (targetGameObject == null || !targetGameObject.activeSelf) return;

        Vector3 tPos = targetTransform.position;
        Vector3 tVel = movingTarget?targetRigidbody.velocity:Vector3.zero;
        float pSpeed = controller.LocalVelocity.z;


        //similar heading? #Stop the hardcoding!
        if (movingTarget && Vector3.Dot(transform.forward, targetTransform.forward) > .8f && preferredDistance < distToTarget)
        {
            Debug.Assert(autopilot != null);
            Debug.Assert(targetRigidbody != null);
            autopilot.MatchSpeed(targetRigidbody.velocity.magnitude, dt);
        }
        else
        {
            controller.SetThrottleInput(1.0f);
        }

        //float distanceError = Mathf.Clamp((distToTarget - preferredDistance) / preferredDistance, 0.5f, 1f);
        //controller.SetThrottleInput(distanceError);

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
        //Compute where we want to go -> basically above target for now
        Vector3 desiredFlyTarget = targetTransform.position + Vector3.up * (zoomAltitude + zoomAltitudeSafe);
        flyTarget = desiredFlyTarget;

        //Debug.Log("DoZoom - desiredFlyTarget " + desiredFlyTarget + " targetTransform.pos" + targetTransform.position);

        //clamp climbrate

        Vector2 toTargetInPlane = new(flyTarget.x - transform.position.x, flyTarget.z - transform.position.z);
        float dist = toTargetInPlane.magnitude;
        float altitude = (targetTransform.position.y - transform.position.y);
        if ((flyTarget.y - transform.position.y) > climbRate * dist) 
        {
            flyTarget = new(flyTarget.x, transform.position.y + climbRate * dist, flyTarget.z);
        }
        //Debug.Log("DoZoom - transform.position "+ transform.position + "flyTarget.y " + flyTarget + " altitude " + altitude + " dist "+ dist);
        //find direction without rurning too hard
        //flyTarget = Vector3.RotateTowards(transform.forward, flyTarget, maxCrank * Mathf.Deg2Rad, 0);

        //check if we reached altitude high enough
        if (-altitude > zoomAltitude)
        {
            if(Vector3.Dot(targetTransform.forward, transform.forward) < Mathf.Cos(zoomDirection * Mathf.Deg2Rad)) return; //we only care if we ar heading in the same direction!
            if (dist < zoomBoxExtents)
            {
                zoomin = false;
                //autopilot.AggresiveTurnAngle = 0.0f;
            }
            
            //we are at the desired altitude
            //see if we are close enough "latitude"-wise... is that the right word?
            //float x = transform.position.x;
            //float z = transform.position.z;
            //float tx = targetTransform.position.x;
            //float tz = targetTransform.position.z;
            //if (Mathf.Abs(tx-x) < zoomBoxExtents && Mathf.Abs(tz - z) < zoomBoxExtents)
            //{
            //    zoomin = false; //we have now zoomed to the correct location to boom
            //}
        }
    }

    private void DoBoom(float dt)
    {
       // Debug.Log("DoBoom");
        //flyTarget
        //Compute where we want to go -> basically attack target
        Vector3 tPos = targetTransform.position;
        Vector3 tVel = movingTarget ? targetRigidbody.velocity : Vector3.zero;
        float pSpeed = controller.LocalVelocity.z;
        if(distToTarget < boomFireDist) //if we close to target change pSpeed to find gun solution
        {
            pSpeed = bulletSpeed;
        }

        //find optimal vector for intercept
        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, pSpeed, out Vector3 impactTarget, out float _))
        {
            flyTarget = impactTarget;
        }
        else
        {
            flyTarget = tPos; //failed to find...
            zoomin = true; //go back to zoomin
        }

        //really want to find the turn angle here and return to zoomin
        //find direction without rurning too hard
        //flyTarget = Vector3.RotateTowards(transform.forward, flyTarget, boomCrank * Mathf.Deg2Rad, 0);

        if (transform.position.y < targetTransform.position.y)
        {
            zoomin = true; // we are below target, rezoom the zoomin
            //autopilot.AggresiveTurnAngle = 10.0f;
        }
    }

    private static readonly Collider[] colliders = new Collider[20];
    public void SearchTarget()
    {
        int hits = Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + transform.forward * 100, 100, colliders, targetLayer);
        if(hits > 0)
        {
            float bestDist = float.MaxValue;
            int bestIndex = 0;
            for (int i = 0; i < hits; i++)
            {

                float dist = (transform.position - colliders[i].transform.position).sqrMagnitude;
                if(dist < bestDist)
                {
                    bestIndex = i;
                    bestDist = dist;
                }
            }
            SetTarget(colliders[bestIndex].gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo == true)
        {
            Color oldColor = Gizmos.color;

           
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(flyTarget, 4f);
            Gizmos.DrawLine(transform.position, flyTarget);
            

            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(gunSolutionTarget, 10f);

            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(ramTarget, 3f);


            Gizmos.color = oldColor;
        }
    }

    public float Evaluate()
    {
        if (targetGameObject == null)
        {
            SearchTarget();
            return 0;
        }
        //if (targetTransform == null) return 0;

        vecToTarget = targetTransform.position - transform.position;
        distToTarget = vecToTarget.magnitude;

        //Debug.Log("Evaluate Guns: " + (1 - ((distToTarget + 1) / maxInterceptDistance)));
        return 1-((distToTarget+1) / maxInterceptDistance);
    }

    public void Execute()
    {
        float dt = Time.fixedDeltaTime;
        if (targetGameObject != null)
        {
           
            DoChase(dt);
            if (zoomin)
            {
                DoBoom(dt);
                DoZoom(dt);
            }


            //TrackTarget();
            UpdateGuns();
        }
        else
        {
            SearchTarget();
        }

        UpdateAutopilot();
    }
}
