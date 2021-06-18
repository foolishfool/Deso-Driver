using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class Alcohol : MonoBehaviour
{
    public float BacNum;
    public GameObject Explosion;
    public GameObject RingEffect;
    private bool isDrinked;
    [HideInInspector]
    public bool HasSign;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.World);

        float distance = Vector3.Distance(gameObject.transform.position, GameController.Instance.Car.transform.position);
        if (distance <= 15 && !HasSign)
        {
            GameObject sign = Instantiate(UIManager.Instance.AlcoholSign, gameObject.transform.position, Quaternion.identity);
            sign.transform.parent = GameObject.Find("UI").transform;
            sign.GetComponent<AlcoholSign>().BelongedAlcohol = gameObject;
            sign.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 1f);
            HasSign = true;
            AudioController.Instance.PlayEventSFX(AudioController.Instance.WarningSFX);
        }
    }
    public IEnumerator LifeBehavior()
    {
        gameObject.transform.DOMoveY(2.7f, 1f).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(20f);
        if (GameController.Instance.GeneratedAlcoholPosesObj.Contains(gameObject.transform.parent.gameObject))
        {
            gameObject.transform.parent.GetChild(1).gameObject.SetActive(false);
            GameController.Instance.GeneratedAlcoholPosesObj.Remove(gameObject.transform.parent.gameObject);
        }
        Disappear();
        yield break;
    }

    public void Disappear()
    {
        gameObject.transform.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(gameObject));
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Car"))
        {
            return;
        }
        if (isDrinked)
        {
            return;
        }
        GameController.Instance.BacNum += BacNum;
        isDrinked = true;

        GameController.Instance.BacNum = (float)Math.Round(GameController.Instance.BacNum * 100f) / 100f;

        UIManager.Instance.UpdateBacText();

        AudioController.Instance.PlayEventSFX(AudioController.Instance.AlcoholSFX);

        if (GameController.Instance.BacNum >0.05)
        {
            GameController.Instance.TwirEffectIncrease();
        }
        else
        {
          GameController.Instance.AIPath.maxSpeed += 2;
        }
        if (GameController.Instance.BacNum >= 0.15f)
        {
            if (!GameController.Instance.PoliceCar.activeSelf)
            {
                GameController.Instance.ShowPoliceCar();
            }

        }
        if (GameController.Instance.GeneratedAlcoholPosesObj.Contains(gameObject.transform.parent.gameObject))
        {
            gameObject.transform.parent.GetChild(1).gameObject.SetActive(false);
            GameController.Instance.GeneratedAlcoholPosesObj.Remove(gameObject.transform.parent.gameObject);
        }
        RingEffect.SetActive(false);
        Explosion.SetActive(true);
        Disappear();
    }
}
