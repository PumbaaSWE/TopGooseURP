using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeekerHead), typeof(SimpleFlight), typeof(Rigidbody))]
public class Missile : MonoBehaviour
{

    [SerializeField]private MissileData missileData; 
    public Rigidbody Rigidbody { get; private set; }
    public SeekerHead SeekerHead { get; private set; }
    public SimpleFlight SimpleFlight { get; private set; }

    public FlappyWings flappyWings;
    public Trail trail;
    public Collider Collider { get; private set; }

    private SphereCollider proxyRange;

    private Transform hardpoint;
    //private float lifeTime;


    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        SeekerHead = GetComponent<SeekerHead>();
        SimpleFlight = GetComponent<SimpleFlight>();
        Collider = GetComponent<Collider>();
        TryGetComponent(out flappyWings);
        TryGetComponent(out trail);
        proxyRange = gameObject.AddComponent<SphereCollider>();

        //Initialize(missileData);
    }

    public void Initialize(MissileData missileData, bool enabledCollider = false)
    {
        this.missileData = missileData;
        proxyRange.radius = missileData.proxyFuseRange;
        proxyRange.enabled = false;
        Rigidbody.isKinematic = true;
        Collider.enabled = enabledCollider;
        if (flappyWings != null)
        {
            flappyWings.Flap(false);
        }
        if(trail != null)
        {
            trail.Emitting(false);
            //trail.
        }
        
    }

    public void SetHardpoint(Transform hardpoint)
    {
        this.hardpoint = hardpoint;
        transform.SetPositionAndRotation(hardpoint.position, hardpoint.rotation);
    }

    public void Launch(Vector3 initialVelocity, float intialThrottle = 0)
    {
        Rigidbody.isKinematic = false;
        Rigidbody.velocity = initialVelocity;
        SeekerHead.Launch();
        SimpleFlight.SetThrottleInput(intialThrottle);
        hardpoint = null;
        if(flappyWings != null)
        {
            flappyWings.Flap(true);
        }
        if (trail != null)
        {
            trail.Emitting(true);
        }
        if (missileData.proxyFuseArmTime > 0) StartCoroutine(ArmProxyFuse(missileData.proxyFuseArmTime));
        enabled = false; // remove if this prevents OnTriggerEnter from being called!
        //lifeTime = 0;
    }

    void Update()
    {
        //float dt = Time.deltaTime;
        //if()
    }

    void LateUpdate()
    {
        if(hardpoint != null)
            transform.SetPositionAndRotation(hardpoint.position, hardpoint.rotation);
    }

    private IEnumerator ArmProxyFuse(float t)
    {
        yield return new WaitForSeconds(t);
        proxyRange.enabled = true;
    }


    public void Explode()
    {
        gameObject.SetActive(false);
        //spawn explosion?
        //return to a pool?
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        //if(!SeekerHead.Launched) return;
        if ((missileData.targetLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit with Layermask");
            Explode();
        }
        else
        {
            Debug.Log("Hit something, but not in Layermask");
        }
    }
}
