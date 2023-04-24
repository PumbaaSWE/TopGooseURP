using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    Collider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }

    public void OnTriggerExit(Collider other)
    {
        //other.BroadcastMessage("ExitPlayableArea");

        if(/*some check to see if collider belongs to player*/ false)
        {
            //activate missiles in ship(s)
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //other.BroadcastMessage("EnteredPlayableArea");
    }
}
