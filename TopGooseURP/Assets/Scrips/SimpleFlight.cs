using UnityEngine;

public class SimpleFlight : MonoBehaviour
{

    [Header("Thrusting")]
    [Tooltip("Poweeeeerrr!!!")][SerializeField] private float maxThrust = 50;
    [Tooltip("(currently unused)m/s^2??")][SerializeField] private float maxAcceleration = 50;
    [Tooltip("Speeeeeeeed!!!")][SerializeField] private float maxSpeed = 50; //change to private!!!!

    [Header("Steering")]
    [Tooltip("Deg per sec; x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnSpeed = new(180, 180, 360);
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnAcceleration = new(999, 999, 999);
    [Tooltip("Set to one if unsure")][SerializeField] private AnimationCurve steeringCurve;

    [Header("Autopiloting")]
    [Tooltip("Strength for autopilot flight.")][SerializeField] private float strength = 2f;
    [Tooltip("Angle at which airplane banks fully into target.")][SerializeField] private float aggressiveTurnAngle = 2f;


    public float MaxSpeed => maxSpeed;
    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public Vector3 LocalAngularVelocity { get; private set; }
    public float Throttle { get; private set; }
    public Vector3 Steering { get; private set; }

    private float throttleInput;
    private Vector3 controlInput;


    public Rigidbody Rigidbody { get; private set; }


    // Start is called before the first frame update
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        UpdateState();

        UpdateSteering(dt);
        UpdateThrottle(dt);
        UpdateThrust();
    }

    public void Free(Vector3 inheritVelocity)
    {
        transform.parent = null;
        Rigidbody.isKinematic = false;
        Rigidbody.velocity = inheritVelocity;
    }

    //will be clamped 0,1 -> but hopefully can add a set desired speed to simplify
    public void SetThrottleInput(float input)
    {
        throttleInput = input;
    }
    public void SetControlInput(Vector3 input)
    {
        controlInput = Vector3.ClampMagnitude(input, 1);
    }

    //thinking this should pick a throttle to get a desired speed not just be a power setting hard to grasp/wrap your head around
    //PID might be overkill, proportional with maybe derivative should be enough
    void UpdateThrottle(float dt)
    {
        
        //float error = desiredSpeed - LocalVelocity.z;
        //float a = maxAcceleration * dt;
        //I need more math-skillz

        Throttle = Mathf.Clamp01(throttleInput); // dt to be incorporated to not throttle to fast
    }

    //hard stop on max speed, we have no drag to limit speed and this is easier to balance and require less math to get right
    void UpdateThrust()
    {
        Rigidbody.AddRelativeForce(Throttle * maxThrust * Vector3.forward);
        if(LocalVelocity.z > maxSpeed)
        {
            Rigidbody.velocity = Velocity.normalized * maxSpeed;
        }
    }

    //maybe mor complex than it needs to be, taken from the full flight controller
    private void UpdateSteering(float dt)
    {
        float speed = Mathf.Max(0, LocalVelocity.z);
        float steeringPower = steeringCurve.Evaluate(speed); //I'd like to remove the curve but relating turn rate to speed is nice 
        Vector3 targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower); //AV is angular velicity........
        Vector3 av = LocalAngularVelocity * Mathf.Rad2Deg;

        Vector3 correction = new(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
        );
        Steering = controlInput;
        Rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float a = acceleration * dt;
        return Mathf.Clamp(error, -a, a);
    }

    private void UpdateState()
    {
        Quaternion invRotation = Quaternion.Inverse(Rigidbody.rotation);
        Velocity = Rigidbody.velocity;
        LocalVelocity = invRotation * Velocity;
        LocalAngularVelocity = invRotation * Rigidbody.angularVelocity;
    }

    /// <summary>
    /// Proportional steering so prone to overshooting and fun wobblyness
    /// </summary>
    /// <param name="flyTarget"></param>
    public void RunAutopilot(Vector3 flyTarget)
    {
        Vector3 localFlyTarget = transform.InverseTransformPoint(flyTarget).normalized * strength;
        float angleOffTarget = Vector3.Angle(transform.forward, flyTarget - transform.position);

        float yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        float pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);

        float agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        // A "wings level roll" is a roll commands the aircraft to fly wings level.
        // This can be done by zeroing out the Y component of the aircraft's right.
        float wingsLevelRoll = transform.right.y;

        // Blend between auto level and banking into the target.
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        float roll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
        controlInput.Set(pitch, yaw, roll);    
    }
}
