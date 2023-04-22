using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFeatherCloud : MonoBehaviour
{

    [SerializeField] GameObject particleCloud;

    // Start is called before the first frame update
    void Start()
    {
        Health health = GetComponent<Health>();
        health.OnDead += SpawnParticleCloud;
    }

    public void SpawnParticleCloud()
    {
        particleCloud.transform.position = gameObject.transform.position;
        Instantiate(particleCloud);
        Destroy(particleCloud, 5);

    }
}
