using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeekerHead), typeof(SimpleFlight), typeof(Rigidbody))]
public class Missile : MonoBehaviour
{

    [SerializeField] private MissileData missileData;

    [Tooltip("The explosion to be spawned when close to target, decides damage and range for this missile")][SerializeField] private Explode explosion;
    public Rigidbody Rigidbody { get; private set; }
    public SeekerHead SeekerHead { get; private set; }
    public SimpleFlight SimpleFlight { get; private set; }

    public FlappyWingsModel flappyWings;
    public Trail trail;
    public Collider Collider { get; private set; }

    private SphereCollider proxyRange;

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

    /// <summary>
    /// Initialize the missile with custom data and owner that should be credited with potential kills or assist.
    /// </summary>
    /// <param name="missileData">The data applied to this missile</param>
    /// <param name="owner">owner to be credited </param>
    /// <param name="enabledCollider">true by deafault, the collider of this missile</param>
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
    /// <summary>
    /// Initialize the missile with owner that should be credited with potential kills or assist. Data is whatever data missile is set to in inspector
    /// </summary>
    public void Initialize(TeamMember owner)
    {
        Initialize(missileData, owner);
    }

    /// <summary>
    /// Launch this missile with specified initial velocity, throttle can also be set
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <param name="intialThrottle"></param>
    public void Launch(Vector3 initialVelocity, float intialThrottle = 1.0f)
    {
        Detatch(initialVelocity);
        SeekerHead.Launch();
        SimpleFlight.SetThrottleInput(intialThrottle);
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

    /// <summary>
    /// COROUTINE: Will make the missile able to detect things close by and explode in their vicinity after t seconds
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private IEnumerator ArmProxyFuse(float t)
    {
        yield return new WaitForSeconds(t);
        proxyRange.enabled = true;
        Collider.enabled = true;
    }

    /// <summary>
    /// Will spawn an explotion and pass on the missiles owner to it, tyhen destroy the missile game object. Damage and radius is whatever the explosion prefab is set to
    /// </summary>
    public void Explode()
    {
        gameObject.SetActive(false);
        if(explosion != null)
            Instantiate(explosion, transform.position, transform.rotation).ExplodeNow(owner);

        Destroy(gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if ((missileData.targetLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Explode();
        }
    }
}
