using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AlcoholSign : MonoBehaviour
{
    public GameObject BelongedAlcohol;
    float alcoholsigntimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        alcoholsigntimer += Time.deltaTime;
        if (BelongedAlcohol)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(BelongedAlcohol.transform.position);
            transform.position = new Vector3(screenPos.x, screenPos.y + 80, screenPos.z);
        }
        else transform.DOScale(Vector3.zero, 0.5f).OnComplete( () => {
            BelongedAlcohol.GetComponent<Alcohol>().HasSign = false;
            Destroy(gameObject); } );

        if (alcoholsigntimer > 5f)
        {
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(
            () => {
                BelongedAlcohol.GetComponent<Alcohol>().HasSign = false;
                Destroy(gameObject);
            }
            );
        }
    }
}
