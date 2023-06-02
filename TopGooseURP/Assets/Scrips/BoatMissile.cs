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
    AudioSource audioSource;
    Rigidbody targetRigidBody;
    Transform target;

    [SerializeField]
    Explode explosionPrefab;

    [SerializeField]
    private float engineStart = 0.5f;

    [SerializeField]
    private float damage = 1000f;
    [SerializeField]
    private float radius = 5.0f;
    [SerializeField]
    private float force = 100f;

    Vector3 desiredDirection;

    float explodeAfter;
    bool exploded;

    ParticleSystem.EmissionModule emission;

    void Start()
    {
        simpleFlight = GetComponent<SimpleFlight>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        //simpleFlight.SetThrottleInput(1);
        emission = GetComponentInChildren<ParticleSystem>().emission;
        transform.forward = Vector3.up;
        rigidBody.velocity = Vector3.up * simpleFlight.MaxSpeed;
        //TargetingMath.ComputeImpact(target.position, targetRigidBody.velocity, transform.position, simpleFlight.MaxSpeed, out Vector3 location, out explodeAfter);
        StartCoroutine(EngineStart(engineStart));
        enabled = false;
    }

    private IEnumerator EngineStart(float t)
    {
        yield return new WaitForSeconds(t);
        simpleFlight.SetThrottleInput(1);
        TargetingMath.ComputeImpact(target.position, targetRigidBody.velocity, transform.position, simpleFlight.MaxSpeed, out Vector3 _, out explodeAfter);
        enabled = true;
    }

    private void Update()
    {
        if (exploded) return;

        explodeAfter -= Time.deltaTime;
        if (explodeAfter < 0)
        {
            exploded = true;
            Explode();
        }

        desiredDirection = DesiredTravelDirection(transform.position, simpleFlight.MaxSpeed, target.position, targetRigidBody.velocity);

        simpleFlight.RunAutopilot(transform.position + desiredDirection * rigidBody.velocity.magnitude);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        targetRigidBody = target.GetComponent<Rigidbody>();
    }

    //Awesome homing missile activities
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

    //Explode when near target
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject && !exploded)
        {
            exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        audioSource.enabled = false;
        Explode explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.ExplodeNow(damage, radius, force);

        GetComponent<SphereCollider>().enabled = false;
        transform.GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
        emission.enabled = false;
        Destroy(gameObject, 10);
    }
}
