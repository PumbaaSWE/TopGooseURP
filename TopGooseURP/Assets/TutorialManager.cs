using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject goose;
    
    // Start is called before the first frame update
    void Start()
    {
        goose.GetComponent<FlightController>();
        goose.GetComponent<Health>();
        //goose.GetComponentsInChildren<Gun>;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
