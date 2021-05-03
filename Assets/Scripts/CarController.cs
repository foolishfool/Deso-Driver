using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Linq;

public class CarController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent SelfNavAgent;
    private Vector3 direction;
    private Vector3 position;
    public Vector3 CurrentDestination;
    public Vector3 PreviousDestination;
    public float InitialMoveSpeed = 10;
    private NavMeshPath navMeshPath ;
   // Start is called before the first frame update
   void Start()
    {
        navMeshPath = new NavMeshPath();
        SelfNavAgent = GetComponent<NavMeshAgent>();
        SelfNavAgent.updateRotation = false;

    }

    // Update is called once per frame
    void Update()
    {
        return;
        if (!SelfNavAgent.pathPending)
        {
            if (SelfNavAgent.remainingDistance <= SelfNavAgent.stoppingDistance)
            {
                SelfNavAgent.isStopped = true;
            }
            MoveAndRotateTowards(position,direction);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Point"))
        {
            Debug.Log(other.gameObject);
            if (other.gameObject == GameController.Instance.TargetPoint)
            {
            
                Debug.Log(GameController.Instance.TargetPoint.name);
                GetComponent<WaypointMover>().movementSpeed = 0;
            }
            else
            {
 
                Point changePoint = new Point();
                if (gameObject.GetComponent<WaypointMover>().waypointsHolder)
                {
                   // Debug.Log(gameObject.GetComponent<WaypointMover>().waypointsHolder.name+ System.DateTime.Now.ToString());
                  //  GameController.Instance.BestSolution.First().TryGetValue(gameObject.GetComponent<WaypointMover>().waypointsHolder, out changePoint);
                    if (other.gameObject == GameController.Instance.BestSolution.First().First().Value.gameObject)
                    {
                                      
                       // Debug.Log(GameController.Instance.BestSolution.First().First().Key.name + System.DateTime.Now.ToString());
                        gameObject.GetComponent<WaypointMover>().waypointsHolder = GameController.Instance.BestSolution.First().First().Key;
                        gameObject.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder();
                       // Debug.Log("Change to new path " + gameObject.GetComponent<WaypointMover>().waypointsHolder.name + "11111111111111111111" + System.DateTime.Now.ToString());
                        //remove one changepointset after pass
                        GameController.Instance.BestSolution.RemoveAt(0);
                        //change to next line


                    }
  
                }

            }


        }
            return;
        // Debug.Log(other.gameObject.name + "ddddd");
        if (other.gameObject.CompareTag("Point"))
        {

            // SelfNavAgent.destination = other.gameObject.transform.position;
            direction = other.gameObject.GetComponent<Point>().Direction;
            position = other.gameObject.GetComponent<Point>().transform.position;
            //  Vector3 newDirecton = new Vector3(0, other.gameObject.GetComponent<Point>().Direction.y, 0);
            //   gameObject.transform.localRotation = Quaternion.LookRotation(other.gameObject.GetComponent<Point>().Direction);
        }
        else position = SelfNavAgent.destination;

        if (other.CompareTag("PickUp"))
        {
            //ui update 
            other.gameObject.GetComponent<Pickup>().Disappear();
        }

        if (other.CompareTag("Alcohol"))
        {
            //ui update 
           // other.gameObject.GetComponent<>().Disappear();
        }
    }

    public bool CalculateNewPath(Vector3 Destination)
    {

        SelfNavAgent.CalculatePath(Destination, navMeshPath);

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private void MoveAndRotateTowards(Vector3 position,Vector3 direction)
    {

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
     //   transform.localPosition = Vector3.Slerp(transform.localPosition, position, Time.deltaTime * 2f);
    }

}
