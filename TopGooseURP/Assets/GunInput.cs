using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInput : MonoBehaviour
{
    public Gun[] guns;
    // Start is called before the first frame update
    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        if(guns == null) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].Fire = Input.GetMouseButton(0);
        }
        
    }
}
