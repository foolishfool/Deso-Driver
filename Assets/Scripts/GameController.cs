using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;
using Pathfinding.Examples;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public GameObject Car;
    //public AstarSmoothFollow2 Camera;
    private static GameController instance;

    public TwirlEffectManipulationExample TwirEffect;

    public GameObject PickupParent;
    public GameObject AlcoholsParent;
    [HideInInspector]
    public List<GameObject> PickUpPoints;

    public TargetMover Target;

    public AIPath AIPath;
    [HideInInspector]
    public bool Drunk;
    [HideInInspector]
    public List<GameObject> AlcoholsPoints;
  
    public float BacNum;
    [HideInInspector]
    public int MoneyNum;

    [HideInInspector]
    public bool TimeRunning;

    public List<GameObject> ResetPositions = new List<GameObject>();

    public List<GameObject> GeneratedPickUpPosesObj;
    public List<GameObject> GeneratedAlcoholPosesObj;
    public List<int> GeneratedAlcoholPosesIndexes;
    //tareet and start in the same line
    public List<GameObject> PickUps;
    public List<GameObject> Alcohols;

    public ParticleSystem Pickupeffect;

    [HideInInspector]
    public GameObject CurrentPickUp;

    private int maxPickupNum = 8;
    private int maxAlcoholNum = 50;

    private float generatePickupInterval;
    private float generateAlcoholInterval;

    private float drinksigntimer;
    public bool FirstPickup;

    private bool canGenerateAlcohol;
    private bool canGeneratePickup;
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

        Target.enabled = false;

        generatePickupInterval = 0;

        for (int i = 0; i < PickupParent.transform.childCount; i++)
        {
            PickUpPoints.Add(PickupParent.transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < AlcoholsParent.transform.childCount; i++)
        {
            AlcoholsPoints.Add(AlcoholsParent.transform.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        canGenerateAlcohol = GeneratedAlcoholPosesObj.Count >= maxAlcoholNum ? false : true;
        canGeneratePickup = GeneratedPickUpPosesObj.Count >= maxPickupNum ? false : true;

        ResetPositions = Target.ResetPositions;


    }



    public IEnumerator GenerateNewPickup()
    {
       NewRound:
        yield return new WaitUntil(() => canGeneratePickup == true);
        generatePickupInterval = Random.Range(1, 5);
       // Debug.Log("Generate New Pick UP!");
        yield return new WaitForSeconds(generatePickupInterval);

        Random: 
        int randomindex = Random.Range(0, PickUpPoints.Count);
        if (!GeneratedPickUpPosesObj.Contains(PickUpPoints[randomindex]))
        {
            int randomPickup = Random.Range(0, PickUps.Count);
            GameObject newPickUP =  Instantiate(PickUps[randomPickup], PickUpPoints[randomindex].transform.position, Quaternion.identity);
            newPickUP.transform.parent = PickUpPoints[randomindex].transform;
            GeneratedPickUpPosesObj.Add(PickUpPoints[randomindex]);
            goto NewRound;
        }

        else 
        {
            goto Random;
        }

    }




    public IEnumerator GenerateNewAlcohol()
    {
     
        NewRound:
        yield return new WaitUntil(() => canGenerateAlcohol == true);

        generateAlcoholInterval = Random.Range(1, 5);
        // Debug.Log("Generate New Pick UP!");
        yield return new WaitForSeconds(generateAlcoholInterval);

        Random:
        int randomindex = Random.Range(0, AlcoholsPoints.Count);
        if (!GeneratedAlcoholPosesObj.Contains(AlcoholsPoints[randomindex]))
        {
            int randomPickup = Random.Range(0, Alcohols.Count);
            if (Vector3.Distance(AlcoholsPoints[randomindex].transform.position,Car.transform.position)<15f)
            {
                goto Random;
            }
            GameObject newAlcohol = Instantiate(Alcohols[randomPickup], AlcoholsPoints[randomindex].transform.position, Quaternion.identity);
            GeneratedAlcoholPosesObj.Add(AlcoholsPoints[randomindex]);
            AlcoholsPoints[randomindex].transform.GetChild(1).gameObject.SetActive(true);
            newAlcohol.transform.parent = AlcoholsPoints[randomindex].transform;

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
            TwirEffect.Amount = 0.2f;
            TwirEffect.Speed = 1.3f;
            AIPath.maxSpeed = 20;
            Drunk = true;
            Car.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            TwirEffect.Amount += 0.1f;
            TwirEffect.Speed += 0.1f;
            AIPath.maxSpeed += 5;

        }
      
       
    }



    public void GameRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1;
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
    }


    public void GameFinish()
    {
        StartCoroutine(ResultPanelShowBehavoir());

    }

    IEnumerator ResultPanelShowBehavoir()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ResultPanelShow();
        yield break;
    }

    public void GamePause()
    {
        UIManager.Instance.PausePanelShow();
    }
    public void GameResume()
    {
        UIManager.Instance.PausePanelHide();
    }
    //call in ui button
    public void ResetTargetUIClick()
    {
        Target.UIClicked = false;
    }
}




