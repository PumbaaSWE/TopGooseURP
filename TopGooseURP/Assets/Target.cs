using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float speed = 50;
    public float range = 400;
    public float turn = .01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.forward;
        
        transform.forward = Vector3.RotateTowards(transform.forward, transform.forward + transform.right, turn, 0);
    }
}
