using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    //[SerializeField] private LayerMask layerMask;
    //[SerializeField] private float gravity = 10;
    //[SerializeField] private DamageType type;
    //[SerializeField] private HitEffectManager hitEffectManager;

    private BulletData data;
    private Vector3 nextPosition;
    private Vector3 velocity;
    private float time;

    private TrailRenderer trail;
    private ObjectPool<Bullet> bulletPool;
    private bool returned;

    private TeamMember owner;

    [SerializeField]private LayerMask birdLayer;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        //hitEffectManager = Instantiate(hitEffectManager);
    }

    /// <summary>
    /// Initialize with BulletData and owner is the one to get credit for kill/assists
    /// </summary>
    /// <param name="data">data, damage and spped and all that</param>
    /// <param name="owner">credit for kill/assists</param>
    /// <param name="position">Where to begin</param>
    /// <param name="rotation">Direction to fly in</param>
    public void Init(BulletData data, TeamMember owner, Vector3 position, Quaternion rotation)
    {
        this.data = data;
        this.owner = owner;
        transform.SetPositionAndRotation(position, rotation);
        if (data.hasTrail)
        {
            trail.time = data.trailConfig.time;
            trail.material = data.trailConfig.material;
            trail.colorGradient = data.trailConfig.color;
            trail.widthCurve = data.trailConfig.width;
            trail.minVertexDistance = data.trailConfig.minVertDistance;
            trail.numCapVertices = data.trailConfig.endCapVerices;
            trail.numCornerVertices = data.trailConfig.cornerVerices;
            trail.shadowCastingMode = data.trailConfig.castShadow;
        }
        velocity = transform.forward * data.speed;
        time = data.timeToLive;
        returned = false;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (returned) return;

        time -= Time.fixedDeltaTime;
        if (time <= 0)
        {
            StartCoroutine(ReturnToPool());
            return;
        }
        velocity.y -= data.gravity * Time.fixedDeltaTime; //v = a * dt
        nextPosition = transform.position + velocity * Time.fixedDeltaTime;  //p = v * dt
        RayCast();
    }

    /// <summary>
    /// Will look for impacts along the flight path and spawn hit effects and if applicaple deal damage on hit.
    /// </summary>
    private void RayCast()
    {

        if (Physics.Raycast(transform.position, velocity, out RaycastHit hit, velocity.magnitude * Time.fixedDeltaTime, data.hitLayer, QueryTriggerInteraction.Ignore))
        {

            transform.position = hit.point;

            //null pointers? no collider(?) no renderer?
            if (hit.collider.gameObject.TryGetComponent(out Renderer renderer))
            {
                Material material = renderer.sharedMaterial;
                HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal, material);
            }
            else
            {
                //renderer = hit.collider.gameObject.GetComponentInChildren<Renderer>();
                //Debug.Log("birdLayer " + birdLayer + " birdLayer.value " + birdLayer.value);
                //Debug.Log("hit.transform.gameObject.layer " + hit.transform.gameObject.layer);

                if ((birdLayer.value & (1 << hit.transform.gameObject.layer)) > 0)
                {
                    HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal, 3);
                    //Debug.Log("Layer HIT " + hit.collider.gameObject.name);
                }
                else
                {
                    HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal);
                    //Debug.Log("No Mat HIT " + hit.collider.gameObject.name);
                }
            }

            //hit.collider.gameObject.layer

            //if ((data.hitLayer.value & (1 << other.transform.gameObject.layer)) > 0)
            //{

            //}


            if (hit.collider.gameObject.TryGetComponent(out Health health))
            {
                //health.ChangeHealth(-data.damage, ChangeHealthType.impact, owner);
                health.DealDamage(new DamageInfo(owner, data.damage, data.damageType, hit.point));
            }
            else
            {

                health = hit.collider.gameObject.GetComponentInParent<Health>();

                if (health != null)
                {
                    health.DealDamage(new DamageInfo(owner, data.damage, data.damageType, hit.point));
                    //health.ChangeHealth(-data.damage, ChangeHealthType.impact, owner);
                }
            }

            //HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal);
            StartCoroutine(ReturnToPool());
        }
        else
        {
            transform.position = nextPosition;
        }
    }
    private IEnumerator ReturnToPool()
    {
        returned = true;
        yield return new WaitForSeconds(data.trailConfig.time); //wait until the trail is done and disapreared...
        bulletPool.Release(this);
        gameObject.SetActive(false);

    }

    internal void SetPool(ObjectPool<Bullet> bulletPool)
    {
        this.bulletPool = bulletPool;
    }

    //internal void SetEffectManager(HitEffectManager hitEffectManager)
    //{
    //    this.hitEffectManager = hitEffectManager;
    //}
}
