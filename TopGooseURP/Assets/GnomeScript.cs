using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeScript : MonoBehaviour
{
    
    private Rigidbody rb;

    //[SerializeField] private float dragPower = 1;
    [SerializeField] private float explodeTimer = 1;
    [SerializeField] private float offset = .3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] GameObject explosionPrefab; 

    private PickUp pickUp;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickUp = GetComponent<PickUp>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(!pickUp.CanBePickedUp) HandleDrop(Time.fixedDeltaTime);
        
    }

    private void HandleDrop(float dt)
    {
        if(!rb.isKinematic) transform.up = rb.velocity;
        if(Physics.Raycast(transform.position, transform.up,out RaycastHit hit, rb.velocity.magnitude * dt, groundLayer)){
            transform.position = hit.point - offset * transform.up;
            rb.isKinematic = true;
            StartCoroutine(Explode());
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

}
