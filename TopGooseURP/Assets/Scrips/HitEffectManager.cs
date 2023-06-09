using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HitEffectManager : MonoBehaviour
{
    [SerializeField] private List<ImpactEffect> impactEffects = new();
    private readonly Dictionary<Material, ObjectPool<ParticleSystem>> impacts = new();

    private static HitEffectManager instance; //singleton bad?
    public static HitEffectManager Instance => instance; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            foreach (ImpactEffect effect in impactEffects)
            {
                effect.Init(transform);
                impacts.Add(effect.Material, effect.GetPool());
            }
            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // just incase som lunatic changes it
            transform.localScale = Vector3.one;
        }
        else
        {
            Destroy(this);
        }
        
    }

    /// <summary>
    /// Spawn a hit effect based/linked to the Material provided, if no match it will spawn the first in the list
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    /// <param name="material"></param>
    public void SpawnEffect(Vector3 position, Vector3 normal, Material material)
    {

        if (impacts.TryGetValue(material, out ObjectPool<ParticleSystem> pool))
        {
            ParticleSystem system = pool.Get();
            SpawnEffect(position, normal, system);
            StartCoroutine(ReturnToPool(pool, system));
        }
        else
        {
            pool = impactEffects[0].GetPool();
            ParticleSystem system = pool.Get();
            SpawnEffect(position, normal, system); // default
            StartCoroutine(ReturnToPool(pool, system));
        }
    }

    /// <summary>
    /// Tell specified ParticleSystem to emit at position and in the direction of the normal
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    /// <param name="system"></param>
    public void SpawnEffect(Vector3 position, Vector3 normal, ParticleSystem system)
    {
        system.transform.position = position;
        system.transform.forward = normal;
        system.Emit(1);
    }

    private IEnumerator ReturnToPool(ObjectPool<ParticleSystem> pool, ParticleSystem system)
    {
        yield return new WaitForSeconds(system.main.duration); //wait until the ParticleSystem is done and disapreared...
        pool.Release(system);

    }

    /// <summary>
    /// Spawn the first hit effect in effect list
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    internal void SpawnEffect(Vector3 position, Vector3 normal)
    {
        ObjectPool<ParticleSystem> pool = impactEffects[0].GetPool();
        ParticleSystem system = pool.Get();
        SpawnEffect(position, normal, system); // default
        StartCoroutine(ReturnToPool(pool, system));
    }

    /// <summary>
    /// Spawn a hit effect based in the id in effect list instead if serching for a material, will clamp the id to fit list to avoid out of range
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    /// <param name="id"></param>
    internal void SpawnEffect(Vector3 position, Vector3 normal, int id)
    {
        id = Math.Clamp(id, 0, impactEffects.Count-1);
        ObjectPool<ParticleSystem> pool = impactEffects[id].GetPool();
        ParticleSystem system = pool.Get();
        SpawnEffect(position, normal, system); // default
        StartCoroutine(ReturnToPool(pool, system));
    }

    [Serializable]
    internal class ImpactEffect
    {
        [SerializeField] private Material material;
        [SerializeField] private ParticleSystem ImpactPrefab;

        public Material Material => material;
        private ObjectPool<ParticleSystem> pariclePool;
        private Transform parent;

        internal ObjectPool<ParticleSystem> GetPool()
        {
            return pariclePool;
        }

        private ParticleSystem OnCreateFunc()
        {
            return Instantiate(ImpactPrefab, parent);
        }

        internal void Init(Transform parent)
        {
            this.parent = parent;
            pariclePool ??= new ObjectPool<ParticleSystem>(OnCreateFunc, pool => pool.gameObject.SetActive(true), pool => pool.gameObject.SetActive(false));
        }
    }
}
