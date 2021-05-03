using System.Collections.Generic;
using UnityEngine;


public class Point : MonoBehaviour
{

    public Vector3 Direction;
    public GameObject StartPoint;
    //through which parent to get to current changepoint 
    //through    public void GetCommonLines(Point start, Point end, List<Dictionary<WaypointsHolder, Point>> oneSolution) to get current point then end is current point's ParentChangePoint
    //as nearestChangePointToALine is only one so ParentChangePoint is only one
    public GameObject ParentChangePoint;
    public List<WaypointsHolder> BelongedWaypointHolders;
    //the waypointhodlers which the lead to  parent change point
    public List<WaypointsHolder> ParentHolders = new List<WaypointsHolder>();

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


    public WaypointsHolder IfTwoPointsInTheSamePath(Point StartPoint)
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
        Debug.Log(StartPoint.gameObject.name + "  No way point Holder ! to " + gameObject.name);
        return null;
    }


    public void GetParentHolders()
    {
        if (ParentChangePoint == null)
        {
            //***if ParentChangePoint is null, neans ParentChangePoint is not set value through GetSolutionLine and means do not need to transfer to other path just folow currwent path is OK
            ParentChangePoint = gameObject;
        }
        for (int j = 0; j < BelongedWaypointHolders.Count; j++)
        {
            GetSharedLineWithparentPoint(BelongedWaypointHolders[j]);
        }

    }


    public void GetSharedLineWithparentPoint(WaypointsHolder givenHolder)
    {
        //if it is the last change point then only the holder that has with target is parentholder
        if (ParentChangePoint == gameObject)
        {
            for (int i = 0; i < ParentChangePoint.GetComponent<Point>().BelongedWaypointHolders.Count; i++)
            {
                //only add the paths that combine target 
                if (WayPointHasObj(BelongedWaypointHolders[i], GameController.Instance.TargetPoint))
                {
                    ParentHolders.Add(BelongedWaypointHolders[i]);
                }

            }
        }
        else
        {
            for (int i = 0; i < ParentChangePoint.GetComponent<Point>().BelongedWaypointHolders.Count; i++)
            {
                if (ParentChangePoint.GetComponent<Point>().BelongedWaypointHolders[i] == givenHolder)
                {
                    ParentHolders.Add(givenHolder);
                }
            }
        }


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
    public GameObject GetNearestChangePointInOneLine(WaypointsHolder holder)
    {
        int pointIndex = 0;
        float currentDistance = 0;
        for (int i = 0; i < holder.waypoints.Count; i++)
        {
            if (holder.waypoints[i].belongedObj.GetComponent<Point>().BelongedWaypointHolders.Count > 1)
            {
                pointIndex = i;
                currentDistance = Vector3.Distance(holder.waypoints[i].belongedObj.transform.position, transform.position);
                goto EndLoopForInitializeCurrentDistance;
            }
        }

        EndLoopForInitializeCurrentDistance:

        for (int i = 0; i < holder.waypoints.Count; i++)
        {
            if (holder.waypoints[i].belongedObj.GetComponent<Point>().BelongedWaypointHolders.Count > 1)
            {
                float newDistance = Vector3.Distance(transform.position, holder.waypoints[i].belongedObj.transform.position);
                if (newDistance < currentDistance)
                {
                    currentDistance = newDistance;
                    pointIndex = i;
                }
            }

        }
        return holder.waypoints[pointIndex].belongedObj;
    }


    public WaypointsHolder GetBestHolderToParentchangePoint()
    {

        GetParentHolders();
        int minWayPointNum = GetWaypointNumToParent(ParentHolders[0]);
        int minNumPathIndex = 0;
        for (int i = 0; i < ParentHolders.Count; i++)
        {
            if (minWayPointNum > GetWaypointNumToParent(ParentHolders[i]))
            {
                minNumPathIndex = i;
            }
        }

        return ParentHolders[minNumPathIndex];
    }

    public int GetWaypointNumToParent(WaypointsHolder holder)
    {
        return holder.GetWayPointsNumBetweenTwoPoint(gameObject.GetComponent<Waypoint>(), ParentChangePoint.GetComponent<Waypoint>());
    }

}
