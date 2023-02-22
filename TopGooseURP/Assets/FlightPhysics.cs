using UnityEngine;

public class FlightPhysics : MonoBehaviour
{

    [Header("Lifting")]
    [SerializeField] private float verticalPower = 1;
    [SerializeField] private float horizontalPower = 1;
    [SerializeField] private AnimationCurve aoaCurvePitch;
    [SerializeField] private AnimationCurve aoaCurveYaw;
    [SerializeField] private float inducedDragPower = 1;

    [Header("Dragging")]
    //[SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private Vector3 dragPower = new Vector3(2, 2, 1);
    [SerializeField] private Vector3 angularDrag = new Vector3(1, 1, 1);

    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public Vector3 LocalAngularVelocity { get; private set; }
    public float AngleOfAttackPitch { get; private set; }
    public float AngleOfAttackYaw { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetVelocities()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        UpdateState(dt);
        UpdateLift();
        UpdateDrag();
        UpdateAngularDrag();
    }


    private void UpdateState(float dt)
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
            AngleOfAttackPitch = 0;
            AngleOfAttackYaw = 0;
            return;
        }

        AngleOfAttackPitch = Mathf.Atan2(-LocalVelocity.x, LocalVelocity.z); //rotation around y-axis
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.y, LocalVelocity.z); //rotation around x-axis
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

        //lift is perpendicular to velocity
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 dragDirection = -liftVelocity.normalized;
        Vector3 inducedDrag = dragForce * inducedDragPower * v2 * dragDirection;

        return lift + inducedDrag;
    }

    private void UpdateLift()
    {
        if (LocalVelocity.sqrMagnitude < 1.0f) return; // too slow

        Vector3 upForce = CalculateLift(AngleOfAttackPitch, Vector3.up, verticalPower, inducedDragPower, aoaCurvePitch);
        Vector3 sideForce = CalculateLift(AngleOfAttackPitch, Vector3.right, horizontalPower, inducedDragPower, aoaCurveYaw);

        Rigidbody.AddRelativeForce(upForce);
        Rigidbody.AddRelativeForce(sideForce);
    }

}
