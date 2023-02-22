using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnimation : MonoBehaviour
{

    [SerializeField] private FlightController controller;
    [SerializeField] private Transform tail;
    [SerializeField] private float strengthPitch = 45;
    [SerializeField] private float strengthRoll = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float pitch = controller.Steering.x * strengthPitch;
        float roll = controller.Steering.z * strengthRoll;

        Vector3 steer = new (pitch, 0, roll);
        tail.localEulerAngles = steer;
    }
}
