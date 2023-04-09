using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjective : Objective
{

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Completed();
    }
}
