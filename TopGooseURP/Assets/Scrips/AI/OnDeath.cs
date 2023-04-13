using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(FlightController))]
public class OnDeath : MonoBehaviour
{
    public float despawnTime = 20;
    public float fadeTime = 3;
    public ParticleSystem goingDown;

    private FlightController flightController;


    void Start()
    {
        GetComponent<Health>().AddDeathEvent(OnDead);
        flightController = GetComponent<FlightController>();
        enabled = false;
        goingDown.Stop();
    }

    private void OnDead()
    {
        enabled = true;
        goingDown.Play();
        flightController.SetThrottleInput(0);
        Destroy(gameObject, despawnTime);
    }

    // Update is called once per frame
    void Update()
    { 
        flightController.SetControlInput(Vector3.forward);
    }
}
