using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour
{
    // Start is called before the first frame update

    public int force = 10000;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
        //GetComponent<Rigidbody>().AddTorque(transform.forward * 100);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //GetComponent<Rigidbody>().AddTorque(transform.forward * 10);
    }
}
