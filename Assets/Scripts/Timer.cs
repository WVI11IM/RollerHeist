using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public float elapsedTime;
    public bool runTime;
    //float remainingTime;

    public static Timer Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        runTime = true;
    }

    void Update()
    {
        TimeIncrease();
    }

    void TimeIncrease()
    {
        if (runTime)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime += 0;
        }

        //SetScore(timerText);

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int miliseconds = (int)(elapsedTime * 1000) % 1000;
        //timerText.text = string.Format("{0:00} : {1:00} : {1:000}", minutes, seconds, miliseconds);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
        scoreText.text = timerText.text;
    }

    /*
    public void SetScore(TextMeshProUGUI score)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int miliseconds = (int)(elapsedTime * 1000) % 1000;
        //timerText.text = string.Format("{0:00} : {1:00} : {1:000}", minutes, seconds, miliseconds);
        score.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
    }
    */

    /*
    void TimeCountdown()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            //
        }

        remainingTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    */
}
