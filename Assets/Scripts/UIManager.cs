using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{


    public Text BacNum;
    public Text MoneyNum; 
    public Text ResultMoney;
    public GameObject TimerUI;

    static UIManager instance;

    public GameObject PausePanel;
    public GameObject ResultPanel;
    public GameObject StartPanel;

    public GameObject InfoPanel1;
    public GameObject InfoPanel2;
    public GameObject InfoPanel3;

    public GameObject DrunkSign;


    public RollingNumbers RollingNumber;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(UIManager)) as UIManager;
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        MoneyNum.text = "$" + GameController.Instance.MoneyNum.ToString();
        BacNum.text = "0.00%";
    }

    public void UpdateMoneyText()
    {
        MoneyNum.text = "$" + GameController.Instance.MoneyNum.ToString();
    }
    public void UpdateBacText()
    {
        if (0.02< GameController.Instance.BacNum  && GameController.Instance.BacNum <= 0.05)
        {
            BacNum.color = new Color(1f, 99 / 255f, 0f);
        }

        if (GameController.Instance.BacNum >0.05)
        {
            BacNum.color = Color.red;
        }
        BacNum.text = GameController.Instance.BacNum.ToString()+ "%"; 
    }

    public void PausePanelShow()
    {
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void PausePanelHide()
    {
        Time.timeScale = 1;
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        PausePanel.SetActive(false);
    }

    public void ResultPanelShow()
    {
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        ResultMoney.text = "$" + GameController.Instance.MoneyNum.ToString();
        ResultPanel.SetActive(true);
     //   AudioController.Instance.EventSFXAudioSource.clip = AudioController.Instance.GameOverSFX;
   //     AudioController.Instance.EventSFXAudioSource.Play();
    }

    public void HideStartPanel()
    {
        StartPanel.SetActive(false);
        InfoPanel1.SetActive(true);
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        AudioController.Instance.BackgourndFXAudioSource.Play();
    }
    public void SetInitalRewardValue(RollingNumbers rollingNumber, int startValue, int endValue)
    {
        rollingNumber.initialNum = startValue;
        rollingNumber.currentValue = startValue;
        rollingNumber.barNum.text = rollingNumber.currentValue.ToString();
        rollingNumber.endNum = endValue;
    }

    public void HideInofoPanel1()
    {
        InfoPanel1.SetActive(false);
        InfoPanel2.SetActive(true);
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
    }

    public void StartHideInfoPanel2Behavior()
    {
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        StartCoroutine(HideInofoPanel2());
    }
    public IEnumerator HideInofoPanel2()
    {
        InfoPanel2.SetActive(false);
        GameController.Instance.Target.enabled = true;
        yield return new WaitForSeconds(3);
        ShowInfoPanel3();
        StartCoroutine(GameController.Instance.GenerateNewPickup());
        yield break;
    }

    public void ShowInfoPanel3()
    {
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        InfoPanel3.SetActive(true);
    }

    public void HideInofoPanel3()
    {
        AudioController.Instance.PlayButtonSFX(AudioController.Instance.ButtonSFX1);
        InfoPanel3.SetActive(false);
    }

    public void TimerShow()
    {
        TimerUI.SetActive(true);
        GameController.Instance.TimeRunning = true;
        StartCoroutine(GameController.Instance.GenerateNewAlcohol());
    }
}
