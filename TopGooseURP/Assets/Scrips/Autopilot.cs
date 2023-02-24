using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autopilot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private FlightController controller;
    [SerializeField] private Transform aircraft;

    [Header("PIDing")]
    [SerializeField] private PIDController pitchPID;
    [SerializeField] private PIDController rollPID;
    [SerializeField] private PIDController yawPID;

    [Header("Autopiloting")]
    [Tooltip("Strength for autopilot flight.")][SerializeField] private float strength = 5f;
    [Tooltip("Angle at which airplane banks fully into target.")][SerializeField] private float aggressiveTurnAngle = 10f;

    private float yaw;
    private float pitch;
    private float roll;

    public float Yaw => yaw;
    public float Pitch => pitch;
    public float Roll => roll;

    public Vector3 Output { get; private set; }

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

    public void RunAdvancedAutopilot(Vector3 flyTarget, float dt)
    {
        // This is my usual trick of converting the fly to position to local space.
        // You can derive a lot of information from where the target is relative to self.
        Vector3 localFlyTarget = aircraft.InverseTransformPoint(flyTarget).normalized;
        float angleOffTarget = Vector3.Angle(aircraft.forward, flyTarget - aircraft.position);

        yaw = yawPID.UpdatePID(-localFlyTarget.x, 0 , dt);

        pitch = pitchPID.UpdatePID(localFlyTarget.y, 0, dt);

        float agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

        // A "wings level roll" is a roll commands the aircraft to fly wings level.
        // This can be done by zeroing out the Y component of the aircraft's right.
        float wingsLevelRoll = aircraft.right.y;

        // Blend between auto level and banking into the target.
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        float targetRoll = -Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);

        roll = rollPID.UpdatePID(0, targetRoll, dt);
        Output = new Vector3(pitch, yaw, targetRoll);
    }
}
