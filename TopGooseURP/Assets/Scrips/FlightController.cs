using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
    [Header("Lifting")]
    [Tooltip("Lift modifier of aoa curve")][SerializeField] private float liftPower = 1;
    [Tooltip("slip modifier of aoa curve yaw")][SerializeField] private float horizontalPower = 1;
    [Tooltip("aoa curve: google it")][SerializeField] private AnimationCurve aoaCurve;
    [Tooltip("Set to zero if unsure, affect slip (sidways drift/slide)")][SerializeField] private AnimationCurve aoaCurveYaw;
    [Tooltip("Loose speed while doing turns and shit(may not work as intended)")][SerializeField] private float inducedDragPower = 1;

    [Header("Dragging")]
    //[SerializeField] private AnimationCurve dragCurve;
    [Tooltip("x: up/down, y: sideways, z: going forward(/backward)")][SerializeField] private Vector3 dragPower = new(1, 1, 0.5f);
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 angularDrag = new(0.5f, 0.5f, 0.5f);

    [Header("Thrusting")]
    [Tooltip("poweeeeeeerrrr!!!")][SerializeField] private float maxThrust = 50;

    [Header("Steering")]
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnSpeed = new(50, 50, 99);
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnAcceleration = new(99, 99, 99);
    [Tooltip("Set to one if unsure")][SerializeField] private AnimationCurve steeringCurve;

    [Header("Autopiloting")]
    [Tooltip("Strength for autopilot flight.")][SerializeField] private float strength = 5f;
    [Tooltip("Angle at which airplane banks fully into target.")][SerializeField] private float aggressiveTurnAngle = 10f;


    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public Vector3 LocalAngularVelocity { get; private set; }
    public float AngleOfAttack { get; private set; }
    public float AngleOfAttackYaw { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public float Throttle { get; private set; }
    public Vector3 Steering { get; private set; }
    public bool DisableInput { get; set; }

    private float throttleInput;
    private Vector3 controlInput;

    public void SetThrottleInput(float input)
    {
        if (DisableInput) return;
        throttleInput = input;
    }
    public void SetControlInput(Vector3 input)
    {
        if (DisableInput) return;
        controlInput = Vector3.ClampMagnitude(input, 1);
    }
    internal void ResetInput()
    {
        throttleInput = 0;
        controlInput = Vector3.zero;
    }

    public void ResetVelocities()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (!DisableInput)
        {
            UpdateSteering(dt);
            UpdateThrottle(dt);
        }
        UpdateThrust();
        UpdateState(dt);
        UpdateLift();
        UpdateDrag();
        UpdateAngularDrag();

    }

    void UpdateThrust()
    {
        Rigidbody.AddRelativeForce(Throttle * maxThrust * Vector3.forward);
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float a = acceleration * dt;
        return Mathf.Clamp(error, -a, a);
    }

    void UpdateThrottle(float dt)
    {
        Throttle = Mathf.Clamp(throttleInput, 0, 1); // dt to be incorporated to not throttle to fast
    }

    private void UpdateSteering(float dt)
    {
        float speed = Mathf.Max(0, LocalVelocity.z);
        float steeringPower = steeringCurve.Evaluate(speed);
        Vector3 targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower);
        Vector3 av = LocalAngularVelocity * Mathf.Rad2Deg;

        Vector3 correction = new(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
        );
        Steering = controlInput;
        Rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);
    }

    private void UpdateState(float dt) //dt needed?
    {
        Quaternion invRotation = Quaternion.Inverse(Rigidbody.rotation);
        Velocity = Rigidbody.velocity;
        LocalVelocity = invRotation * Velocity;

        //Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.blue);
        //Debug.DrawLine(transform.position, transform.position + Velocity * 5, Color.yellow);

        LocalAngularVelocity = invRotation * Rigidbody.angularVelocity;
        CalculateAngleOfAttack();
    }

    private void CalculateAngleOfAttack()
    {
        if (LocalVelocity.sqrMagnitude < 0.1f)
        {
            AngleOfAttack = 0;
            AngleOfAttackYaw = 0;
            return;
        }

        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z); //rotation around x-axis
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z); //rotation around y-axis
    }

    private void UpdateDrag()
    {
        Vector3 lvn = LocalVelocity.normalized;
        float lv2 = LocalVelocity.sqrMagnitude;

        Vector3 coefficient = Vector3.Scale(lvn, dragPower);

        Vector3 drag = coefficient.magnitude * lv2 * -lvn;

        Rigidbody.AddRelativeForce(drag);
    }

    //to not rotate out of control...
    private void UpdateAngularDrag()
    {
        var av = LocalAngularVelocity;
        var drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    private Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, float inducedDragPower, AnimationCurve aoaCurve)
    {
        Vector3 liftVelocity = Vector3.ProjectOnPlane(LocalVelocity, rightAxis);    //project velocity onto YZ plane -> sweep angles and stuff?
        float v2 = liftVelocity.sqrMagnitude;                                     //square of velocity

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        float liftForce = v2 * liftCoefficient * liftPower;

        //Debug.Log("Lift Coefficient:" + liftCoefficient + ", at:" + angleOfAttack + ", deg:" + angleOfAttack * Mathf.Rad2Deg);

        //lift is perpendicular to velocity
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 dragDirection = -liftVelocity.normalized;
        Vector3 inducedDrag = dragForce * inducedDragPower * v2 * dragDirection;

        //Debug.DrawLine(transform.position, transform.position+lift, Color.green);
        //Debug.DrawLine(transform.position, transform.position + inducedDrag, Color.red);

        return lift + inducedDrag;
    }

    private void UpdateLift()
    {
        if (LocalVelocity.sqrMagnitude < 1.0f) return; // too slow

        Vector3 upForce = CalculateLift(AngleOfAttack, Vector3.right, liftPower, inducedDragPower, aoaCurve);
        Vector3 sideForce = CalculateLift(AngleOfAttackYaw, Vector3.up, horizontalPower, inducedDragPower, aoaCurveYaw);

        Rigidbody.AddRelativeForce(upForce);
        Rigidbody.AddRelativeForce(sideForce);
    }

    public void RunAutopilot(Vector3 flyTarget, out float yaw, out float pitch, out float roll)
    {
        // This is my usual trick of converting the fly to position to local space.
        // You can derive a lot of information from where the target is relative to self.
        Vector3 localFlyTarget = transform.InverseTransformPoint(flyTarget).normalized * strength;
        

        float angleOffTarget = Vector3.Angle(transform.forward, flyTarget - transform.position);

        // IMPORTANT!
        // These inputs are created proportionally. This means it can be prone to
        // overshooting. Use of a PID controller for each axis is highly recommended.

        // ====================
        // PITCH AND YAW
        // ====================

        // Yaw/Pitch into the target so as to put it directly in front of the aircraft.
        // A target is directly in front the aircraft if the relative X and Y are both
        // zero. Note this does not handle for the case where the target is directly behind.
        yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);
        pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);

        // ====================
        // ROLL
        // ====================

        // Roll is a little special because there are two different roll commands depending
        // on the situation. When the target is off axis, then the plane should roll into it.
        // When the target is directly in front, the plane should fly wings level.

        // An "aggressive roll" is input such that the aircraft rolls into the target so
        // that pitching up (handled above) will put the nose onto the target. This is
        // done by rolling such that the X component of the target's position is zeroed.
        float agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        // A "wings level roll" is a roll commands the aircraft to fly wings level.
        // This can be done by zeroing out the Y component of the aircraft's right.
        float wingsLevelRoll = transform.right.y;

        // Blend between auto level and banking into the target.
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        roll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
    }


}
