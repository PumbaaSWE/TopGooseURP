using UnityEngine;

public class SeekerHead : MonoBehaviour
{

    private static readonly Collider[] hitCollidersBuffer = new Collider[10]; //this is dirty but cool

    [Header("Seeking")]
    [Tooltip("What layer is targeted and locked on to")][SerializeField] private LayerMask heatLayer;
    [Tooltip("Minimum range in meters of lock on cone")][SerializeField] private float range = 150;
    [Tooltip("Instantaneous Field Of View of the seeker head/detector in degrees")]
    [SerializeField][Range(1,89)] private float seekerFOV = 12;


    [SerializeField] private float seekerRangeConeIgnore = 2;
    //[Range(0, 1)] public float seekerOffBoreInfluence = 0.1f;//future use maybe

    [Header("Tracking")]
    [Tooltip("Tracking angle in degrees for off-boresight capability")] //rename? It's fov/2 really... maxOffBoreAngle?
    [SerializeField][Range(1, 180)] private float trackFOV = 100;
    [Tooltip("Tacking rate in degrees per second")]
    [SerializeField] private float trackRate = 22;
    [SerializeField][Range(0, 10)] private float ProNavN = 3; // not used

    [Header("Locking")]
    [SerializeField] private float minLockTime = .5f;
    [SerializeField] private bool autoUncage = true; 

    [Header("Flying")]
    [Tooltip("The autopilot used")] //require component? and get on start instead?
    [SerializeField] private SimpleFlight autoPilot;



    private float halfRadians;
    private float length; //length of cone (height more accurate?)
    private float radius;
    private float cosSeekerFov; //cached to compare with dot-products for fast angle check
    private float trackFOVRadians;
    private float trackRateRadians;
    private float cosTrackFov; //cached to compare with dot-products for fast angle check

    private Transform lockedTarget;
    /// <summary>
    /// read only, true if this seeker has a target looked
    /// </summary>
    public bool TargetLocked { get; private set; }
    /// <summary>
    /// read only, position in world space where this seeker wants to fly, input for the autopilot
    /// </summary>
    public Vector3 FlyTarget { get; private set; }

    /// <summary>
    /// read only, true if this seeker been launched
    /// </summary>
    public bool Launched { get; private set; } = false;

    public bool active = false; //maybe remove

    public bool uncaged = false;

    //public bool Uncaged
    //{
    //    get { return uncaged; }
    //    set
    //    {
    //        if (!Launched) uncaged = value;
    //    }
    //}


    public enum SeekerTone { Active, InView, InViewOffBore, Locked };
    public SeekerTone Tone { get; private set; } = SeekerTone.Active;

    private Vector3 cageDirection;

    private Vector3 seekDirection; 
    private Vector3 prevTargetPosition;

    private float lockTime;

    public bool drawDebugInfo = true;

    /// <summary>
    /// read only, for UI, what is this seeker looking at
    /// </summary>
    public Vector3 SeekerViewPositon
    {
        get { return transform.position + seekDirection * range; }
    }
    /// <summary>
    /// read only, for UI, what is the locked targets position, if no target jus seekers view is returned 
    /// </summary>
    public Vector3 TargetPosition
    {
        get { 
            if(TargetLocked && lockedTarget != null)
                return lockedTarget.position;
            return SeekerViewPositon;
        }
    }

    void Start()
    {
        halfRadians = seekerFOV * Mathf.Deg2Rad / 2.0f;
        cosSeekerFov = Mathf.Cos(halfRadians);
        length = range * Mathf.Cos(halfRadians);
        radius = range * Mathf.Sin(halfRadians);


        trackFOVRadians = trackFOV * Mathf.Deg2Rad;
        trackRateRadians = trackRate * Mathf.Deg2Rad;
        cosTrackFov = Mathf.Cos(trackFOVRadians);

        seekDirection = transform.forward;
        //FlyTarget = transform.position + autoPilot.MaxSpeed * range * seekDirection; 
    }

    /// <summary>
    /// Will activate this seeker to look for targets, if launced it cannot be disabled
    /// </summary>
    /// <param name="active"></param>
    public void Activate(bool active)
    {
        if (Launched) return;
        if(!this.active && active)
        {
            seekDirection = transform.forward;
            TargetLocked = false;
            lockedTarget = null;
        }
        this.active = active;
    }

    private void OnEnable()
    {
        if (Launched) return;
        ClearTarget();
        Uncage(false);
    }

    /// <summary>
    /// Removes current target and allow the seeker to look a different one.
    /// </summary>
    public void ClearTarget()
    {
        seekDirection = transform.forward; 
        TargetLocked = false;
        lockedTarget = null;
    }

