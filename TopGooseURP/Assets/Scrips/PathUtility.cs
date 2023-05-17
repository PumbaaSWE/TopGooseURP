using UnityEditor;
using UnityEngine;

public class PathUtility : MonoBehaviour, IUtility
{

    public WaypointsPath currentPath;
    public float acceptableDistance;
    public float maxDistance = 100;

    //private Autopilot autopilot;
    //FlightController flightController;

    private int waypointId = 0;
    private Vector3 currentWaypoint;
    private Vector3 prevWaypoint;
    private float distFromPath;

    public bool showDebugInfo;

    public float Evaluate()
    {
        if(currentPath == null) return -1f;
        //distFromWaypoint = Vector3.Distance(currentWaypoint, transform.position);

        distFromPath = HandleUtility.DistancePointLine(transform.position, prevWaypoint, currentWaypoint);
        if (showDebugInfo) Debug.Log("Distance From  Path: " + distFromPath);
        if (distFromPath > maxDistance) //if we fly really far off find the nearest one
        {
            prevWaypoint = currentWaypoint;
            currentWaypoint = currentPath.GetClosest(transform.position, ref waypointId);
            if (showDebugInfo) Debug.Log("GetClosest Path: " + waypointId);
            //distFromWaypoint = Vector3.Distance(currentWaypoint, transform.position);
        }

        float ratio = distFromPath / maxDistance; // to be used to compute final utility score, i.e. the further away from waypoint the highter utility
        //maybe better from closest waypoint and or the center i.e. currentPath.pos
        if (showDebugInfo) Debug.Log("Evaluate Path: " + ratio);
        return ratio;
    }

    public void Execute()
    {
        float distFromWaypoint = Vector3.Distance(currentWaypoint, transform.position);
        if (distFromWaypoint < acceptableDistance)
        {
            if(showDebugInfo)Debug.Log("GetNext Path: " + waypointId);
            prevWaypoint = currentWaypoint;
            currentWaypoint = currentPath.GetNext(ref waypointId);
            if (showDebugInfo) Debug.Log("GotNext Path: " + waypointId);
        }

        actor.SetFlyTarget(currentWaypoint);

        //autopilot.FlyTo(currentWaypoint);
        //autopilot.RunAutopilot(currentWaypoint, out float pitch, out float yaw, out float roll);
        //flightController.SetControlInput(new Vector3(pitch, yaw, roll));
    }


    AIActor actor;
    // Start is called before the first frame update
    void Start()
    {
        //autopilot = GetComponent<Autopilot>();
        //flightController = GetComponent<FlightController>();
        currentWaypoint = currentPath.GetClosest(transform.position, ref waypointId);
        prevWaypoint = currentWaypoint;
        actor = GetComponent<AIActor>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentWaypoint);
            Gizmos.DrawWireSphere(currentWaypoint, 3);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, prevWaypoint);
            Gizmos.DrawWireSphere(prevWaypoint, 3);
        }
    }
}
