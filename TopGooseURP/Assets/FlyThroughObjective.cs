using UnityEngine;

public class FlyThroughObjective : Objective
{


    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            CountOne();
    }
}
