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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.World);
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

        GameController.Instance.BacNum += BacNum;

        GameController.Instance.BacNum = (float)Math.Round(GameController.Instance.BacNum * 100f) / 100f;

        UIManager.Instance.UpdateBacText();

        if (GameController.Instance.BacNum >0.05)
        {
            GameController.Instance.TwirEffectIncrease();
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
