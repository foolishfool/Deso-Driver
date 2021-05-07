using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;

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
    public GameObject CurrentPositionObj;
    //the target position

    public GameObject TargetPoint;
    public List<GameObject> GeneratedPickUpPosesObj;
    public List<int> GeneratedAlcoholPosesIndexes;
    //tareet and start in the same line
    [HideInInspector]
    public bool FirstChangePointIsSameLinewithTarget;
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

    List<GameObject> allNearestChangePoints = new List<GameObject>();

    int count = 0;
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
                count = 0;
                float currentY = Car.gameObject.transform.position.y;
                Vector3 newPos = new Vector3(hit.point.x, currentY, hit.point.z);
                Vector3 newPos2 = new Vector3(hit.point.x, currentY+5, hit.point.z);

                Transform effectPosition = GetNearestPoint(newPos2);
                TargetPoint = GetNearestPointObj(newPos);
                Debug.Log(TargetPoint.name + "  New Tareget " + Time.frameCount);
                CurrentPositionObj = GetNearestPointObj(Car.gameObject.transform.position);
                TargetPoint.GetComponent<Point>().StartPoint = CurrentPositionObj;
                Instantiate(HitEffect, effectPosition.position, Quaternion.identity);

                //reset
                for (int i = 0; i < AllpointshaveParent.Count; i++)
                {
                    if (AllpointshaveParent[i].ParentChangePoint)
                    {
                        AllpointshaveParent[i].ParentChangePoint = null;
             
                    }
              
                }
                allNearestChangePoints.Clear();

                if (CurrentPositionObj == TargetPoint)
                {
                    return;
                }
                //cannot go back for uncircle path
                if (Car.GetComponent<WaypointMover>().waypointsHolder)
                {
                    if (Car.GetComponent<WaypointMover>().waypointsHolder.isNotCirclePath)
                    {
                        if(Car.GetComponent<WaypointMover>().waypointsHolder.GetComponent<PathGenerator>().Points.Contains(TargetPoint))
                        if (Car.GetComponent<WaypointMover>().waypointsHolder.GetComponent<PathGenerator>().Points.IndexOf(TargetPoint) < Car.GetComponent<WaypointMover>().waypointsHolder.GetComponent<PathGenerator>().Points.IndexOf(CurrentPositionObj))
                        {
                            //cannot go back and 
                            Instantiate(HitEffect2, effectPosition.position, Quaternion.identity);
                            return;
                        }
                    }

                }
      

                if (TargetPoint.GetComponent<Point>().BelongedWaypointHolders.Count!= 0)
                {
                    //reset
                    BestSolution.Clear();
                    FirstChangePointIsSameLinewithTarget = false;
                    Car.GetComponent<CarController>().Sequence.Kill();

                    GetSolutionLine(CurrentPositionObj.GetComponent<Point>(), TargetPoint.GetComponent<Point>());
            
                     SetInitalHolder();
                     Car.GetComponent<WaypointMover>().movementSpeed = Car.GetComponent<CarController>().InitialMoveSpeed;

                }

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


    private Transform GetNearestPoint(Vector3 position)
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
      
        Debug.Log("GetSolutonLine () from start " + start.gameObject.name + " to end " + end.gameObject.name);
        if (end.IfTwoPointsInTheSamePath(start) != null) 
        {
            //caanot set end.ParentChangePoint = end.gameObject; only last changepoint in solution has this 
            // end.ParentChangePoint = end.gameObject;
            Debug.Log("Reach End!  " + Time.frameCount);
            //get best solution

                //means target and start in the same line
             GetOneSolutionFromEndChangePoint(end, null);


        }
        else

        {

            for (int i = 0; i < end.BelongedWaypointHolders.Count; i++)
            {
              //  Debug.Log(end.BelongedWaypointHolders[i].name + "Path name");
               //get the nearest change point (point that have more than one paths) from start to last path
              GameObject   nearestChangePointToALine = start.GetNearestChangePointInOneLine(end.BelongedWaypointHolders[i]);

                if (!allNearestChangePoints.Contains(nearestChangePointToALine))
                    allNearestChangePoints.Add(nearestChangePointToALine);

                //if nearestChangePointToALine is not in the same line with target
                //apply parentchangepoint for each change point , which will be used in future best solution get


                if (!nearestChangePointToALine.GetComponent<Point>().IfTwoPointsInTheSamePath(TargetPoint.GetComponent<Point>()))
                {
                    nearestChangePointToALine.GetComponent<Point>().ParentChangePoint = end.gameObject;

                    Debug.Log(nearestChangePointToALine.name + " Is Applied Parent Point : " + end.gameObject.name + Time.frameCount);
                }
                else
                {

                    //ParentChangePoint is itself
                    nearestChangePointToALine.GetComponent<Point>().ParentChangePoint = nearestChangePointToALine;
                    Debug.Log(nearestChangePointToALine.name + " Is Appled Parent Point : " + nearestChangePointToALine.name + Time.frameCount);
                    if (nearestChangePointToALine != TargetPoint)
                    {
                        nearestChangePointToALine.GetComponent<Point>().IsSameLineWithTargetButNotTarget = true;
                    }
                }

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

            GameObject nearestChangePoint = allNearestChangePoints[nearIndex].gameObject;

          // Debug.Log("nearestChangePointToALine " + nearestChangePoint.name);
          //  Debug.Log("Get One parent chage point " + nearestChangePoint.GetComponent<Point>().ParentChangePoint.name);

            if (!AllpointshaveParent.Contains(nearestChangePoint.GetComponent<Point>()))
            {
                AllpointshaveParent.Add(nearestChangePoint.GetComponent<Point>());
            }
            if (end.gameObject == nearestChangePoint)
            {
                Instantiate(HitEffect2, end.gameObject.transform.position, Quaternion.identity);
                return;
                Debug.Log("No path to get to this point !");
            }
            count++;
            if (count>40)
            {
                Debug.Log(nearestChangePoint.GetComponent<Point>().name + " Out of Loop");
                return;
            }
            Debug.Log(nearestChangePoint.GetComponent<Point>().name + " One Loop");
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
        if (BestSolution.Count == 0)
        {
            //in this situation target is in the same line with start and bestsolution is no apply and just apply the common holder to car
            Debug.Log("Target is in the same line with start " + Time.frameCount);
        
            if (Car.GetComponent<WaypointMover>().waypointsHolder)
            {
                return;
            }
            //set holder to be the line have target
            Car.GetComponent<WaypointMover>().waypointsHolder = CurrentPositionObj.GetComponent<Point>().IfTwoPointsInTheSamePath(TargetPoint.GetComponent<Point>());
            //go from nearest point CurrentPositionObj is the nearest point
            Car.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder(CurrentPositionObj);
            return;
        }

        if (FirstChangePointIsSameLinewithTarget)
        {
            Debug.Log(BestSolution.First().First().Key.name + "2222222");
            //set current hodler to be the one that contains both currentpositionobj and first chagnepoint
            Car.GetComponent<WaypointMover>().waypointsHolder = BestSolution.First().First().Value.GetBestHolderContainsTargetandFirstchangePoint();
        }
        else
        {
            Car.GetComponent<WaypointMover>().waypointsHolder = BestSolution.First().First().Key;

            Debug.Log(BestSolution.First().First().Key.name  + "111111111111");
        }
       

    
        Debug.Log("Set Initial Path " + Car.GetComponent<WaypointMover>().waypointsHolder.name);
        //go from nearest point
        Car.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder(null);

    }


    public void GetOneSolutionFromEndChangePoint(Point endPoint, List<Dictionary<WaypointsHolder, Point>> oneSolution)
    {
        if (oneSolution == null)
        {
            oneSolution = new List<Dictionary<WaypointsHolder, Point>>();

        }

        //for the situation endpoint is target and come here by  execute      GetOneSolutionFromEndChangePoint(end, null);
        if (endPoint.gameObject == TargetPoint)
        {
            //set current hodler to be the one that contains both currentpositionobj and first chagnepoint
            //continue move on current 
            if (Car.GetComponent<WaypointMover>().waypointsHolder)
            {           //do nothing continue move on current line
                Debug.Log("Do Nothing and keep on current line" + Time.frameCount);
                return;
            }
            Car.GetComponent<WaypointMover>().waypointsHolder = endPoint.GetBestHolderContainsTargetandFirstchangePoint();
            Debug.Log("Change holder !  to   " + Car.GetComponent<WaypointMover>().waypointsHolder.name + Time.frameCount);
            Car.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder(null);
            return;
        }
            //to the targetpoint
           if (endPoint.ParentChangePoint == endPoint.gameObject)
            {

              Dictionary<WaypointsHolder, Point> oneChangePoint = new Dictionary<WaypointsHolder, Point>();
              oneChangePoint.Add(endPoint.GetBestHolderToParentchangePoint(), endPoint);
              Debug.Log("Add holder:" +  endPoint.GetBestHolderToParentchangePoint().name + " changepoint: " + endPoint.gameObject.name + "To solution ");

            //get bestSolution 

                oneSolution.Add(oneChangePoint);
                BestSolution = oneSolution;

            if (BestSolution.First().First().Value.IsSameLineWithTargetButNotTarget)
            {
                FirstChangePointIsSameLinewithTarget = true;
            }
            //reset
            endPoint.IsSameLineWithTargetButNotTarget = false;


            Debug.Log("Get Best Solution!");
            }
            else
            {
                Dictionary<WaypointsHolder, Point> oneChangePoint = new Dictionary<WaypointsHolder, Point>();
                oneChangePoint.Add(endPoint.GetBestHolderToParentchangePoint(), endPoint);
            //***as long as oneSolution == null if will be always endPoint.ParentChangePoint == endPoint.gameObject
            //so in this case we can just use tempsolution
                oneSolution.Add(oneChangePoint);
          
                Debug.Log("Add change point to best solution" + oneChangePoint.First().Value.gameObject.name);

                GetOneSolutionFromEndChangePoint(endPoint.ParentChangePoint.GetComponent<Point>(), oneSolution);
            }


    }
}




