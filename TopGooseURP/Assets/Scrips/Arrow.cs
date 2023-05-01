using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    [Range(0f, 5f)]
    float rotationSpeed, positionOffset;

    Quaternion rotation;

    private void Start()
    {
        rotation = transform.rotation;
    }

    void Update()
    {
        rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        transform.position = transform.parent.position + (target.position - transform.position).normalized * positionOffset;
    }
    private void LateUpdate()
    {
        transform.rotation = rotation;
    }
}
