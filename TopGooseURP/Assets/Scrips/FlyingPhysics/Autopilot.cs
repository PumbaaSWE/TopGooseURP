using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Autopilot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private FlightController controller;
    [SerializeField] private Transform aircraft;

    [Header("PIDing")]
    [SerializeField] private PIDController pitchPID = new() 
    { 
        proportionalGain = 5,
        derivativeGain = -1
    };
    [SerializeField] private PIDController yawPID = new()
    {
        proportionalGain = 5,
        derivativeGain = -1
    };
    [Tooltip("Affected by strength as well!")]
    [SerializeField] private PIDController rollPID = new()
    {
        proportionalGain = 5,
        derivativeGain = 0
    };

    [SerializeField]
    private PIDController speedPID = new()
    {
        proportionalGain = 1,
        derivativeGain = 1
    };

    [Header("Autopiloting")]
    [Tooltip("Strength for autopilot flight.")][SerializeField] private float strength = 5f;
    [Tooltip("Angle at which airplane banks fully into target.")][SerializeField] private float aggressiveTurnAngle = 10f;
    [Tooltip("AI only, limit pitch down manuevers.")][SerializeField] private float pitchUpThreshold = 15f;
    [Space]
    [Tooltip("DEBUG")][SerializeField] private bool showDebugInfo;

    private float yaw;
    private float pitch;
    private float roll;

    public float Yaw => yaw;
    public float Pitch => pitch;
    public float Roll => roll;

    public Vector3 Output { get; private set; }
    //public Vector3 output { get; private set; }

    public void RunAutopilot(Vector3 flyTarget, out float pitch, out float yaw, out float roll)
    {
        // This is my usual trick of converting the fly to position to local space.
        // You can derive a lot of information from where the target is relative to self.
        Vector3 localFlyTarget = aircraft.InverseTransformPoint(flyTarget).normalized * strength;
        float angleOffTarget = Vector3.Angle(aircraft.forward, flyTarget - aircraft.position);

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
        float wingsLevelRoll = aircraft.right.y;

        // Blend between auto level and banking into the target.
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        roll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
    }
    Vector3 pitchError;
    public void RunAutopilot2(Vector3 flyTarget, out float pitch, out float yaw, out float roll)
    {
        Vector3 localFlyTarget = aircraft.InverseTransformPoint(flyTarget).normalized * strength;



        float angleOffTarget = Vector3.Angle(aircraft.forward, flyTarget - aircraft.position);


        yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        float wingsLevelRoll = aircraft.right.y;
        float agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        //Vector3
        pitchError = new Vector3(0, localFlyTarget.y, localFlyTarget.z).normalized;

        float pitchAngle = Vector3.SignedAngle(transform.forward, pitchError, transform.right);
        if (pitchAngle > pitchUpThreshold)
        {
            //agressiveRoll = Mathf.Ceil(agressiveRoll);
        }
        pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);



        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        roll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
        Output = new Vector3(pitch, yaw, roll);
        //Debug.Log("Output: " +Output);
    }

    public void RunAdvancedAutopilot2(Vector3 flyTarget, float dt, out float pitch, out float yaw, out float roll)
    {
        // This is my usual trick of converting the fly to position to local space.
        // You can derive a lot of information from where the target is relative to self.
        Vector3 localFlyTarget = aircraft.InverseTransformPoint(flyTarget).normalized;
        float angleOffTarget = Vector3.Angle(aircraft.forward, flyTarget - aircraft.position);

        yaw = yawPID.UpdatePID(-localFlyTarget.x, 0, dt);

        pitch = pitchPID.UpdatePID(localFlyTarget.y, 0, dt); //target is zero, when we fly straight at the target x and y is 0 (then z is distance to target kinda for free... just fun side note :P)

        //float agressiveRoll = Mathf.Clamp(localFlyTarget.x * rollPID.proportionalGain, -1f, 1f);
        float agressiveRoll =  Mathf.Clamp(localFlyTarget.x * strength, -1f, 1f);

        // A "wings level roll" is a roll commands the aircraft to fly wings level.
        // This can be done by zeroing out the Y component of the aircraft's right.
        float wingsLevelRoll = aircraft.right.y;

        // Blend between auto level and banking into the target.
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        roll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);

        //roll = rollPID.UpdatePID(0, targetRoll, dt);
        roll = rollPID.UpdatePID(wingsLevelRoll, -agressiveRoll, dt); //roll is the acctual angle we want?
    }

    public void FlyTo(Vector3 flyTarget, bool useAdvanced = false)
    {
        float pitch, yaw, roll; 
        if (useAdvanced)
        {
            RunAdvancedAutopilot2(flyTarget, Time.fixedDeltaTime, out pitch, out yaw, out roll);
        }
        else
        {
            RunAutopilot(flyTarget, out pitch, out yaw, out roll);
        }
        controller.SetControlInput(new Vector3(pitch, yaw, roll));
    }

    public void MatchSpeed(float speed, float dt)
    {
        float throttle = speedPID.UpdatePID(controller.LocalVelocity.z, speed, dt);
        controller.SetThrottleInput(throttle);
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * pitchError * 2);
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position, transform.position + Output * 2);
        }
    }
}
