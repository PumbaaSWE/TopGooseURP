using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float gravity = 10;
    //[SerializeField] private HitEffectManager hitEffectManager;

    private BulletData data;
    private Vector3 nextPosition;
    private Vector3 velocity;
    private float time;

    private TrailRenderer trail;
    private ObjectPool<Bullet> bulletPool;
    private bool returned;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        //hitEffectManager = Instantiate(hitEffectManager);
    }

    public void Init(BulletData data, Vector3 position, Quaternion rotation)
    {
        this.data = data;
        transform.SetPositionAndRotation(position, rotation);
        if(data.trailTime > 0)
        {
            trail.enabled = true;
            trail.emitting = true;
            trail.time = data.trailTime;
            trail.Clear();
        }
        else
        {
            trail.enabled = false;
            trail.emitting = false;
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
        if(time <= 0)
        {
            StartCoroutine(ReturnToPool());
            return;
        }
        velocity.y -= gravity * Time.fixedDeltaTime; //v = a * dt
        nextPosition = transform.position + velocity * Time.fixedDeltaTime;  //p = v * dt
        RayCast();
    }

    private void RayCast()
    {

        if (Physics.Raycast(transform.position, velocity, out RaycastHit hit, velocity.magnitude, layerMask)) {

            transform.position = hit.point;

            //null pointers? no collider(?) no renderer?
            if( hit.collider.gameObject.TryGetComponent(out Renderer renderer))
            {
                Material material = renderer.sharedMaterial;
                HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal, material);
            }

            if(hit.collider.gameObject.TryGetComponent(out Health health))
            {
                health.ChangeHealth(-data.damage);
            }

            HitEffectManager.Instance.SpawnEffect(hit.point, hit.normal);
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
        yield return new WaitForSeconds(data.trailTime); //wait until the trail is done and disapreared...
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
