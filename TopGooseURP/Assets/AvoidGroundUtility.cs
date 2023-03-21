using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidGroundUtility : MonoBehaviour, IUtility
{
    FlightController fc;


    public float Evaluate()
    {
        return 0f;
    }

    public void Execute()
    {
        Debug.Log("Executing AvoidGroundUtil");
    }
}
