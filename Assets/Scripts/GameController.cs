using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class GameController : MonoBehaviour
{
    [HideInInspector]

    public GameObject Car;
    private static GameController instance;
    public GameObject HitEffect;
    public GameObject HitEffect2;
    public List<GameObject> CheckPoints;
    //all the endchagnepoints currently get
    public List<Point> AllpointshaveParent = new List<Point>();
    public GameObject Points;
    public List<Transform> AlcoholPoses;
    //current the nearest point from car
    [HideInInspector] public GameObject CurrentPositionObj;
    //the target position
   // [HideInInspector]
    public GameObject TargetPoint;
    public List<GameObject> GeneratedPickUpPosesObj;
    public List<int> GeneratedAlcoholPosesIndexes;

    public List<GameObject> PickUps;
    public List<GameObject> Alcohols;


    [HideInInspector]
    public List<Dictionary<WaypointsHolder, Point>> BestSolution = new List<Dictionary<WaypointsHolder, Point>>();

    [HideInInspector]
    public List<Point> AllchangePointsInFirstLine = new List<Point>();

    private Dictionary<Point, int> changPointWithBestSolutionWayPointNum = new Dictionary<Point, int>();

    private int maxPickupNum = 10;
    private int maxAlcoholNum = 8;
    private int generatePickupInterval;
    //first line needs to do
    [HideInInspector]
    public List<WaypointsHolder> InitialHolders;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameController)) as GameController;
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameController");
                    instance = obj.AddComponent<GameController>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        generatePickupInterval = 0;

        for (int i = 0; i < Points.transform.childCount; i++)
        {
            CheckPoints.Add(Points.transform.GetChild(i).gameObject);
        }

        StartCoroutine(GenerateNewPickup());
   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CastRay();
        }
    }


    void CastRay()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //ignore both layers
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("HighBuilding") | 1 << LayerMask.NameToLayer("IgnoreRayCast"));
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Floor"))
            {
                float currentY = Car.gameObject.transform.position.y;
                Vector3 newPos = new Vector3(hit.point.x, currentY, hit.point.z);
                Vector3 newPos2 = new Vector3(hit.point.x, currentY+5, hit.point.z);

                Transform destinationpos = GetNearPoint(newPos2);
                TargetPoint = GetNearestPointObj(newPos2);
                CurrentPositionObj = GetNearestPointObj(Car.gameObject.transform.position);
                TargetPoint.GetComponent<Point>().StartPoint = CurrentPositionObj;
                Instantiate(HitEffect, destinationpos.position, Quaternion.identity);

                for (int i = 0; i < AllpointshaveParent.Count; i++)
                {
                    if (AllpointshaveParent[i].ParentChangePoint)
                    {
                        AllpointshaveParent[i].ParentChangePoint = null;
                    }
              
                }

                if (TargetPoint.GetComponent<Point>().BelongedWaypointHolders.Count!= 0)
                {
                    BestSolution.Clear();
                    GetSolutionLine(CurrentPositionObj.GetComponent<Point>(), TargetPoint.GetComponent<Point>());
                  //  GetSolutionLine(CurrentPositionObj.GetComponent<Point>(), GameObject.Find("Point (161)").GetComponent<Point>());

              
                      SetInitalHolder();
                      Car.GetComponent<WaypointMover>().movementSpeed = Car.GetComponent<CarController>().InitialMoveSpeed;


                }


               // if (!Car.GetComponent<CarController>().CalculateNewPath(newPos2))
               //  Instantiate(HitEffect2, newPos2, Quaternion.identity);

            }
        }
       }

    private IEnumerator GenerateNewPickup()
    {
       NewRound:
        if (GeneratedPickUpPosesObj.Count == maxPickupNum)
        {
            yield break;
        }

        generatePickupInterval = Random.Range(5, 15);
       // Debug.Log("Generate New Pick UP!");
        yield return new WaitForSeconds(generatePickupInterval);

        Random: 
        int randomindex = Random.Range(0, CheckPoints.Count);
        if (!GeneratedPickUpPosesObj.Contains(CheckPoints[randomindex]))
        {
            int randomPickup = Random.Range(0, PickUps.Count);
           GameObject newPickUP =  Instantiate(PickUps[randomPickup], CheckPoints[randomindex].transform.position, Quaternion.identity);
            newPickUP.GetComponent<Pickup>().BelongedPoint = CheckPoints[randomindex];
            GeneratedPickUpPosesObj.Add(CheckPoints[randomindex]);
            goto NewRound;
        }

        else 
        {
            goto Random;
        }

    }


    private Transform GetNearPoint(Vector3 position)
    {
        int pointIndex = 0;
        float currentDistance =
            Vector3.Distance(position, CheckPoints[0].transform.position); ;

        for (int i = 0; i < CheckPoints.Count; i++)
        {
            float newDistance = Vector3.Distance(position, CheckPoints[i].transform.position);
            if (newDistance < currentDistance)
            {
                currentDistance = newDistance;
                pointIndex = i;
            }
        }
        return CheckPoints[pointIndex].transform;
    }


    private GameObject GetNearestPointObj(Vector3 position)
    {
        int pointIndex = 0;
        float currentDistance =
            Vector3.Distance(position, CheckPoints[0].transform.position); ;

        for (int i = 0; i < CheckPoints.Count; i++)
        {
            float newDistance = Vector3.Distance(position, CheckPoints[i].transform.position);
            if (newDistance < currentDistance)
            {
                currentDistance = newDistance;
                pointIndex = i;
            }
        }
        return CheckPoints[pointIndex];
    }

    //one slution means all the pointset should be in this solution
    public void GetSolutionLine(Point start, Point end)
    {
        if (end.IfTwoPointsInTheSamePath(start) != null)
        {
            //caanot set end.ParentChangePoint = end.gameObject; only last changepoint in solution has this 
            // end.ParentChangePoint = end.gameObject;
            Debug.Log("Reach End!");
            //get best solution
            GetOneSolutionFromEndChangePoint(end,null);
            
        }
        else

        {

            List<GameObject> allNearestChangePoints = new List<GameObject>();
            GameObject nearestChangePoint = new GameObject();

            for (int i = 0; i < end.BelongedWaypointHolders.Count; i++)
            {
              //  Debug.Log(end.BelongedWaypointHolders[i].name + "Path name");
               //get the nearest change point (point that have more than one paths) from start to last path
                GameObject nearestChangePointToALine = start.GetNearestChangePointInOneLine(end.BelongedWaypointHolders[i]);
         
                allNearestChangePoints.Add(nearestChangePointToALine);

                if (end.gameObject != TargetPoint)
                {
                    nearestChangePointToALine.GetComponent<Point>().ParentChangePoint = end.gameObject;
                }
                else nearestChangePointToALine.GetComponent<Point>().ParentChangePoint = nearestChangePointToALine;

            }

            //get the nearest change points in allNearestChangePoints
            int nearIndex = 0;

            float mindDistance = Vector3.Distance(start.gameObject.transform.position, allNearestChangePoints[0].gameObject.transform.position);
            for (int i = 0; i < allNearestChangePoints.Count; i++)
            {                      
                if (mindDistance > Vector3.Distance(start.gameObject.transform.position, allNearestChangePoints[i].gameObject.transform.position))
                {
                   nearIndex = i;
                }        
            }

            nearestChangePoint = allNearestChangePoints[nearIndex].gameObject;

          // Debug.Log("nearestChangePointToALine " + nearestChangePoint.name);
          //  Debug.Log("Get One parent chage point " + nearestChangePoint.GetComponent<Point>().ParentChangePoint.name);

            if (!AllpointshaveParent.Contains(nearestChangePoint.GetComponent<Point>()))
            {
                AllpointshaveParent.Add(nearestChangePoint.GetComponent<Point>());
            }

           GetSolutionLine(start, nearestChangePoint.GetComponent<Point>());

        }
    }


    //if given onepoint exit in one solution
    public bool PointExitInOneSoltuion(List<Dictionary<WaypointsHolder, Point>> onesolution, Point onePoint)
    {
        for (int i = 0; i < onesolution.Count; i++)
        {
            if (onesolution[i].ContainsValue(onePoint))
            {
                return true;
            }
        }

        return false;
    }

    //find out how many waypoint in one solution
    public int GetPointNumberInoneSolution(List<Dictionary<WaypointsHolder, Point>> onesolution)
    {
        int num = 0;
        for (int i = 0; i < onesolution.Count; i++)
        {
            num += onesolution[i].First().Key.waypoints.Count;
        }

        return num;
    }
    //find which change point to go at very begining in the first line same with start point
    public Point GetChangePointWithAllChangesPointsInSameLineWithStartPoint(Point startPoint, WaypointsHolder belongedHolder, List<Point> allchangePointsInFirstLine)
    {
        int minNum = belongedHolder.GetWayPointsNumBetweenTwoPoint(startPoint.GetComponent<Waypoint>(), allchangePointsInFirstLine[0].GetComponent<Waypoint>())
            + changPointWithBestSolutionWayPointNum[allchangePointsInFirstLine[0]];
        Point currentBestPoint = allchangePointsInFirstLine[0];
        for (int i = 0; i < allchangePointsInFirstLine.Count; i++)
        {
            int currentLineMinNum = belongedHolder.GetWayPointsNumBetweenTwoPoint(startPoint.GetComponent<Waypoint>(), allchangePointsInFirstLine[i].GetComponent<Waypoint>());
            currentLineMinNum += changPointWithBestSolutionWayPointNum[allchangePointsInFirstLine[i]];

            if (minNum > currentLineMinNum)
            {
                currentBestPoint = allchangePointsInFirstLine[i];
            }

        }

        return currentBestPoint;
    }


    public void SetInitalHolder()
    {
        Debug.Log(BestSolution.Count);
        Car.GetComponent<WaypointMover>().waypointsHolder =  BestSolution.First().First().Key;
        //go from nearest point
        Car.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder();
        Debug.Log("Set Initial Path " + Car.GetComponent<WaypointMover>().waypointsHolder.name);
    }


    public void GetOneSolutionFromEndChangePoint(Point endPoint, List<Dictionary<WaypointsHolder, Point>> oneSolution )
    {
        if (oneSolution == null)
        {
            oneSolution = new List<Dictionary<WaypointsHolder, Point>>();
        }


            //to the targetpoint
            if (endPoint.ParentChangePoint == endPoint.gameObject)
            {
              Dictionary<WaypointsHolder, Point> oneChangePoint = new Dictionary<WaypointsHolder, Point>();
              oneChangePoint.Add(endPoint.GetBestHolderToParentchangePoint(), endPoint);
              oneSolution.Add(oneChangePoint);
             //get bestSolution
               BestSolution = oneSolution;
               Debug.Log("Get Best Solution!");
            }
            else
            {
                Dictionary<WaypointsHolder, Point> oneChangePoint = new Dictionary<WaypointsHolder, Point>();
                oneChangePoint.Add(endPoint.GetBestHolderToParentchangePoint(), endPoint);
                oneSolution.Add(oneChangePoint);
                Debug.Log("Add change point to best solution" + oneChangePoint.First().Value.gameObject.name);
                GetOneSolutionFromEndChangePoint(endPoint.ParentChangePoint.GetComponent<Point>(), oneSolution);
            }     
    }
}




