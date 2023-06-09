using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointsPath : MonoBehaviour
{
    private List<Vector3> waypoints = new List<Vector3>();

    public List<Transform> waypointsTransforms = new List<Transform>();
    public bool drawDebugInfo;


    /// <summary>
    /// Gets the world space position of the next waypoint on this path in relation to referenced id of prevoius point
    /// </summary>
    /// <param name="waypointId"></param>
    /// <returns></returns>
    public Vector3 GetNext(ref int waypointId)
    {
        waypointId = (waypointId + 1) % waypointsTransforms.Count;
        return waypointsTransforms[waypointId].position;
        //waypointId = (waypointId + 1) % waypoints.Count;
        //return waypoints[waypointId];
    }
    /// <summary>
    /// Gets the world space position of the closet waypoint on this path to specified postion, also allows a reference to the id of this point to be sent
    /// </summary>
    /// <param name="postion"></param>
    /// <param name="waypointId"></param>
    /// <returns></returns>
    public Vector3 GetClosest(Vector3 postion, ref int waypointId)
    {
        float bestDist = float.MaxValue;
        int bestId = waypointId;
        for (int i = 0; i < waypointsTransforms.Count; i++)
        {
            float dist = Vector3.Distance(waypointsTransforms[i].position, postion);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestId = i;
            }

        }
        waypointId = bestId;
        return waypointsTransforms[bestId].position;
    }

    // Start is called before the first frame update
    void Awake()
    {
        waypointsTransforms = GetComponentsInChildren<Transform>().ToList();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnDrawGizmos()
    {
        if (drawDebugInfo)
        {
            Gizmos.color = Color.yellow;
            Vector3 from = waypointsTransforms[^1].position;
            for (int i = 0; i < waypointsTransforms.Count; i++)
            {
                Vector3 to = waypointsTransforms[i].position;
                Gizmos.DrawLine(from, to);
                from = to;
            }

        }

    }
}
