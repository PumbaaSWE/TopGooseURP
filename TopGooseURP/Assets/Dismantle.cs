using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Dismantle : MonoBehaviour
{

    Health health;
    Slicer slicer;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        slicer = GetComponent<Slicer>();
        health.OnDead += Slice;
    }

    private void Slice()
    {
        slicer.split = true;
    }
}