using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class BoatController : MonoBehaviour
{
    [SerializeField]
    float accelerationForce;

    [SerializeField]
    float turnForce;

    [SerializeField]
    float maxVelocity;

    [SerializeField]
    float turnSpeed;
    float turnVelocity;

    [SerializeField]
    Transform target;

    Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rigidBody.velocity.magnitude < maxVelocity) rigidBody.AddForce(transform.forward * accelerationForce, ForceMode.Force);

        //Get the angle that the boat wants to face.
        Vector3 targetDirection = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        //Turn boat towards that targetAngle slowly.
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //If the angle differs more than 30 degrees from the targetAngle, slow down.
        if (Mathf.Abs(targetAngle - angle) % 360 > 30) rigidBody.AddForce(-rigidBody.velocity * 0.5f);
    }
}
