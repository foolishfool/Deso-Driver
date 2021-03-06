using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 120;
    public Text timeText;


    void Update()
    {
        if (GameController.Instance.TimeRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                GameController.Instance.TimeRunning = false;
                GameController.Instance.GameFinish();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:0}M{1:00}S", minutes, seconds);

        if (seconds <=30 && minutes == 0)
        {
            timeText.color = Color.red;
        }
    }
}
