using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;
using Pathfinding.Examples;

public class GameController : MonoBehaviour
{

    public GameObject Car;
    public AstarSmoothFollow2 Camera;
    private static GameController instance;

    public TwirlEffectManipulationExample TwirEffect;

    public GameObject PickupParent;
    public GameObject AlcoholsParent;
    [HideInInspector]
    public List<GameObject> PickUpPoints;


    [HideInInspector]
    public bool Drunk;
    [HideInInspector]
    public List<GameObject> AlcoholsPoints;
    public List<Transform> AlcoholPoses;

    public List<GameObject> GeneratedPickUpPosesObj;
    public List<GameObject> GeneratedAlcoholPosesObj;
    public List<int> GeneratedAlcoholPosesIndexes;
    //tareet and start in the same line
    public List<GameObject> PickUps;
    public List<GameObject> Alcohols;

    public bool IsCameraLevelingUp;

    private int maxPickupNum = 10;
    private int maxAlcoholNum = 20;

    private int generatePickupInterval;
    private int generateAlcoholInterval;

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

        for (int i = 0; i < PickupParent.transform.childCount; i++)
        {
            PickUpPoints.Add(PickupParent.transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < AlcoholsParent.transform.childCount; i++)
        {
            AlcoholsPoints.Add(AlcoholsParent.transform.GetChild(i).gameObject);
        }
        StartCoroutine(GenerateNewPickup());
        StartCoroutine(GenerateNewAlcohol());

    }

    // Update is called once per frame
    void Update()
    {
      // if (IsCameraLevelingUp)
      // {
      //     Camera.height++;
      // }
      // else 
      // {
      //     if (Camera.height>=15)
      //     {
      //         Camera.height--;
      //     }
      // }
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
        int randomindex = Random.Range(0, PickUpPoints.Count);
        if (!GeneratedPickUpPosesObj.Contains(PickUpPoints[randomindex]))
        {
            int randomPickup = Random.Range(0, PickUps.Count);
           GameObject newPickUP =  Instantiate(PickUps[randomPickup], PickUpPoints[randomindex].transform.position, Quaternion.identity);
            GeneratedPickUpPosesObj.Add(PickUpPoints[randomindex]);
            goto NewRound;
        }

        else 
        {
            goto Random;
        }

    }




    private IEnumerator GenerateNewAlcohol()
    {
        NewRound:
        if (GeneratedAlcoholPosesObj.Count == maxAlcoholNum)
        {
            yield break;
        }

        generateAlcoholInterval = Random.Range(5, 10);
        // Debug.Log("Generate New Pick UP!");
        yield return new WaitForSeconds(generateAlcoholInterval);

        Random:
        int randomindex = Random.Range(0, AlcoholsPoints.Count);
        if (!GeneratedAlcoholPosesObj.Contains(AlcoholsPoints[randomindex]))
        {
            int randomPickup = Random.Range(0, Alcohols.Count);
            GameObject newPickUP = Instantiate(Alcohols[randomPickup], AlcoholsPoints[randomindex].transform.position, Quaternion.identity);
            GeneratedAlcoholPosesObj.Add(AlcoholsPoints[randomindex]);
            goto NewRound;
        }

        else
        {
            goto Random;
        }

    }



    public void TwirEffectIncrease()
    {
        if (!Drunk)
        {
            TwirEffect.Amount = 0.3f;
            TwirEffect.Speed = 1.5f;
            Drunk = true;
        }
        else
        {
            TwirEffect.Amount += 0.3f;
            TwirEffect.Speed += 0.2f;
        }
      
       
    }



}




