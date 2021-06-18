using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;

public class PoliceCar : MonoBehaviour
{

    public List<Light> Spotlights;
    public TargetMover Target;
    // Start is called before the first frame update
    void Start()
    {
        LightShining();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LightShining()
    {
        for (int i = 0; i < Spotlights.Count; i++)
        {
            Spotlights[i].DOIntensity(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car") && !GameController.Instance.PoliceChecked && GameController.Instance.TimeRunning)
        {
            Target.AIPath.maxSpeed = 0;
            AudioController.Instance.PoliceCarAudioSource.DOFade(0.3f, 0.5f);
            GameObject sign = Instantiate(UIManager.Instance.PoliceSign, gameObject.transform.position, Quaternion.identity);
            sign.transform.parent = GameObject.Find("UI").transform;
            sign.GetComponent<PoliceCarSign>().BelongedPoliceCar = gameObject;
            sign.transform.DOScale(Vector3.one, 1f).OnComplete(() => StartCoroutine(sign.GetComponent<PoliceCarSign>().ShowInfo(GameController.Instance.BacNum>0.0f))) ;
            GameController.Instance.PoliceChecked = true;
        }


      if (other.gameObject.CompareTag("Collision"))
        {
            for (int i = 0; i < GameController.Instance.Paths.Count; i++)
            {
                if (GameController.Instance.Paths[i].Target.gameObject == other.gameObject)
                {
                    GameController.Instance.Paths[i].Pause();
                }
            }

            Vector3 direction = (other.gameObject.transform.position - gameObject.transform.position).normalized;
          other.gameObject.GetComponent<Rigidbody>().AddForce(direction*3, ForceMode.Impulse);
      }

    }


    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Car"))
        {
            Target.AIPath.maxSpeed = 15;

            AudioController.Instance.PoliceCarAudioSource.DOFade(0, 4f);
        }
    }

}
