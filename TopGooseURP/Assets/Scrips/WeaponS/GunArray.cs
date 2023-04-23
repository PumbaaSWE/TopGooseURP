using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunArray : MonoBehaviour
{

    public Gun[] guns;

    // Start is called before the first frame update
    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        if (guns == null) gameObject.SetActive(false);
    }

    public void FireAll(bool fire)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].Fire = fire;
        }
    }


    public void ZeroGunsAtPoint(Vector3 aimPoint)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].ZeroAt(aimPoint);
        }
    }

    public void ZeroGunsAtRange(float range)
    {
        ZeroGunsAtPoint(transform.position + transform.forward * range);
    }
}
