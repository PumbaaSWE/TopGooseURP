using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractice : MonoBehaviour
{

    Vector3 startPos;
    public Vector3 direction = new Vector3(1,0,0);
    public float range = 150;
    public float minSpeed = 15;
    public float maxSpeed = 15;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        speed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.position += speed * Time.fixedDeltaTime * direction.normalized;
        if(Vector3.Distance(transform.position, startPos) > range)
        {
            speed = -speed;
        }
    }
}
