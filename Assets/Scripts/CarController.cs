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

    public float InitialMoveSpeed = 10;
    [HideInInspector]
    public Sequence Sequence ;

    // Start is called before the first frame update
    void Start()
    {

        SelfNavAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
 

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Point"))
        {
            Debug.Log(other.gameObject.name + "  Enconter !" );
            if (other.gameObject == GameController.Instance.TargetPoint)
            {
            
                Debug.Log(GameController.Instance.TargetPoint.name);
                GetComponent<WaypointMover>().movementSpeed = 0;
                Sequence = DOTween.Sequence();
                Sequence.Append (transform.DOMove(other.gameObject.transform.position,1f));
                Sequence.Play();
            }
            else
            {
 
                Point changePoint = new Point();
                if (gameObject.GetComponent<WaypointMover>().waypointsHolder)
                {

                    if (GameController.Instance.BestSolution.Count == 0)
                    {
                        //in this situation, there is no bestsolution, as tartet and start in the same line just need to move follow current line is OK
                        return;
                    }

                    if (other.gameObject == GameController.Instance.BestSolution.First().First().Value.gameObject)
                    {
                                      
                        gameObject.GetComponent<WaypointMover>().waypointsHolder = GameController.Instance.BestSolution.First().First().Key;
                        gameObject.GetComponent<WaypointMover>().ResetCurrentPositionWhenChangeHolder(other.gameObject);
          
                        //remove one changepointset after pass
                        GameController.Instance.BestSolution.RemoveAt(0);


                    }
  
                }

            }


        }
 
     
    }


    private void MoveAndRotateTowards(Vector3 position,Vector3 direction)
    {

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
     //   transform.localPosition = Vector3.Slerp(transform.localPosition, position, Time.deltaTime * 2f);
    }

}
