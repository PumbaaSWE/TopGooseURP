using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

public class BombComputer : MonoBehaviour
{
    public DecalProjector decalProjector;
    Rigidbody rb;
    public int length = 100;
    public LayerMask layer;
    public float timeStep = .5f;

    void Start()
    {
        //decalProjector = GetComponentInChildren<DecalProjector>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 initialPos = transform.position;
        Vector3 initialVel = rb.velocity * timeStep;
        Vector3 gravity = Physics.gravity;
        bool hitGround = false;
        for (int i = 0; i < length; i++)
        {
            initialVel += timeStep * timeStep * gravity;
            if (Physics.Raycast(initialPos, initialVel, out RaycastHit hit, initialVel.magnitude, layer))
            {
                if(decalProjector)
                    decalProjector.transform.position = hit.point;

                //Debug.DrawLine(hit.point, hit.point+Vector3.up*10, Color.yellow);
                hitGround = true;
                break;
            }
            Debug.DrawLine(initialPos, initialPos + initialVel, Color.magenta);
            initialPos += initialVel;
            //Debug.DrawLine(prevPos, initialPos);
            
        }
        Debug.Assert(hitGround, "Why not hit ground?");
    }
}
