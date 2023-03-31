using System.Collections.Generic;
using UnityEngine;

public class SeekerHead : MonoBehaviour
{

    private static readonly Collider[] hitCollidersBuffer = new Collider[10]; //this is dirty but cool
    [Header("Seeking")]
    [Tooltip("What layer is targeted and locked on to")][SerializeField] private LayerMask heatLayer;
    [Tooltip("Minimum range in meters of lock on cone")][SerializeField] private float range = 150;
    [Tooltip("Instantaneous Field Of View of the seeker head/detector in degrees")]
    [SerializeField][Range(1,89)] private float seekerFOV = 12;


    [Range(0, 1)] public float seekerRangeInfluence = 1;//future use maybe when my math is better...
    [Range(0, 1)] public float seekerOffBoreInfluence = 0.1f;//future use maybe

    [Header("Tracking")]
    [Tooltip("Tacking angle in degrees for off-boresight capability")]
    [SerializeField][Range(1, 180)] private float trackFOV = 100;
    [Tooltip("Tacking rate in degrees per second")]
    [SerializeField] private float trackRate = 22;
    [SerializeField][Range(0, 10)] private float ProNavN = 3;

    [Header("Locking")]
    [SerializeField] private float minLockTime = .5f; //future use maybe

    [Header("Flying")]
    [Tooltip("The autopilot used, I'd like an interface here(?)")]
    [SerializeField] private SimpleFlight autoPilot;



    private float halfRadians;
    private float length; //length of cone (height more accurate?)
    private float radius;
    private float cosSeekerFov; //cached to compare with dot-products for fast angle check
    private float trackFOVRadians;
    private float trackRateRadians;
    private float cosTrackFov; //cached to compare with dot-products for fast angle check

    private Transform lockedTarget;
    private bool targetLocked;
    private Vector3 flyTarget;

    private Vector3 seekDirection; 
    private Vector3 prevTargetPosition;

    private float lockTime;//future use maybe

    public bool debugDraw = true;

    // Start is called before the first frame update
    void Start()
    {
        halfRadians = seekerFOV * Mathf.Deg2Rad / 2.0f;
        cosSeekerFov = Mathf.Cos(halfRadians);
        length = range * Mathf.Cos(halfRadians);
        radius = range * Mathf.Sin(halfRadians);


        trackFOVRadians = trackFOV * Mathf.Deg2Rad;
        trackRateRadians = trackRate * Mathf.Deg2Rad;
        cosTrackFov = Mathf.Cos(trackFOVRadians);
        //Debug.Log("trackRateRadians " + trackRateRadians + " trackRate " + trackRate);
        autoPilot.SetThrottleInput(1); //It's a rocket, just full powah

        seekDirection = transform.forward;
        flyTarget = transform.position + autoPilot.MaxSpeed * range * seekDirection; // 100 should ideally be the maxSpeed, to never reach it, but that is hidden in autopilot
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector3 desiredSeekerDir = transform.forward;
        if (targetLocked)
        {
            desiredSeekerDir = (lockedTarget.position - transform.position);
        }
        seekDirection = Vector3.RotateTowards(seekDirection, desiredSeekerDir, dt * trackRateRadians, 0);

        if (Seek(transform.position, seekDirection, out Transform target))
        {
            Vector3 targetPosition = target.position;
            Vector3 targetVel = (targetPosition - prevTargetPosition) / dt; //v=ds/dt
            prevTargetPosition = targetPosition;

            if (TargetingMath.ComputeImpact(targetPosition, targetVel, transform.position, autoPilot.LocalVelocity.z, out Vector3 impact, out float _))
            {
                flyTarget = impact;
            }
            else
            {
                flyTarget = targetPosition;
            }
            //update HUD
            //do sounds
            targetLocked = true;
            lockedTarget = target;
        }
        else
        {
            //update HUD
            //do other sounds
            //if targetLocked then we have lost target -> explode or reset target possible things

            targetLocked = false;
        }
        autoPilot.RunAutopilot(flyTarget); // outside if statement wll make it loop/continue to last seen position

        //bit more advanced and maybe overkull for now
        //if (targetLocked)
        //{
        //    TrackTarget(dt);
        //}
        //else
        //{
        //    LockOn(dt);
        //}
    }

    /*
     * https://en.wikipedia.org/wiki/Proportional_navigation
     */
    private Vector3 ProNav(Vector3 targetPosition, float dt)
    {
        Vector3 targetVel = (targetPosition - prevTargetPosition) / dt; //v=ds/dt
        prevTargetPosition = targetPosition;

        Vector3 relVel = targetVel - autoPilot.Velocity; //Vr=Vt-Vm
        Vector3 relPos = targetPosition - transform.position; //Rr=Rt-Rm

        Vector3 rotVec = Vector3.Cross(relPos, relVel) / Vector3.Dot(relPos, relPos); //omega:O rotational vector of line of sight change

        //desired acceleration
        Vector3 a = Vector3.Cross(ProNavN * relVel, rotVec); //a = N*Rr x O

        return a;
    }

