using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Dismantle : MonoBehaviour
{

    MeshFilter[] allVisible;
    Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();

        allVisible = GetComponentsInChildren<MeshFilter>();
        health.OnDead += AddRigidbodies;
    }

    private void AddRigidbodies()
    {
        for (int i = 0; i < allVisible.Length; i++)
        {
            allVisible[i].gameObject.AddComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
