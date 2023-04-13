using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        gun = GetComponentInChildren<Gun>();
        gun.Fire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) < range)
        {
            Vector3 direction = target.position - barrel.position;
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
