using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BirdAnimation : MonoBehaviour
{
    private FlightController flightController;

    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;
    [SerializeField] private float strengthRollWing = 20;

    [SerializeField] private Transform tailWing;

    [Space]
    [SerializeField] private float maxPitch = 45;
    //[SerializeField] private float speedPitch = 45;
    [SerializeField] private float accelerationPitch = 45;
    //[SerializeField] private float strengthYaw = 0;
    [SerializeField] private float strengthRoll = 20;
    [Space]
    [SerializeField] private bool useFixed = false;


    private Vector3 leftOriginal;
    private Vector3 rightOriginal;
    private Vector3 tailOriginal;
    private Vector3 tailDeflection;
    private float tailDeflectionPitch;

    private void Start()
    {
        if (!TryGetComponent(out flightController))
        {
            Debug.LogWarning("BirdAnimation - Attatched to a object without FlightController, how will that work?");
            enabled = false;
        }

        if (TryGetComponent(out RagdollHandler ragdollHandler))
        {
            ragdollHandler.onRagdollEnable += DisableAnimations;
        }
        leftOriginal = leftWing.localEulerAngles;
        rightOriginal = rightWing.localEulerAngles;
        tailOriginal = tailWing.localEulerAngles;
    }

    private void DisableAnimations()
    {
        enabled = false;
    }

    private void Update()
    {
        if (!useFixed) DoAnimationMagic(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (useFixed) DoAnimationMagic(Time.fixedDeltaTime);
    }

    private void DoAnimationMagic(float dt)
    {
        Vector3 steering = flightController.Steering; //stering input of the controller, x is pitch, y is yaw, and x is roll.

        float tailRoll = 0;
        if (Mathf.Abs(steering.z) > 0.005f) //over some threashold do some roll anim
        {
            float wingRoll = steering.z * strengthRollWing;
            Vector3 wingDeflection = new(wingRoll, 0, 0);
            leftWing.localEulerAngles = leftOriginal - wingDeflection;
            rightWing.localEulerAngles = rightOriginal + wingDeflection;
            tailRoll = steering.z * strengthRoll;
        }

        float localEulerX = ClampAngle( tailWing.localEulerAngles.x );
        float pitch = 0;
        if (Mathf.Abs(steering.x) > 0.08f)
        {
            pitch = steering.x * -maxPitch;
        }

        float error = AngleDifference(pitch, localEulerX);

        float a = accelerationPitch * dt;
        pitch =  Mathf.Clamp(error, -a, a);
        tailDeflectionPitch += pitch;

        //Debug.Log("pitch = " + pitch + " localEulerX = " + localEulerX);
        Vector3 steer = new(tailDeflectionPitch, tailRoll, 0); //tail in rig is turned wrong :/ y is roll and z is yaw


       // float roll = steering.z * strengthRoll;

        tailWing.localEulerAngles = steer;
    }

    private float ClampAngle(float angle)
    {
        if(angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private static float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;   //calculate modular difference, and remap to [-180, 180]
    }
}
