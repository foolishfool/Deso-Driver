using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private bool isAlerting;
    private bool isWarning;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();
    }

    public IEnumerator LifeBehavior()
    {
        yield return new WaitForSeconds(15f);
        isAlerting = true;
        yield return new WaitForSeconds(10f);
        isAlerting = false;
        isWarning = true;
        yield return new WaitForSeconds(15f);
        Disappear();
        yield break;
    }


    private void UIUpdate()
    {
        if (isAlerting)
            UIAlert();
        else if (isWarning)
            UIWarning();
        else UINormal();
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }

    public void UINormal()
    {
        if (gameObject.GetComponent<Target>().NeedArrowIndicator && !gameObject.GetComponent<Target>().IsVisible)
        {
            gameObject.GetComponent<Target>().BelongedIndicator.SetImag(gameObject.GetComponent<Target>().StartImage);
        }
        if (gameObject.GetComponent<Target>().NeedBoxIndicator && gameObject.GetComponent<Target>().IsVisible)
        {

            gameObject.GetComponent<Target>().BelongedIndicator.SetImageColor(gameObject.GetComponent<Target>().TargetColor);
        }


    }

    public void UIAlert()
    {
        if (gameObject.GetComponent<Target>().NeedArrowIndicator && !gameObject.GetComponent<Target>().IsVisible)
        {
            gameObject.GetComponent<Target>().BelongedIndicator.SetImag(gameObject.GetComponent<Target>().WaitingImage);
        }
        if (gameObject.GetComponent<Target>().NeedBoxIndicator && gameObject.GetComponent<Target>().IsVisible)
        {
            Color orange = new Color(255F, 162F, 0F);
            gameObject.GetComponent<Target>().BelongedIndicator.SetImageColor(orange);
        }
           

    }

    public void UIWarning()
    {
        if (gameObject.GetComponent<Target>().NeedArrowIndicator && !gameObject.GetComponent<Target>().IsVisible)
            gameObject.GetComponent<Target>().BelongedIndicator.SetImag(gameObject.GetComponent<Target>().LeaveImage);
        if (gameObject.GetComponent<Target>().NeedBoxIndicator && gameObject.GetComponent<Target>().IsVisible)
            gameObject.GetComponent<Target>().BelongedIndicator.SetImageColor(Color.red);
    }

}
