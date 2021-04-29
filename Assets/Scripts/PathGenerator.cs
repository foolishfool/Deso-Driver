using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    public List<GameObject> Points;
    private WaypointsHolder wayPointHolder;
    // Start is called before the first frame update
    void Start()
    {
        wayPointHolder = GetComponent<WaypointsHolder>();
        generatePath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generatePath()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            wayPointHolder.CreateWaypoint(Points[i].transform.position,"waypoint", Points[i]);
            Points[i].GetComponent<Point>().BelongedWaypointHolders.Add(wayPointHolder);
        }
    }

    public bool ContainsPoint(GameObject point)
    {
        if (Points.Contains(point))
        {
            return true;
        }
        return false;
    }
}
