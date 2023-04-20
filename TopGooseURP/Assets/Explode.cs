using System.Collections;
using UnityEngine;

public class Explode : MonoBehaviour
{

    private static Collider[] _colliders = new Collider[20];

    public ParticleSystem[] systems;

    [SerializeField] float damage = 60;
    [SerializeField] float maxRange = 5;
    [SerializeField] float minRange = 1;
    [SerializeField] LayerMask layerMask;

    [SerializeField] float force = 1000;
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

    public void ExplodeNow()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
        }

        Vector3 exposionCenter = transform.position;

        int nrHits = Physics.OverlapSphereNonAlloc(exposionCenter, maxRange, _colliders, layerMask);

        for (int i = 0; i < nrHits; i++) //if a gameObj have multiple colliders it will be hit mutiple times ...-.-
        {
            Rigidbody rb = _colliders[i].attachedRigidbody;
            if (_colliders[i].TryGetComponent(out Health health))
            {
                Vector3 closestPoint = _colliders[i].ClosestPoint(exposionCenter); // if we do distance to collider transform.pos we might not "hit" if collider is big, still touching part of it
                float dist = Vector3.Distance(closestPoint, exposionCenter); //maybe ray cast to see if nothing is between first?
                float damage = (1 - dist / maxRange) * this.damage;

                health.ChangeHealth(-damage);
                if (health.dead && rb != null)
                {
                    rb.AddExplosionForce(force, exposionCenter, maxRange);
                }
            }
            else if(rb != null)
            {
                rb.AddExplosionForce(force, exposionCenter, maxRange);
            }
        }
        

        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Stop();
            systems[i].Clear();
        }
    }
}
