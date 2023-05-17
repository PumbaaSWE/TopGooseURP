using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMultipleObjective : Objective
{
    
    public List<Health> toKill = new();
    
    // Start is called before the first frame update
    void Start()
    {
        if(count < 2)
            count = toKill.Count;
        for (int i = 0; i < toKill.Count; i++)
        {
            toKill[i].OnDead += CountOne;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
