using UnityEngine;


[CreateAssetMenu(menuName = "MissileData")]
public class MissileData : ScriptableObject
{
    [Header("Miscing")]
    [Tooltip("Auto destruct in seconds")] public float timeToLive = 5;
    public float trailTime = 5;

    [Header("Thrusting")]
    [Tooltip("Speeeeeeeed!!!")]
    public float maxSpeed = 25; //m/s
    [Tooltip("(currently unused)m/s^2??")]
    public float maxAcceleration = 100;
    [Tooltip("Poweeeeerrr!!!")]
    public float maxThrust = 100;

    [Header("Steering")]
    [Tooltip("Deg per sec; x: pitch, y: yaw, z: roll")]public Vector3 turnSpeed = new(180, 180, 360);
    [Tooltip("x: pitch, y: yaw, z: roll")] public Vector3 turnAcceleration = new(999, 999, 999);
    [Tooltip("Set to one if unsure")] public AnimationCurve steeringCurve;

    [Header("Autopiloting")]
    [Tooltip("Strength for autopilot flight.")] public float strength = 2f;
    [Tooltip("Angle at which airplane banks fully into target.")] public float aggressiveTurnAngle = 2f;

    [Header("Damaging")]
    public float damage = 40;
    public float proxyFuseRange = 1.0f;

    [Header("Seeking")]
    public float seekerRange = 250;
    [Range(1, 89)] public float seekerFOV = 45;
    [Range(0, 1)] public float seekerRangeInfluence = 1;
    [Range(0, 1)] public float seekerOffBoreInfluence = 0.1f;

    public LayerMask targetLayer;


}
