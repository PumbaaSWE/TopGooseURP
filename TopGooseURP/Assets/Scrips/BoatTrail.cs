using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTrail : MonoBehaviour
{
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3 (transform.position.x, startPos.y, transform.position.z);
    }
}
