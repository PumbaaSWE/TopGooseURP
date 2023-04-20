using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    
    RagdollHandler ragdollHandler;
    FlightController flightController;
    Health health;
    Rigidbody rigidBody;

    Renderer renderer;

    [SerializeField] GameObject feathers;

    [SerializeField] float dissolveSpeed;
    [SerializeField] float startDissolvingAfter;
    [SerializeField] float spin;
    [SerializeField] float spinPreviousUpdate;
    [SerializeField] float spinPreviousPreviousUpdate;

    [SerializeField] Vector3 angularVelocity;

    float t;
    bool dissolve, ragdoll, dead, counterClockWise;

    private void Start()
    {
        ragdollHandler = GetComponent<RagdollHandler>();
        flightController = GetComponent<FlightController>();
        health = GetComponent<Health>();
        rigidBody = GetComponent<Rigidbody>();

        renderer = GetComponentInChildren<Renderer>();

        ragdollHandler.onRagdollEnable += OnRagdoll;
        health.AddDeathEvent(OnDeathDo);
    }

    void Update()
    {
        //When not dead, keep track of the previous - and, well - previous previous spin in order to determine which direction to roll on death
        if(health.health > 0)
        {
            spinPreviousPreviousUpdate = spinPreviousUpdate;
            spinPreviousUpdate = spin;
            spin = transform.rotation.eulerAngles.z;

            //For now, die when pressing space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                health.ChangeHealth(-99999);
            }
        }
        

        //If you haven't died yet, you shall not pass!
        if (!dead) return;

        //If you haven't ragdolled yet, roll! (Jack might dislike this because physics are handled outside of fixedUpdate)
        if (!ragdoll)
        {
            spin += (counterClockWise) ? Time.deltaTime * 180 : Time.deltaTime * -180;
            transform.forward = rigidBody.velocity;
            transform.Rotate(Vector3.forward * spin, Space.Self);
        }

        //If not dissolving yet, count down time until dissolve
        else if (!dissolve)
        {
            startDissolvingAfter -= Time.deltaTime;

            if (startDissolvingAfter < 0)
                dissolve = true;
        }

        //If dissolving and not dissolved yet, add to "t"-value
        else if (t < 1)
        {
            t += dissolveSpeed * Time.deltaTime;
            renderer.material.SetFloat("_T", t);
        }

        //When fully dissolved, remove gameObject from the scene
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnRagdoll()
    {
        dead = true;
        ragdoll = true;
    }

    private void OnDeathDo()
    {
        rigidBody.AddRelativeTorque(new Vector3(0, 0, 0.2f), ForceMode.Impulse);
        dead = true;
        flightController.enabled = false;

        if (spin > spinPreviousPreviousUpdate)
            counterClockWise = true;

        var feathersInstance = Instantiate(feathers, gameObject.transform.position, Quaternion.identity);
        feathersInstance.transform.parent = gameObject.transform;
    }
}
