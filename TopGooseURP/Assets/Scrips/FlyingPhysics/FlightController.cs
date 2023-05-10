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
    [Tooltip("Drag multiplier x: up/down, y: sideways, z: going forward(/backward)")][SerializeField] private Vector3 dragPower = new(3, 3, 1);
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 angularDrag = new(0.5f, 0.5f, 0.5f);
    [Tooltip("How much do the airframe want to turn into the airflow")][SerializeField] private float directionalDragCoeff = 1;
    //[Tooltip("Soft speed limit (m/s), drag increases alot arount this speed")][SerializeField] private float speedDragLimit = 17;
    //[Tooltip("How much should gravity affect the soft speed limit?")][SerializeField] private float angleDragFactor = 5;
    [Tooltip("DONT CHANGE! Computed drag Coeff")][SerializeField] private float dragCoeff = 0;

    [Header("Thrusting")]
    [Tooltip("poweeeeeeerrrr!!!")][SerializeField] private float maxThrust = 50;
    [Tooltip("Soft speed limit (m/s), compote drag coeaf based on thism maxThrust and mass.")][SerializeField] private float softSpeedLimit = 17;

    [Header("Steering")]
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnSpeed = new(50, 50, 99);
    [Tooltip("x: pitch, y: yaw, z: roll")][SerializeField] private Vector3 turnAcceleration = new(99, 99, 99);
    [Tooltip("Set to one if unsure")][SerializeField] private AnimationCurve steeringCurve;

    [Header("Cieling")]
    [Tooltip("Flight Cieling in meters, will push the controller down with max thrust")][SerializeField] private float flightCieling = 200;

    /// <summary>
    /// Read only - Rigidbody velocity
    /// </summary>
    public Vector3 Velocity { get; private set; }
    /// <summary>
    /// Read only - The Rigidbody velocity in this objects local space
    /// </summary>
    public Vector3 LocalVelocity { get; private set; }
    /// <summary>
    /// Read only - The Rigidbody angular velocity in this objects local space
    /// </summary>
    public Vector3 LocalAngularVelocity { get; private set; }
    /// <summary>
    /// Read only - The angle of attack in degrees of the forward vector (pitch)
    /// </summary>
    public float AngleOfAttack { get; private set; }
    /// <summary>
    /// Read only - The yawing angle of attack in degrees of the forward vector
    /// </summary>
    public float AngleOfAttackYaw { get; private set; }
    /// <summary>
    /// Read only - Reference to the attached rigidbody
    /// </summary>
    public Rigidbody Rigidbody { get; private set; }
    /// <summary>
    /// Read only - Current position of the throttle
    /// </summary>
    public float Throttle { get; private set; }
    /// <summary>
    /// Read only - Current Stering vector used as input for steering, pitch, yaw and roll
    /// </summary>
    public Vector3 Steering { get; private set; }
    /// <summary>
    /// Used to ignore any control input set by any other script
    /// </summary>
    public bool DisableInput { get; set; }

    private float throttleInput;
    private Vector3 controlInput;


    /// <summary>
    /// Set the throttle, the value is clamped between 0 and 1. Is how much of the maxThrust that is applied.
    /// </summary>
    /// <param name="input"></param>
    public void SetThrottleInput(float input)
    {
        if (DisableInput) return;
        throttleInput = input;
    }

    /// <summary>
    /// Set the input for pitch, yaw and roll (in that order) with a vector.
    /// -1 to 1 are max and min values
    /// </summary>
    /// <param name="input">The Vector3 representing pitch, yaw and roll</param>
    public void SetControlInput(Vector3 input)
    {
        if (DisableInput) return;
        controlInput = Vector3.ClampMagnitude(input, 1);
    }

    /// <summary>
    /// Set all inputs to zero, useful for respawns
    /// </summary>
    public void ResetInput()
    {
        throttleInput = 0;
        controlInput = Vector3.zero;
    }
    /// <summary>
    /// Instantly stop this rigidbody, useful for respawns
    /// </summary>
    public void ResetVelocities()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.velocity = transform.forward * softSpeedLimit;

        //terminal velocity v = sqrt(2ma/pAC)
        //pAC is simplified to a single number in this simulation. ie, air dencity and cross area are ignored and bundled into drag coeff
        //a is derived from F=ma -> a = F/m
        //so 2ma -> 2F because m cancels out
        //we want to know C so rearanging gives : C = 2F/v^2
        //but when computing drag that formula contains 1/2 so I'll ignore the *2 now and the *.5 later... a pro-gamer move

        float v2 = softSpeedLimit * softSpeedLimit;
        dragCoeff = maxThrust / v2;

    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        UpdateState(dt);
        if (!DisableInput)
        {
            UpdateSteering(dt);
            UpdateThrottle(dt);
        }
        UpdateThrust();
        UpdateLift();
        UpdateDrag();
        UpdateAngularDrag();


        if(flightCieling < transform.position.y)
        {
            //float force = transform.position.y - flightCieling;
            Rigidbody.AddForce(Vector3.down*maxThrust, ForceMode.Acceleration);
            //Debug.Log("Flight Cieling reached");
        }

        UpdateState(dt);

    }

    private void UpdateThrust()
    {
        Rigidbody.AddRelativeForce(Throttle * maxThrust * Vector3.forward);
    }

    private float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float a = acceleration * dt;
        return Mathf.Clamp(error, -a, a);
    }

    private void UpdateThrottle(float dt)
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

    //Drag = coeff * density * velocity^2/2 * reference area. coeff and ref area is baked into dragPower,
    //so z is drag in forward direction, x is going belly/back first and y is going sideways.
    private void UpdateDrag()
    {
        Vector3 lvn = LocalVelocity.normalized; // directon to apply the drag in
        float lv2 = LocalVelocity.sqrMagnitude;

        Vector3 coefficient = Vector3.Scale(lvn, dragPower)*dragCoeff;

        Vector3 drag = coefficient.magnitude * lv2 * -lvn;

        Rigidbody.AddRelativeForce(drag);



        //if we pass speedDragLimit the drag should increase by alot
        //if (1.0f < LocalVelocity.z / speedDragLimit) // this will be over one when speed is over speedDragLimit
        //{
        //    float dirFactor = 1 + Vector3.Dot(Rigidbody.velocity.normalized, Vector3.up); // are we going downwards, i.e. gravity helps? then less drag, add 1 to make it >=0 [0-2]
        //    we are over the desired speed so we remove the delta multiplied with a factor related to direction so diving is less affected. (and going up is more)
        //    Rigidbody.AddRelativeForce(dirFactor * dirFactor * -(LocalVelocity - lvn * speedDragLimit), ForceMode.VelocityChange);
        //}

    }

    //to not rotate out of control...
    private void UpdateAngularDrag()
    {
        Vector3 av = LocalAngularVelocity;
        Vector3 drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass

        //this adds torque so the object want rotate in the direction of travel i.e. pointing in velocity direction, (think dart)
        //rotation in x is pitch and velocity in y is falling/climbing. in y is yaw and x in velocity is strafing, z is roll and we don't need that
        Rigidbody.AddRelativeTorque(new Vector3(-directionalDragCoeff * LocalVelocity.y, LocalVelocity.x * directionalDragCoeff, 0));
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
