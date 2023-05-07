using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

[RequireComponent(typeof(SimpleFlight))]
[RequireComponent(typeof(Rigidbody))]
public class BoatMissile : MonoBehaviour
{
    SimpleFlight simpleFlight;
    Rigidbody rigidBody;
    Rigidbody targetRigidBody;
    Transform target;

    [SerializeField]
    GameObject explosionPrefab;

    Vector3 desiredDirection;

    void Start()
    {
        simpleFlight = GetComponent<SimpleFlight>();
        rigidBody = GetComponent<Rigidbody>();
        simpleFlight.SetThrottleInput(1);
    }

    private void Update()
    {
        desiredDirection = DesiredTravelDirection(transform.position, simpleFlight.MaxSpeed, target.position, targetRigidBody.velocity);

        simpleFlight.RunAutopilot(transform.position + desiredDirection * rigidBody.velocity.magnitude);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        targetRigidBody = target.GetComponent<Rigidbody>();
    }

    private Vector3 DesiredTravelDirection(Vector3 pos, float maxSpeed, Vector3 targetPos, Vector3 targetVel)
    {
        Vector3 C = targetPos + targetVel * Time.deltaTime; //Circle center
        Vector3 LOS = targetPos - pos; //Line of sight

        //float a = LOS.x * LOS.x + LOS.y * LOS.y + LOS.z * LOS.z;
        //float b = 2 * LOS.x * (pos.x - C.x) + 2 * LOS.y * (pos.y - C.y) + 2 * LOS.z * (pos.z - C.z);
        //float c = (pos.x - C.x) * (pos.x - C.x) + (pos.y - C.y) * (pos.y - C.y) + (pos.z - C.z) * (pos.z - C.z) - maxSpeed * Time.deltaTime * maxSpeed * Time.deltaTime;

        float a = Vector3.Dot(LOS, LOS);
        float b = Vector3.Dot(LOS, pos - C) * 2;
        float c = Vector3.Dot(pos - C, pos - C) - Mathf.Pow(maxSpeed * Time.deltaTime, 2);

        if (b * b - 4 * a * c <= 0)
            return (targetPos - pos).normalized;

        float t = 2 * c / (-b + Mathf.Sqrt(b * b - 4 * a * c));

        if (t > 1 || t < 0)
            return (targetPos - pos).normalized;

        Vector3 pointOnLOS = LOS * t + pos;
        return (C - pointOnLOS).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 10);

            GetComponent<SphereCollider>().enabled = false;
            transform.GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
            transform.GetComponentInChildren<ParticleSystem>().enableEmission = false;
            Destroy(gameObject, 10);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + desiredDirection * rigidBody.velocity.magnitude);
    //}
}
