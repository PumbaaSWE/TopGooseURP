using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Bomb : MonoBehaviour
{

    public GameObject explosionPrefab;
    public LayerMask layer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, rb.velocity, out RaycastHit hit ,rb.velocity.magnitude*Time.fixedDeltaTime, layer))
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject explosion = Instantiate(explosionPrefab, hit.point, rotation * explosionPrefab.transform.rotation);
            //explosion.GetComponent<Explode>().ExplodeNow();
            StartCoroutine(ReturnToPool(explosion));
            gameObject.SetActive(false);
            Destroy(explosion, 5);
        }
    }

    private IEnumerator ReturnToPool(GameObject explosion)
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
