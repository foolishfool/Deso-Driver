using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{

    private bool isAlerting;
    private bool isWarning;
    private float drinksigntimer;
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


        if (GameController.Instance.GeneratedPickUpPosesObj.Contains(gameObject.transform.parent.gameObject))
        {
            GameController.Instance.GeneratedPickUpPosesObj.Remove(gameObject.transform.parent.gameObject);
        }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (GameController.Instance.Drunk)
            {

                GameObject sign = Instantiate(UIManager.Instance.DrunkSign, gameObject.transform.position, Quaternion.identity);
                sign.transform.parent = GameObject.Find("UI").transform;
                sign.transform.Find("Image").GetComponent<Image>().sprite = gameObject.GetComponent<Target>().LeaveImage;
                sign.GetComponent<DrunkSign>().BelongedPickUp = gameObject;
                sign.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 1f);
                GameController.Instance.CurrentPickUp = gameObject;


                //go away, you are drunk
                return;
            }
            if (!GameController.Instance.Pickupeffect.gameObject.activeSelf)
            {
                GameController.Instance.Pickupeffect.gameObject.SetActive(true);
            }
            GameController.Instance.Pickupeffect.Play();
            AudioController.Instance.PlayEventSFX(AudioController.Instance.PickUpSFX);
            if (!GameController.Instance.FirstPickup)
            {
          
                UIManager.Instance.TimerShow();
                GameController.Instance.FirstPickup = true;
            }

            UIManager.Instance.SetInitalRewardValue(UIManager.Instance.RollingNumber, GameController.Instance.MoneyNum, GameController.Instance.MoneyNum+50);

            GameController.Instance.MoneyNum += 50;
            //  UIManager.Instance.UpdateMoneyText();

            UIManager.Instance.RollingNumber.StartRolling();
            AudioController.Instance.PlayButtonSFX(AudioController.Instance.MoneySFX);
            //gameObject.transform.SetParent(other.gameObject.transform);
            gameObject.transform.DOMove(other.gameObject.transform.position, 0.5f).OnComplete(()=>Destroy(gameObject));

    
            if (GameController.Instance.GeneratedPickUpPosesObj.Contains(gameObject.transform.parent.gameObject))
            {
                
                GameController.Instance.GeneratedPickUpPosesObj.Remove(gameObject.transform.parent.gameObject);
            }
        }
    }
}
