using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtility : MonoBehaviour, IUtility
{

    public WaypointsPath currentPath;
    public float acceptableDistance;
    public float maxDistance = 100;

    private Autopilot autopilot;
    FlightController flightController;

    private int waypointId = 0;
    private Vector3 currentWaypoint;
    private float distFromWaypoint;

    public float Evaluate()
    {
        if(currentPath == null) return -1f;
        distFromWaypoint = Vector3.Distance(currentWaypoint, transform.position);

        if (distFromWaypoint > maxDistance) //if we fly really far off find the nearest one
        {
            currentWaypoint = currentPath.GetClosest(transform.position, ref waypointId);
            distFromWaypoint = Vector3.Distance(currentWaypoint, transform.position);
        }

        float ratio = distFromWaypoint / maxDistance; // to be used to compute final utility score, i.e. the further away from waypoint the highter utility
        //maybe better from closest waypoint and or the center i.e. currentPath.pos

        return 1.0f;
    }

    public void Execute()
    {
        if(distFromWaypoint < acceptableDistance)
        {
            currentWaypoint = currentPath.GetNext(ref waypointId);
        }
        autopilot.FlyTo(currentWaypoint);
        //autopilot.RunAutopilot(currentWaypoint, out float pitch, out float yaw, out float roll);
        //flightController.SetControlInput(new Vector3(pitch, yaw, roll));
    }

    // Start is called before the first frame update
    void Start()
    {
        autopilot = GetComponent<Autopilot>();
        flightController = GetComponent<FlightController>();
        currentWaypoint = currentPath.GetClosest(transform.position, ref waypointId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
