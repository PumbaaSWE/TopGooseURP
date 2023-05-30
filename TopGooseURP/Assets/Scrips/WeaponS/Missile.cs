using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeekerHead), typeof(SimpleFlight), typeof(Rigidbody))]
public class Missile : MonoBehaviour
{

    [SerializeField] private MissileData missileData;

    [SerializeField] private Explode explosion;
    public Rigidbody Rigidbody { get; private set; }
    public SeekerHead SeekerHead { get; private set; }
    public SimpleFlight SimpleFlight { get; private set; }

    public FlappyWingsModel flappyWings;
    public Trail trail;
    public Collider Collider { get; private set; }

    private SphereCollider proxyRange;

    //private Transform hardpoint;
    //private float lifeTime;

    private TeamMember owner;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        SeekerHead = GetComponent<SeekerHead>();
        SimpleFlight = GetComponent<SimpleFlight>();
        Collider = GetComponent<Collider>();
        TryGetComponent(out flappyWings);
        //TryGetComponent(out trail);
        trail = GetComponentInChildren<Trail>();
        proxyRange = gameObject.AddComponent<SphereCollider>();
        proxyRange.isTrigger = true;
        //Initialize(missileData);
    }

    public void Initialize(MissileData missileData, TeamMember owner, bool enabledCollider = true)
    {
        this.missileData = missileData;
        this.owner = owner;
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

    public void Initialize(TeamMember owner)
    {
        Initialize(missileData, owner);
    }

    //public void SetHardpoint(Transform hardpoint)
    //{
    //    this.hardpoint = hardpoint;
    //    transform.SetPositionAndRotation(hardpoint.position, hardpoint.rotation);
    //}

    public void Launch(Vector3 initialVelocity, float intialThrottle = 1.0f)
    {
        //Rigidbody.isKinematic = false;
        //Rigidbody.velocity = initialVelocity;
        Detatch(initialVelocity);
        SeekerHead.Launch();
        SimpleFlight.SetThrottleInput(intialThrottle);
        //hardpoint = null;
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
        //Destroy(gameObject, missileData.timeToLive);
    }

    /// <summary>
    /// Will detach the missile i.e. make the rigidbody active an set its velocity and time to live.
    /// </summary>
    /// <param name="initialVelocity"></param>
    public void Detatch(Vector3 initialVelocity)
    {
        Rigidbody.isKinematic = false;
        Rigidbody.velocity = initialVelocity;
        Destroy(gameObject, missileData.timeToLive);
    }


    void Update()
    {
        //float dt = Time.deltaTime;
        //lifeTime += dt;
        //if(lifeTime > missileData.timeToLive) Explode();

    }

    void LateUpdate()
    {
        //if(hardpoint != null)
        //    transform.SetPositionAndRotation(hardpoint.position, hardpoint.rotation);
    }

    private IEnumerator ArmProxyFuse(float t)
    {
        yield return new WaitForSeconds(t);
        //Debug.LogWarning("ProxyFuse Enabled");
        proxyRange.enabled = true;
        Collider.enabled = true;
    }


    public void Explode()
    {
        gameObject.SetActive(false);
        //spawn explosion?
        //return to a pool?
        if(explosion != null)
            Instantiate(explosion, transform.position, transform.rotation).ExplodeNow(owner);

        Destroy(gameObject);
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    //if(!SeekerHead.Launched) return;
    //    if ((missileData.targetLayer.value & (1 << other.transform.gameObject.layer)) > 0)
    //    {
    //        Debug.Log("Hit with Layermask");
    //        //Explode();
    //    }
    //    else
    //    {
    //        Debug.Log("Hit something, but not in Layermask");
    //    }
    //}
    public void OnTriggerExit(Collider other)
    {
        //if(!SeekerHead.Launched) return;
        if ((missileData.targetLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Explode();
        }
        else
        {
            
        }
    }
}
