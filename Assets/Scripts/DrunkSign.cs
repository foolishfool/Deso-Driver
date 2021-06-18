using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrunkSign : MonoBehaviour
{

    public GameObject BelongedPickUp;
    float drinksigntimer;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        drinksigntimer += Time.deltaTime;
        if (BelongedPickUp)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(BelongedPickUp.transform.position);
            transform.position = new Vector3(screenPos.x, screenPos.y + 80, screenPos.z);
        }
        else transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            BelongedPickUp.GetComponent<Pickup>().HasSign = false;
            Destroy(gameObject);
        });

        if (drinksigntimer > 5f)
        {
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
                BelongedPickUp.GetComponent<Pickup>().HasSign = false;
                Destroy(gameObject);
            });
        }
    }
}
