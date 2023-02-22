using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeScript : MonoBehaviour
{
    
    private Rigidbody rb;

    bool isDropped = false;
    bool isCarried = false;
    Transform carrier;
    [SerializeField] private float dragPower = 1;
    [SerializeField] private float explodeTimer = 1;
    [SerializeField] private float offset = .3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] GameObject explosionPrefab; 

    public bool CanBePickedUp => !isDropped;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (isDropped)
        {
            HandleDrop(dt);
        }else if (isCarried)
        {
            transform.SetLocalPositionAndRotation(carrier.position, carrier.rotation);
        }
        else
        {
            //for testing..
            //transform.up = Vector3.Lerp(transform.up, rb.velocity.normalized, dt * dragPower);
        }
    }

    private void HandleDrop(float dt)
    {
        transform.up = Vector3.Lerp(transform.up, rb.velocity.normalized, dt * dragPower);
        if(Physics.Raycast(transform.position, transform.up,out RaycastHit hit, rb.velocity.magnitude * dt, groundLayer)){
            transform.position = hit.point - offset * transform.up;
            rb.isKinematic = true;
            StartCoroutine(Explode());
            isDropped = false;
        }
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTimer);
        //spawn explosion
        //destroy/return to pool
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); //for now
    }

    public void PickUp(Transform carrier)
    {
        this.carrier = carrier;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        isCarried = true;
        isDropped = false;
    }

    public void Drop(Vector3 velocity)
    {
        carrier = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        isCarried = false;
        isDropped = true;
        rb.velocity = velocity;
    }
}
