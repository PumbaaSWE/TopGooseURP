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
        Debug.Log(LocalVelocity + "<-LocalVelocity");
        float lv2 = LocalVelocity.sqrMagnitude;

        Vector3 coefficient = Vector3.Scale(lvn, dragPower);

        Vector3 drag = coefficient.magnitude * lv2 * -lvn;

        Debug.Log(drag + "<-drag");

        Rigidbody.AddRelativeForce(drag*0.01f);
    }

    //to not rotate out of control...
    private void UpdateAngularDrag()
    {
        Vector3 av = LocalAngularVelocity;
        Vector3 drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    private Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, float inducedDragPower, AnimationCurve aoaCurve)
    {
        Vector3 liftVelocity = Vector3.ProjectOnPlane(LocalVelocity, rightAxis);    //project velocity onto YZ plane -> sweep angles and stuff?
        float velocitySqrd = liftVelocity.sqrMagnitude;

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        float liftForce = velocitySqrd * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 dragDirection = -liftVelocity.normalized;
        Vector3 inducedDrag = dragForce * inducedDragPower * velocitySqrd * dragDirection;

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

}
