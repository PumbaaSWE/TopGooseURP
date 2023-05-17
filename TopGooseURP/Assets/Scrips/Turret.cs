using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Turret : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform barrel;
    [SerializeField]
    Transform canon;

    [SerializeField]
    float range;

    [SerializeField]
    BulletData bulletData;

    Gun gun;

    Rigidbody targetRb;
    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody>();
        gun = GetComponentInChildren<Gun>();
        gun.Fire = true;

        GetComponent<Health>().OnDead += () => 
        {
            gun.Fire = false;
            enabled = false;
        };
    }

    

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            
            return;
        }
        if (Vector3.Distance(target.position, transform.position) < range)
        {
            TargetingMath.ComputeImpact(target.position, targetRb.velocity, transform.position, gun.BulletSpeed, out Vector3 location, out float _);

            Vector3 direction = location - barrel.position;
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);

            barrel.localEulerAngles = new Vector3(rotation.eulerAngles.x, 0, 0);
            canon.localEulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
            gun.Fire = true;
        }
        else
        {
            gun.Fire = false;
        }
    }
}
