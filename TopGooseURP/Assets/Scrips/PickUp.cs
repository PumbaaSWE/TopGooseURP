using System;
using UnityEngine;

public class PickUp : MonoBehaviour
{


    private Rigidbody rb;

    private bool isDropped = false;
    private bool isCarried = false;
    private Transform carrier;

    public delegate void OnPickUpEvent(Transform carrier);
    public OnPickUpEvent OnPickUp;

    public delegate void OnDropEvent(Vector3 velocity);
    public OnDropEvent OnDrop;

    public delegate void OnCarryEvent(Vector3 position, Quaternion rotation);
    public OnCarryEvent OnCarry;

    public delegate void OnInteractEvent();
    public OnInteractEvent OnInteract;

    public bool CanBePickedUp => !isDropped;
    //public bool IsDropped => isDropped;
    public bool AutoCarry { get; set; } = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCarried)
        {
            OnCarry?.Invoke(carrier.position, carrier.rotation);
            if(AutoCarry) transform.SetLocalPositionAndRotation(carrier.position, carrier.rotation);
        }
    }


    public void PickedUp(Transform carrier)
    {
        this.carrier = carrier;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        isCarried = true;
        isDropped = false;
        OnPickUp?.Invoke(carrier);
    }

    public void Drop(Vector3 velocity)
    {
        carrier = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        isCarried = false;
        isDropped = true;
        rb.velocity = velocity;
        OnDrop?.Invoke(velocity);
    }
}
