using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        for (int i = 0; i < length; i++)
        {
            if(Physics.Raycast(initialPos, initialVel, out RaycastHit hit, initialVel.magnitude*1.05f, layer))
            {
                if(decalProjector)
                    decalProjector.transform.position = hit.point;
                
                break;
            }
            else
            {
                Vector3 prevPos = initialPos;
                initialVel += gravity * timeStep * timeStep;
                initialPos += initialVel;
                Debug.DrawLine(prevPos, initialPos);
            }
        }
    }
}
