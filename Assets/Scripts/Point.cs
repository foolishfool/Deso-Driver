using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Point : MonoBehaviour
{

    public Vector3 Direction;
    public GameObject StartPoint;
    public List<WaypointsHolder> BelongedWaypointHolders;
    // Start is called before the first frame update
    void Start()
    {
        Direction = transform.GetChild(0).position - gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Renderer>().isVisible)
        {
            if (!GameController.Instance.CheckPoints.Contains(gameObject))
            {
                GameController.Instance.CheckPoints.Add(gameObject);
            }

        }
        else GameController.Instance.CheckPoints.Remove(gameObject);

    }


    public WaypointsHolder GetSameWaypointHolder(Point StartPoint)
    {
        for (int i = 0; i < BelongedWaypointHolders.Count; i++)
        {
           // Debug.Log(StartPoint.name);
            //return a hoder that contains both start and target
            if (WayPointHasObj(BelongedWaypointHolders[i], StartPoint.gameObject))
            {
                return BelongedWaypointHolders[i];
            }

        }
        Debug.Log("No way point Holder !");
        return null;
    }
    public WaypointsHolder GetSharedLine(Point comparedPoint)
    {

            for (int j = 0; j < BelongedWaypointHolders.Count; j++)
            {
                GetSharedLine(BelongedWaypointHolders[j], comparedPoint);
            }

        return null;
    }


    public WaypointsHolder GetSharedLine(WaypointsHolder givenHolder, Point comparedPoint)
    {

        for (int i = 0; i < comparedPoint.BelongedWaypointHolders.Count; i++)
        {
            if (comparedPoint.BelongedWaypointHolders[i] == givenHolder)
            {
                return givenHolder;
            }
        }

        return null;
    }

    public bool WayPointHasObj(WaypointsHolder holder, GameObject point)
    {
        for (int i = 0; i < holder.waypoints.Count; i++)
        {
            if (holder.waypoints[i].belongedObj == point)
            {
                return true;
            }
        }

        return false;
    }

    //get nearest points in one line
    public GameObject GetNearestPointInOneLine(WaypointsHolder holder)
    {
        int pointIndex = 0;
        float currentDistance =
            Vector3.Distance(holder.waypoints[0].belongedObj.transform.position, transform.position); ;

        for (int i = 0; i < holder.waypoints.Count; i++)
        {
            float newDistance = Vector3.Distance(transform.position, holder.waypoints[i].belongedObj.transform.position);
            if (newDistance < currentDistance)
            {
                currentDistance = newDistance;
                pointIndex = i;
            }
        }
        return holder.waypoints[pointIndex].belongedObj;
    }


}
