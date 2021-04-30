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
    public GameObject Points;
    public List<Transform> AlcoholPoses;
    //current the nearest point from car
    [HideInInspector] public GameObject CurrentPositionObj;
    //the target position
    [HideInInspector]
    public GameObject TargetPoint;
    public List<GameObject> GeneratedPickUpPosesObj;
    public List<int> GeneratedAlcoholPosesIndexes;

    public List<GameObject> PickUps;
    public List<GameObject> Alcohols;

    //each element in AllRoutesCanArrive is one solution
    //List<Dictionary<WaypointsHolder, Point>> is the sequence of change point and line 
    //car move sequence should from end to from of List<Dictionary<WaypointsHolder, Point>>
    [HideInInspector]
    public List<List<Dictionary<WaypointsHolder, Point>>> AllRoutesCanArrive = new List<List<Dictionary<WaypointsHolder, Point>>>();

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

                //  if (Car.GetComponent<CarController>().CalculateNewPath(destinationpos.position))
                //  {
                //      Car.GetComponent<NavMeshAgent>().isStopped = false;
                //      //  Car.GetComponent<NavMeshAgent>().velocity *= 0.1f ;
                //        //  Car.GetComponent<NavMeshAgent>().destination = destinationpos.position;
                //     
                //      Instantiate(HitEffect, destinationpos.position, Quaternion.identity);
                //   //   Car.transform.DORotateQuaternion(destinationpos.rotation,2f);
                //      //   Debug.Log(newPos.x + "   " + newPos.y + "   " + newPos.z);
                //  }
                if (TargetPoint.GetComponent<Point>().BelongedWaypointHolders.Count!= 0)
                {
                    GetCommonLines(CurrentPositionObj.GetComponent<Point>(), TargetPoint.GetComponent<Point>());
                    //handle AllRoutesCanArrive 
                    BestSolution = GetOneSolutionFromAll(TargetPoint.GetComponent<Point>());
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

    public void GetCommonLines(Point start, Point end)
    {
        if (end.GetSameWaypointHolder(start) != null)
        {
            List<Dictionary<WaypointsHolder, Point>> oneSolution = new List<Dictionary<WaypointsHolder, Point>>();
            Dictionary<WaypointsHolder, Point> oneChangepoint = new Dictionary<WaypointsHolder, Point>();
            oneChangepoint.Add(end.GetSameWaypointHolder(start),null);
            oneSolution.Add(oneChangepoint);
            //only add the last point ,which is the point in the same line with start point
            AllchangePointsInFirstLine.Add(oneChangepoint.First().Value);
           // changPointWithBestSolutionWayPointNum.Add(oneChangepoint.First().Value, 0);
            AllRoutesCanArrive.Add(oneSolution);
            InitialHolders.Add(end.GetSameWaypointHolder(start));
        }
        else

        {
            for (int i = 0; i < end.BelongedWaypointHolders.Count; i++)
            {
                List<Dictionary<WaypointsHolder, Point>> oneSolution = new List<Dictionary<WaypointsHolder, Point>>();
              
                GameObject nearestPointToALine = start.GetNearestPointInOneLine(end.BelongedWaypointHolders[i]);
                Dictionary<WaypointsHolder, Point> oneChangepoint = new Dictionary<WaypointsHolder, Point>();
              
                oneChangepoint.Add(end.BelongedWaypointHolders[i], nearestPointToALine.GetComponent<Point>());
                oneSolution.Add(oneChangepoint);
                AllRoutesCanArrive.Add(oneSolution);
                GetCommonLines(start, nearestPointToALine.GetComponent<Point>());
            }
            

     
        }
    }


    public List<Dictionary<WaypointsHolder, Point>> GetOneSolutionFromAll(Point onePoint)
    {
        int currentMinNum = GetPointNumberInoneSolution(AllRoutesCanArrive[0]);
        List<Dictionary<WaypointsHolder, Point>> currentSolution = AllRoutesCanArrive[0];
        for (int i = 0; i < AllRoutesCanArrive.Count; i++)
        {
            if (PointExitInOneSoltuion(AllRoutesCanArrive[i],onePoint))
            {
                if (GetPointNumberInoneSolution(AllRoutesCanArrive[i]) < currentMinNum)
                {
                    currentMinNum = GetPointNumberInoneSolution(AllRoutesCanArrive[i]);
                    currentSolution = AllRoutesCanArrive[i];
                }
            }

        }

        return currentSolution;
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
       Car.GetComponent<WaypointMover>().waypointsHolder =  BestSolution.Last().First().Key;
    }
}