    /// <summary>
    /// Sets the cage direction to desired track angle, uses world space direction
    /// </summary>
    /// <param name="direction"></param>
    public void Track(Vector3 direction)
    {
        //move seeker head towards
        cageDirection = direction;
    }

    /// <summary>
    /// Sets cage status, uncaging is allowing the seeker itself to look in whatever direction it wants to, else it will look where ever the cage direction is telling it to look.
    /// </summary>
    /// <param name="uncage"></param>
    public void Uncage(bool uncage)
    {
        if (Launched) return;
        uncaged = uncage;
    }

    public void Launch()
    {
        if (!TargetLocked) FlyTarget = transform.position + autoPilot.MaxSpeed * range * transform.forward; //if launced without lock go forward and not world origin
        //uncaged = true; //should be done already

        autoPilot.SetThrottleInput(1);
        Launched = true;
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
    void FixedUpdate()
    {
        if (lockedTarget == null || !lockedTarget) //it died or was removed for some reason
        {
            TargetLocked = false;
        }


        float dt = Time.fixedDeltaTime;
        //WorkingTracking(dt);

        TrackTarget(dt); //experimental
        if(Launched) autoPilot.RunAutopilot(FlyTarget);
    }

    /// <summary>
    /// Depricated for use if the code fails
    /// </summary>
    /// <param name="dt"></param>
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
                FlyTarget = impact;
            }
            else
            {
                FlyTarget = targetPosition;
            }
            TargetLocked = true;
            lockedTarget = target;
        }
        else
        {
            //if targetLocked then we have lost target -> explode or reset FlyTarget possible things
            TargetLocked = false;
        }
        autoPilot.RunAutopilot(FlyTarget); // outside if statement will make it loop/continue to last seen position
    }

    /*
     * https://en.wikipedia.org/wiki/Proportional_navigation
     */
    /// <summary>
    /// I did not make this work, for future testing, dont use!
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
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

        seekDirection = Vector3.RotateTowards(seekDirection, desiredSeekerDir, dt * trackRateRadians, 0); //smoothly track towards desired dir with a limit on radians per sec

        if(Vector3.Dot(seekDirection, transform.forward) < cosTrackFov) //if we seek outside the tracking fov
            seekDirection = Vector3.RotateTowards(transform.forward, seekDirection, trackFOVRadians, 0); //make sure its inside

        cageDirection = transform.forward; //this is so seeker is pointed forward incase Track() is not called i.e seekDir will not be connected to a world direction but local
    }

    /// <summary>
    /// We are uncaged so compute intercept point based on relative speed and position to target. Computes target velocity based on positional change to not rely on rigidbody.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dt"></param>
    private void HandleUncaged(Transform target, float dt)
    {
        Vector3 targetPosition = target.position;
        Vector3 targetVel = (targetPosition - prevTargetPosition) / dt; //v=ds/dt
        prevTargetPosition = targetPosition;
        if (TargetingMath.ComputeImpact(targetPosition, targetVel, transform.position, autoPilot.LocalVelocity.z, out Vector3 impact, out float _))
        {
            FlyTarget = impact; // + Random.insideUnitSphere * 5 * tti; // for some randomness in flight
        }
        else
        {
            FlyTarget = targetPosition;
        }
        
    }



    /// <summary>
    /// Main logic, Point the seeker in the desired direction look for the target, of try find a new
    /// </summary>
    /// <param name="dt"></param>
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

    /// <summary>
    /// If a target is found we can tell the seeker to look on and make the seeker automatically track it, decided by uncaging the missile
    /// </summary>
    /// <param name="potentialTarget"></param>
    /// <param name="trackAngle"></param>
    /// <param name="dt"></param>
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
                return;
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
    /// <summary>
    /// Search in a cone in specified direction from specified point after a transform(gameObject) on a certain layer. Cone and layer is defiended in inspector, variables of this class
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="target">out: if found target is the transform of that wahat was fount</param>
    /// <param name="offBoreAngle">please note its in cos(radians), not radians or degrees</param>
    /// <returns>True is a target was found, else false</returns>
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
            if (dist > seekerRangeConeIgnore && angleToTarget < cosSeekerFov) continue; //cone seeker sees
            //if (trackAngle < cosTrackFov) continue; // cone missile can track
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

    //To draw debug info abouth the seeker as search volume and direction, seen target and intercept point
    private void OnDrawGizmos()
    {
        if (!drawDebugInfo) return;
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
            Gizmos.DrawWireSphere(FlyTarget, 1);
            Gizmos.DrawLine(transform.position, FlyTarget);
            
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
