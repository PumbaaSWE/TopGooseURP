using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPredictor : MonoBehaviour
{
    Vector3 prevVel;
    Vector3 prevPos;
    Rigidbody rigid;
    public float timeToPredict = 10;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float time = Time.fixedDeltaTime;
        Vector3 velocity = (transform.position - prevPos) / time;
        Vector3 acceleration = (rigid.velocity - prevVel) / time;

        Vector3 prevDrawPoint = transform.position;
        for (float simulationTime = time; simulationTime <= timeToPredict; simulationTime+=time)
        {
            Vector3 displacement = rigid.velocity * simulationTime + simulationTime * simulationTime * acceleration / 2;
            Vector3 drawPoint = transform.position + displacement;
            Debug.DrawLine(prevDrawPoint, drawPoint, Color.green);
            prevDrawPoint = drawPoint;
        }


        prevPos = transform.position;
        prevVel = rigid.velocity;
    }


}
