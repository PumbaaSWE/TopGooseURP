﻿using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] private bool autoUncage = true; //future use maybe

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
    public bool TargetLocked { get; private set; }
    private Vector3 flyTarget;

    public bool Launched { get; private set; } = false;

    public bool active = false;
    public bool uncaged = false;

    //public bool Uncaged
    //{
    //    get { return uncaged; }
    //    set
    //    {
    //        if (!Launched) uncaged = value;
    //    }
    //}

    public enum SeekerStage { Seeking, Locked, Launched };
    public SeekerStage Stage { get; private set; } = SeekerStage.Seeking;

    public enum SeekerTone { Active, InView, InViewOffBore, Locked };
    public SeekerTone Tone { get; private set; } = SeekerTone.Active;

    private Vector3 cageDirection;

    private Vector3 seekDirection; 
    private Vector3 prevTargetPosition;

    private float lockTime;//future use maybe

    public bool debugDraw = true;

    public Vector3 SeekerViewPositon
    {
        get { return transform.position + seekDirection * range; }
    }

    public Vector3 TargetPosition
    {
        get { 
            if(TargetLocked)
                return lockedTarget.position;
            return SeekerViewPositon;
        }
    }

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
        //autoPilot.SetThrottleInput(1); //It's a rocket, just full powah

        seekDirection = transform.forward;
        flyTarget = transform.position + autoPilot.MaxSpeed * range * seekDirection; // 100 should ideally be the maxSpeed, to never reach it, but that is hidden in autopilot
    }

    public void Activate(bool active)
    {
        if (Launched) return;
        this.active = active;
    }

    public void Track(Vector3 direction)
    {
        //move seeker head towards
        cageDirection = direction;
    }

    public void Uncage(bool uncage)
    {
        if (Launched) return;
        uncaged = uncage;
    }

    public void Launch(Vector3 inheritVelocity)
    {
        if (!TargetLocked) flyTarget = transform.position + autoPilot.MaxSpeed * range * seekDirection;
        //uncaged = true; //should be done already

        autoPilot.Free(inheritVelocity);
        autoPilot.SetThrottleInput(1);
        Launched = true;
        Destroy(gameObject, 5.0f);
    }


    /*
     *How it suppose to work:
     *Missile is attached to Plane
     *Pilot points in direction to seek in -> Track(cageDirection)
     *If seeker sees it will make tone
     *If ungaged seeker head will follow whatever target it sees by it self and ignore pilot view dir (cageDirection)
     *Launch will happen if uncaged and locked
     *If lock is lost we go back to caged
     *
     *Seek dir
     *When not fired and caged -> in cageDir (or forward in slave mode) 
     *When not fired and uncaged and sees target-> at target
     *When not fired and uncaged and dont sees target-> forward
     *When fired and dont see target -> forward
     *When fired and see target -> at target
     *
     *When fired -> runAutopilot
     *
     *cannot go uncaged to caged after being fired
     *if fired uncaged it will go uncaged (?)
     *
     *How is not fired, uncaged, and not seeing target handeled?
     */
    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockedTarget == null || !lockedTarget) //it died or was removed for some reason
        {
            TargetLocked = false;
        }


        float dt = Time.fixedDeltaTime;
        //WorkingTracking(dt);

        TrackTarget(dt); //experimental
        if(Launched) autoPilot.RunAutopilot(flyTarget);
    }

    private void WorkingTracking(float dt)
    {
        Vector3 desiredSeekerDir = transform.forward;
        if (TargetLocked)
        {
            desiredSeekerDir = (lockedTarget.position - transform.position);
        }
        else if (!uncaged)
        {
            desiredSeekerDir = cageDirection;
        }
        seekDirection = Vector3.RotateTowards(seekDirection, desiredSeekerDir, dt * trackRateRadians, 0);

        if (Seek(transform.position, seekDirection, out Transform target, out float trackAngle))
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
            TargetLocked = true;
            lockedTarget = target;
        }
        else
        {
            //update HUD
            //do other sounds
            //if targetLocked then we have lost target -> explode or reset target possible things

            TargetLocked = false;
        }
        autoPilot.RunAutopilot(flyTarget); // outside if statement wll make it loop/continue to last seen position
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

    /*
     * If is uncaged and has no target -> forward
     * If has target and is uncaged -> at target
     * If not uncaged with or without target -> cage direction
     */
    private void HandleSeekDirection(float dt)
    {
        Vector3 desiredSeekerDir = transform.forward;
        if (!uncaged)
        {
            desiredSeekerDir = cageDirection;
        }
        else if (TargetLocked)
        {
            desiredSeekerDir = (lockedTarget.position - transform.position);
        }
        //seekDirection = transform.rotation * transform.forward; //this is to keep the angle local to the "missile"
        seekDirection = Vector3.RotateTowards(seekDirection, desiredSeekerDir, dt * trackRateRadians, 0);
        //LimitVector(ref seekDirection);
    }
    private void LimitVector(ref Vector3 toLimit)
    {
        float angle = Vector3.Dot(toLimit, transform.forward);
        if (angle < cosTrackFov){
            Vector3 rotAxis = Vector3.Cross(toLimit, transform.forward);
            toLimit = Quaternion.AngleAxis(trackFOV, rotAxis) * transform.forward;
            //toLimit = Vector3.RotateTowards(toLimit, transform.forward, cosTrackFov - angle, 0); //needed an acos to work
        }
       // return toLimit;
    }

    private void HandleUncaged(Transform target, float dt)
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
        
    }


    private void TrackTarget(float dt)
    {
        HandleSeekDirection(dt);

        if (Seek(transform.position, seekDirection, out Transform target, out float trackAngle))
        {
            if (TargetLocked)
            {
                HandleUncaged(target, dt);
            }
            else
            {
                LockOn(target, trackAngle, dt);
            }
        }
        else
        {
            TargetLocked = false;
            lockedTarget = null;
            Uncage(false);
            lockTime = 0;
        }

    }

    //
    private void LockOn(Transform potentialTarget, float trackAngle, float dt)
    {  
        lockedTarget = potentialTarget;  
        if(uncaged)
        {
            TargetLocked = true;
            Tone = SeekerTone.Locked;
            return;
        }
        else if (autoUncage)
        {
            lockTime += dt;
            if(lockTime >= minLockTime)
            {
                uncaged = true;
                TargetLocked = true;
                Tone = SeekerTone.Locked;
            }
        }
        if (trackAngle > Mathf.Cos(27 * Mathf.Deg2Rad))
        {
            Tone = SeekerTone.InViewOffBore;
        }
        else
        {
            Tone = SeekerTone.InView;
        }
    }

    private bool Seek(Vector3 position, Vector3 direction, out Transform target, out float offBoreAngle)
    {
        target = null;
        offBoreAngle = 0;
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
                bestValue = value;
                bestIndex = i;
                offBoreAngle = trackAngle;
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

        if (TargetLocked)
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
