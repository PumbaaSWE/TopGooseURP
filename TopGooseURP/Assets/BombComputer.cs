using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

public class BombComputer : MonoBehaviour
{
    [Tooltip("What is projected on the ground")]
    [SerializeField] private DecalProjector decalProjector;
    [Tooltip("Where are bombs dropped from? Uses objects transform if empty")]
    [SerializeField] private Transform dropPoint;
    [Tooltip("How many iterations, related to how far down do we search for impact")]
    [SerializeField] private int length = 100;
    [Tooltip("Where to stop, dont apply on units etc.")]
    [SerializeField] private LayerMask layer;
    [Tooltip("High is more effective and reach further but less accurate")]
    [SerializeField] private float timeStep = .5f;

    private Rigidbody rb;
    private BombBay bombBay;
    public bool Active { get; private set; }

    void Awake()
    {
        //decalProjector = GetComponentInChildren<DecalProjector>();
        rb = GetComponent<Rigidbody>();
        if (!dropPoint)
        {
            dropPoint = transform;
        }

        if (!TryGetComponent(out bombBay))
        {
            Debug.LogWarning("WeaponSystem - Missing Bomb Bay script on this game object!");
        }
        bombBay.OnActivationChange += Enable;
    }
    private void OnDestroy()
    {
        bombBay.OnActivationChange -= Enable;
    }
    private void Enable(bool enable)
    {
        enabled = enable;
        decalProjector.gameObject.SetActive(enable);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 initialPos = transform.position;
        Vector3 initialVel = rb.velocity * timeStep;
        Vector3 gravity = Physics.gravity;
        for (int i = 0; i < length; i++)
        {
            initialVel += timeStep * timeStep * gravity;
            if (Physics.Raycast(initialPos, initialVel, out RaycastHit hit, initialVel.magnitude, layer))
            {
                if(decalProjector)
                    decalProjector.transform.position = hit.point;
                break;
            }
            Debug.DrawLine(initialPos, initialPos + initialVel, Color.magenta);
            initialPos += initialVel;     
        }
    }
}