    private void TrackTarget(float dt)
    {
        if (lockedTarget == null) //it died or was removed for some reason
        {
            targetLocked = false;
            return;
        }
        Vector3 toTarget = lockedTarget.position - transform.position;
        float angleToTarget = Vector3.Dot(transform.forward, toTarget.normalized);

        
        //float angleToTarget = Vector3.Angle(toTarget, transform.forward) * Mathf.Deg2Rad;
        //check if angle is ok for cone
        if (angleToTarget < cosSeekerFov)
        {
            targetLocked = false;
            lockTime = 0;
            //targetLost
            //autoPilot.SetFlyTarget(bestTarget.position, false);
            autoPilot.SetControlInput(Vector3.zero);
        }
        else
        {
            //autoPilot.SetFlyTarget(bestTarget.position, true);
            //autoPilot.RunAutopilot(lockedTarget.position);
        }
        
    }

    private void LockOn(float dt)
    {
        if (Seek(transform.position, transform.forward, out Transform potentialTarget))
        {
            if (potentialTarget == lockedTarget)
            {
                lockTime += dt;
                if (lockTime >= minLockTime)
                {
                    targetLocked = true;
                }
            }
            else
            {
                lockTime = 0;
                lockedTarget = potentialTarget;
            }
        }
        else
        {

        }
    }

    private bool Seek(Vector3 position, Vector3 direction, out Transform target)
    {
        target = null;
        //as Unity do not multithread MonoBehaviours (I think(!?)) all SeekerHeads can use the same static buffer as it's only used in this method alone
        int hits = Physics.OverlapCapsuleNonAlloc(position + direction * radius, position + direction * length, radius, hitCollidersBuffer, heatLayer);
        if (hits == 0) return false;

        //Collider[] hitCollidersBuffer = Physics.OverlapCapsule(position + direction*radius, position + direction * length, radius, heatLayer);
        //if(hitCollidersBuffer == null)
        //{
        //    Debug.LogWarning("SeekerHead.Seek() -> OverlapCapsule can return null");
        //    return false;
        //}
        //if (hitCollidersBuffer.Length == 0)
        //{
        //    return false;
        //}

        float bestValue = float.MaxValue;
        int bestIndex = -1;
        for (int i = 0; i < hits /*hitCollidersBuffer.Length*/; i++) 
        {
            Vector3 toTarget = hitCollidersBuffer[i].transform.position - position;
            float dist = toTarget.magnitude;
            float angleToTarget = Vector3.Dot(direction, toTarget/dist);
            float trackAngle = Vector3.Dot(transform.forward, toTarget / dist);
            //check if angle is inside the cone if not 
            if (angleToTarget < cosSeekerFov) continue; //cone seeker sees
            if (trackAngle < cosTrackFov) continue; // cone missile can track
            //ideally the best would be range+change in angle, but that requre history of all targets...

            float value = dist * (1-angleToTarget); //the closer target is to center line the higher angleToTarget is (0<..1) 
            //float value = dist;

            if (value < bestValue)
            {
                bestValue = angleToTarget;
                bestIndex = i;
            }
            
        }
        if (bestIndex < 0) return false; //nothing found inside the cone!

        target = hitCollidersBuffer[bestIndex].transform;
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw) return;
        Vector3 point0 = transform.position + seekDirection * radius;
        Vector3 point1 = transform.position + seekDirection * length;

        if (targetLocked)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

       // Gizmos.DrawWireSphere(point0, radius);

        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawLine(transform.position, point1 + transform.up * radius);
        Gizmos.DrawLine(transform.position, point1 - transform.right * radius);
        Gizmos.DrawLine(transform.position, point1 + transform.right * radius);
        Gizmos.DrawLine(transform.position, point1 - transform.up * radius);

        //Gizmos.DrawLine(point0 + transform.up * radius, point1 + transform.up * radius);
        //Gizmos.DrawLine(point0 - transform.right * radius, point1 - transform.right * radius);
        //Gizmos.DrawLine(point0 + transform.right * radius, point1 + transform.right * radius);
        //Gizmos.DrawLine(point0 - transform.up * radius, point1 - transform.up * radius);

        if (lockedTarget)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(flyTarget, 1);
            Gizmos.DrawLine(transform.position, flyTarget);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lockedTarget.position, 1);
            Gizmos.DrawLine(transform.position, lockedTarget.position);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + seekDirection * 100);
    }

}
