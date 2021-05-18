using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Alcohol : MonoBehaviour
{
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
        yield return new WaitForSeconds(15f);
        Disappear();
        yield break;
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameController.Instance.TwirEffectIncrease();
        Disappear();
    }
}
