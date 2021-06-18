using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Pathfinding;

public class PoliceCarSign : MonoBehaviour
{
    public GameObject BelongedPoliceCar;
    float signtimer;
    public Text Info;
    public Image Pic;
    public Sprite Fail;
    public Sprite Pass;
    public Sprite Suspend;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

        if (BelongedPoliceCar)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(BelongedPoliceCar.transform.position);
            transform.position = new Vector3(screenPos.x, screenPos.y + 80, screenPos.z);
  
        }
        else transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(gameObject));
    }


    public IEnumerator ShowInfo(bool isDrunk)
    {
        yield return new WaitForSeconds(1.5f);

        if (isDrunk)
        {
            Info.text = "Fail!";
            Info.color = Color.red;
            Pic.sprite = Fail;
            AudioController.Instance.PlayEventSFX(AudioController.Instance.FailSFX);
            yield return new WaitForSeconds(1f);
            Pic.sprite = Suspend;
            Info.text = "Lose your licence!";
            AudioController.Instance.PlayEventSFX(AudioController.Instance.LicenceLoseSFX);
            GameController.Instance.GameFinish();          
            UIManager.Instance.ResultInfo.text = "GAME OVER";
            AudioController.Instance.PoliceCarAudioSource.DOFade(0, 1f);
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        else
        {
            Info.text = "Pass!";
            Pic.sprite = Pass;
            AudioController.Instance.PlayEventSFX(AudioController.Instance.PassSFX);
            Info.color = Color.green;
            yield return new WaitForSeconds(0.8f);
            BelongedPoliceCar = null;   
            GameController.Instance.PoliceCar.GetComponent<AIDestinationSetter>().target = GameController.Instance.PoliceIntialTransform;
            AudioController.Instance.PoliceCarAudioSource.DOFade(0.6f, 3f);
        }

    }
}
