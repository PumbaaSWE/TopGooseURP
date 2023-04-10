using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    private Collider[] colliders;
    private Rigidbody[] rigidbodies;

    private Collider[] hitBox;
    private Rigidbody Rigidbody;

    [Header("Rigidbodies")]
    [SerializeField] private float drag = 1;
    [SerializeField] private CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Continuous;
    [SerializeField] private float speedMul = .5f;
    [SerializeField] private float triggerForce = 5;
    //[SerializeField] private float gravityMod = 1;
    [Space]
    [Header("Joints")]
    [SerializeField] private bool changeJoints = true;
    [SerializeField] private bool enableProjection = true;
    [SerializeField] private float projectionDistance = .5f;
    [SerializeField] private float projectionAngle = 90;
    [SerializeField] private bool enablePreprocessing = false;

    private bool active; //Make property

    public delegate void OnRagdollEnable ();
    public OnRagdollEnable onRagdollEnable;

    Vector3 storedVelocity;
    Vector3 storedAngularVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
        
        GetRagdollBits();

    }

    void FixedUpdate()
    {

        storedVelocity = Rigidbody.velocity;
        storedAngularVelocity = Rigidbody.angularVelocity;

    }

    private void GetRagdollBits()
    {
        //Get hitbox and rigit body of this object
        Rigidbody = GetComponentInParent<Rigidbody>();
        hitBox = GetComponentsInParent<Collider>();

        //ragdolly bits are in children but GetComponentsInChildren also gets this objects things so need to exclude them...
        colliders = GetComponentsInChildren<Collider>(true).Except(hitBox).ToArray();

        List<Rigidbody> rigidbodies = GetComponentsInChildren<Rigidbody>(true).ToList();
        rigidbodies.Remove(Rigidbody);
        this.rigidbodies = rigidbodies.ToArray();

        for (int i = 0; i < rigidbodies.Count; i++)
        {
            rigidbodies[i].drag = drag;
            rigidbodies[i].collisionDetectionMode = collisionDetectionMode;
            //set more initial values
        }

        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>(true);
        for (int i = 0; i < joints.Length && changeJoints; i++)
        {
            joints[i].enableProjection = enableProjection;
            joints[i].projectionDistance = projectionDistance;
            joints[i].projectionAngle = projectionAngle;
            joints[i].enablePreprocessing = enablePreprocessing;
            //set more initial values
        }

        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        onRagdollEnable?.Invoke();
        //disable this objects
        for (int i = 0; i < hitBox.Length; i++)
        {
            hitBox[i].enabled = false;
        }
        Rigidbody.isKinematic = true;


        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].velocity = storedVelocity * speedMul;
        }
    }

    //resetting the ragdoll need an animation or saving relative positions+rotations of starting "pose"
    public void DisableRagdoll()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = true;
        }
        //reverse order
        for (int i = 0; i < hitBox.Length; i++)
        {
            hitBox[i].enabled = true;
        }
        Rigidbody.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impulse = collision.impulse.magnitude;
        if(impulse >= triggerForce * Time.fixedDeltaTime)
        {
            
            Debug.Log("RagdollHandler - Enabling Ragdoll, Impulse magnitude: " + impulse + " computed impact force(I think): " + impulse / Time.fixedDeltaTime);
            EnableRagdoll();
        }
        
        
        
    }
}
