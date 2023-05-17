using System.Collections;
using UnityEngine;

public class Explode : MonoBehaviour
{

    private static readonly Collider[] _colliders = new Collider[20];

    public ParticleSystem[] systems;

    [SerializeField] float damage = 60;
    [SerializeField] float maxRange = 5;
    //[SerializeField] float minRange = 1;
    [SerializeField] LayerMask layerMask;

    [SerializeField] float force = 1000;
    [SerializeField] DamageType type;
    //[SerializeField] bool explodeOnStart = true;

    void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>();
    }

    void Start()
    {
        //if(explodeOnStart) ExplodeNow();    
    }

    public void Initialize()
    {
        //if(explodeOnStart) ExplodeNow();    
    }

    public void ExplodeNow( TeamMember owner = null )
    {
        ExplodeNow(damage, maxRange, force, owner);
    }


    public void ExplodeNow(float maxDamage, float maxRange, float force, TeamMember owner = null)
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
        }

        Vector3 exposionCenter = transform.position;

        int nrHits = Physics.OverlapSphereNonAlloc(exposionCenter, maxRange, _colliders, layerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < nrHits; i++) //if a gameObj have multiple colliders it will be hit mutiple times ...-.-
        {
            Rigidbody rb = _colliders[i].attachedRigidbody;
            if (!_colliders[i].TryGetComponent(out Health health))
            {
                health = _colliders[i].GetComponentInParent<Health>();
            }

            if (health != null)
            {
                Vector3 closestPoint = _colliders[i].ClosestPoint(exposionCenter); // if we do distance to collider transform.pos we might not "hit" if collider is big, still touching part of it
                float dist = Vector3.Distance(closestPoint, exposionCenter); //maybe ray cast to see if nothing is between first?
                float damage = (1 - dist / maxRange) * maxDamage;

                health.DealDamage(new DamageInfo(owner, damage, type, exposionCenter, force, maxRange));

                if (health.Dead && rb != null)
                {
                    rb.AddExplosionForce(force, exposionCenter, maxRange);

                }
            }

            if (rb != null)
            {
                rb.AddExplosionForce(force, exposionCenter, maxRange);
            }
        }


        StartCoroutine(ReturnToPool());
    }


    private IEnumerator DelayedExplosionForce(Rigidbody rigidbody, Vector3 exposionCenter)
    {
        yield return null;
        rigidbody.AddExplosionForce(force, exposionCenter, maxRange);
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Stop();
            systems[i].Clear();
        }
        Destroy(gameObject);
    }
}
