using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{

    public GameObject missile;
    public Vector3 initVel = new Vector3(0, 0, 10);
    public float maxSpeed = 25;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject go = Instantiate(missile, transform.position + transform.up, transform.rotation);

            if(go.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.velocity = rb.velocity;
            }
            else
            {
                Debug.LogWarning("MissileLauncher: cannot access the rigidbody component of the missile");
            }

            Destroy(go, 5.0f);
        }
    }
}
