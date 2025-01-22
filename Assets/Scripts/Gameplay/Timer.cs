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

    public static Timer Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        runTime = false;
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

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int miliseconds = (int)(elapsedTime * 1000) % 1000;
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
        scoreText.text = timerText.text;
    }

    public void StartTimer()
    {
        runTime = true;
    }
}
