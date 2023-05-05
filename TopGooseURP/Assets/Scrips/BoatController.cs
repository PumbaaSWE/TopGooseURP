using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
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

    Vector3 travelAxis;
    Vector3 distanceFromShore;
    Vector3 targetPos;

    Rigidbody rigidBody;
    Transform boundary;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        boundary = transform.parent.GetComponentInChildren<MapBoundary>().gameObject.transform;

        if(Mathf.Abs(boundary.position.x - transform.position.x) < Mathf.Abs(boundary.position.z - transform.position.z))
        {
            travelAxis = Vector3.right;

            distanceFromShore = Vector3.forward;
            distanceFromShore *= (transform.position.z - boundary.position.z > 0) ? boundary.position.z + boundary.localScale.z / 2 : boundary.position.z - boundary.localScale.z / 2;
        }
        else
        {
            travelAxis = Vector3.forward;

            distanceFromShore = Vector3.right;
            distanceFromShore *= (transform.position.x - boundary.position.x > 0) ? boundary.position.x + boundary.localScale.x / 2 : boundary.position.x - boundary.localScale.x / 2;
        }
    }

    private void FixedUpdate()
    {
        //Drive da boat
        if (rigidBody.velocity.magnitude < maxVelocity) rigidBody.AddForce(transform.forward * accelerationForce, ForceMode.Force);

        //Sets the target along the side of the island that the boat was on at start
        targetPos = Vector3.Scale(target.position, travelAxis);
        targetPos += distanceFromShore;

        //Get the angle that the boat wants to face.
        Vector3 targetDirection = (targetPos - transform.position).normalized;
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        //Turn boat towards that targetAngle slowly.
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //If the angle differs more than 30 degrees from the targetAngle, slow down so you don't drift like crazy.
        if (Mathf.Abs(targetAngle - angle) % 360 > 30) rigidBody.AddForce(-rigidBody.velocity * 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if(targetPos != Vector3.zero) Gizmos.DrawLine(transform.position, targetPos);
    }
}
