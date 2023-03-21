using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingAI : MonoBehaviour, IUtility
{
    [SerializeField] private Rigidbody targetRigidbody; //to be dynamically allocated
    [SerializeField] private Transform targetTransform; //to be dynamically allocated
    [SerializeField] private GameObject targetGameObject; //Use target interface/sqript instead
    private bool movingTarget = false;

    [SerializeField] float maxInterceptDistance = 5000;
    [SerializeField] float minInterceptDistance = 50;
    [SerializeField] float minDistance = 5;

    [SerializeField][Range(0.0f, 1.0f)] float ramming = 0;
    [SerializeField][Range(0.0f, 1.0f)] float guns = 0;
    [SerializeField][Range(0.0f, 45.0f)] float gunsConeToFire = 1.0f;
    [SerializeField][Range(0.9f, 1.0f)] float gunsAlignToFire = .99f;
    [SerializeField][Range(0.0f, 1.0f)] float minHeat = 0.3f;
    [SerializeField][Range(0.0f, 1.0f)] float maxHeat = 0.9f;
    bool gunsOverheat;

    private Vector3 ramTarget; // vector to ram target
    private Vector3 gunSolutionTarget; // vector to get guns on target target
    private Vector3 flyTarget = Vector3.zero; // actual fly towards target
    [SerializeField] private float bulletSpeed = 800; //to be allocated from Guns attached later

    private Rigidbody selfRigidbody;
    private Autopilot autopilot;
    private FlightController controller;
    private Gun[] gunArray;

    public bool showDebugInfo;

    void Start()
    {
        selfRigidbody = GetComponent<Rigidbody>();
        autopilot = GetComponent<Autopilot>();
        controller = GetComponent<FlightController>();
        controller.SetThrottleInput(1.0f);
        SetTarget(targetGameObject);
        gunArray = GetComponentsInChildren<Gun>();
        gunsAlignToFire = Mathf.Cos(gunsConeToFire * Mathf.Deg2Rad);

    }

    public void SetTarget(GameObject gameObject)
    {
        targetGameObject = gameObject;
        if (gameObject.TryGetComponent(out Rigidbody rigidbody))
        {
            targetRigidbody = rigidbody;
            movingTarget = true;
        }
        else
        {
            movingTarget = false;
        }
        targetTransform = gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void UpdateGuns()
    {
        if(gunArray == null) return;
        bool fire = false;
        Vector3 fwd = transform.forward;

        //Vector3 distToTarget = (flyTarget - transform.position);


        Vector3 dirToAim = (flyTarget - transform.position).normalized;
        if (Vector3.Dot(fwd, dirToAim) > gunsAlignToFire && !gunsOverheat)
        {
            fire = true;
        }

        for (int i = 0; i < gunArray.Length; i++)
        {
            gunArray[i].Fire = fire;
            if (gunArray[i].Heat > maxHeat) gunsOverheat = true;
            else if (gunArray[i].Heat < minHeat) gunsOverheat = false;
        }
    }

    private void UpdateAutopilot()
    {
        autopilot.RunAutopilot(flyTarget, out float pitch, out float yaw, out float roll);
        Vector3 input = new(pitch, yaw, roll);
        controller.SetControlInput(input);
    }

    private void TrackTarget()
    {
        //return if no target
        if (targetGameObject == null || !targetGameObject.activeSelf) return;

        Vector3 tPos = targetTransform.position;
        Vector3 tVel = movingTarget?targetRigidbody.velocity:Vector3.zero;
        float pSpeed = selfRigidbody.velocity.magnitude;
        flyTarget = tPos;
        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, pSpeed, out ramTarget, out float tti))
        {
            flyTarget = Vector3.Lerp(flyTarget, ramTarget, ramming);
            Debug.Assert(tti >= 0, "tti: " + tti);
        }
        if (TargetingMath.ComputeImpact(tPos, tVel, transform.position, bulletSpeed, out gunSolutionTarget, out float _))
        {
            flyTarget = Vector3.Lerp(flyTarget, gunSolutionTarget, guns);
        }

    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo == true)
        {
            Color oldColor = Gizmos.color;

           
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(flyTarget, 10f);
            

            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(gunSolutionTarget, 10f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ramTarget, 5f);


            Gizmos.color = oldColor;
        }
    }

    public float Evaluate()
    {
        if (!targetGameObject) return 0;
        

        return 1.0f;
    }

    public void Execute()
    {
        TrackTarget();
        UpdateAutopilot();
        UpdateGuns();
    }

    public void AddGameObject(GameObject gameObject)
    {
        //
    }
}
