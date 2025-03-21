using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    float depthBeforeSubmerged;
    
    [SerializeField]
    float displacementAmount;
    
    [SerializeField]
    float yOffset;

    Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()  //Simple script to simulate the bobbing up and down motion to look like a floating object.
    {
        if(transform.position.y < yOffset)
        {
            float displacementMultipler = Mathf.Clamp01((-transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidBody.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y) * displacementMultipler, ForceMode.Force);
        }
    }
}
