using System.Collections;
using UnityEngine;

public class Explode : MonoBehaviour
{

    public ParticleSystem[] systems;

    void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>();
    }

    public void ExplodeNow()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
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
