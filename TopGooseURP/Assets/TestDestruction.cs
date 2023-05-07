using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDestruction : MonoBehaviour
{


    Rigidbody[] rigidbodies;
    public GameObject foundation;
    public GameObject model;
    public float force = 100;
    public float radius = 100;
    public float density = 1;
    //bool destryed

    //also do fade broken pices with  disolve mat shader.

    void Awake()
    {
        //foundation = 
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].gameObject.SetActive(false);
            Vector3 size = rigidbodies[i].gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
            float volume = Mathf.Max(size.x * size.y * size.z, 1.0f);

            rigidbodies[i].mass = volume * density;
        }
        foundation.SetActive(false);
        radius = model.GetComponent<Collider>().bounds.extents.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartDestruction();
        }
    }

    private void StartDestruction()
    {
        model.SetActive(false);
        foundation.SetActive(true);
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].gameObject.SetActive(true);
            if(force > float.Epsilon)
                rigidbodies[i].AddExplosionForce(force, transform.position, radius);
        }
        enabled = false; // temp
    }
}
