using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoAnalytics : MonoBehaviour
{
    MainMenu mainMenuScript;
    public int freeModeStartData;
    //public int freeModeWinData;
    //public int freeModeGameOverData;
    public int level1StartData;
    public int level1WinData;
    public int level1GameOverData;
    public int level2StartData;
    public int level2WinData;
    public int level2GameOverData;

    public GameObject analyticsUI;
    [SerializeField] TextMeshProUGUI freeModeStartDataText;
    [SerializeField] TextMeshProUGUI level1StartDataText;
    [SerializeField] TextMeshProUGUI level1WinDataText;
    [SerializeField] TextMeshProUGUI level1GameOverDataText;
    [SerializeField] TextMeshProUGUI level2StartDataText;
    [SerializeField] TextMeshProUGUI level2WinDataText;
    [SerializeField] TextMeshProUGUI level2GameOverDataText;

    [SerializeField] TextMeshProUGUI level1AverageTimeDataText;
    [SerializeField] TextMeshProUGUI level2AverageTimeDataText;

    void Start()
    {
        mainMenuScript = GetComponent<MainMenu>();
        freeModeStartData = PlayerPrefs.GetInt("Level0StartData");
        //freeModeWinData = PlayerPrefs.GetInt("Level0WinData");
        //freeModeGameOverData = PlayerPrefs.GetInt("Level0GameOverData");
        level1StartData = PlayerPrefs.GetInt("Level1StartData");
        level1WinData = PlayerPrefs.GetInt("Level1WinData");
        level1GameOverData = PlayerPrefs.GetInt("Level1GameOverData");
        level2StartData = PlayerPrefs.GetInt("Level2StartData");
        level2WinData = PlayerPrefs.GetInt("Level2WinData");
        level2GameOverData = PlayerPrefs.GetInt("Level2GameOverData");

        UpdateAnalyticsTexts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!analyticsUI.activeInHierarchy)
            {
                analyticsUI.SetActive(true);
            }
            else analyticsUI.SetActive(false);
        }
    }

    public void UpdateAnalyticsTexts()
    {
        freeModeStartDataText.text = freeModeStartData.ToString();
        level1StartDataText.text = level1StartData.ToString();
        level1WinDataText.text = level1WinData.ToString();
        level1GameOverDataText.text = level1GameOverData.ToString();
        level2StartDataText.text = level2StartData.ToString();
        level2WinDataText.text = level2WinData.ToString();
        level2GameOverDataText.text = level2GameOverData.ToString();

        DisplayLevel1AverageTimeData();
        DisplayLevel2AverageTimeData();
    }
    public void DisplayLevel1AverageTimeData()
    {
        float level1AverageTime = PlayerPrefs.GetFloat("Level1AverageTime", 0);
        int minutes = Mathf.FloorToInt(level1AverageTime / 60);
        int seconds = Mathf.FloorToInt(level1AverageTime % 60);
        int miliseconds = (int)(level1AverageTime * 1000) % 1000;
        level1AverageTimeDataText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
    }
    public void DisplayLevel2AverageTimeData()
    {
        float level2AverageTime = PlayerPrefs.GetFloat("Level2AverageTime", 0);
        int minutes = Mathf.FloorToInt(level2AverageTime / 60);
        int seconds = Mathf.FloorToInt(level2AverageTime % 60);
        int miliseconds = (int)(level2AverageTime * 1000) % 1000;
        level2AverageTimeDataText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
    }
    public void ResetAnalyticsData()
    {
        for(int i = 0; i <= mainMenuScript.numberOfLevels; i++)
        {
            PlayerPrefs.SetInt("Level" + i + "StartData", 0);
            PlayerPrefs.SetInt("Level" + i + "WinData", 0);
            PlayerPrefs.SetInt("Level" + i + "GameOverData", 0);
            PlayerPrefs.SetInt("Level" + i + "RestartFromPauseData", 0);
            PlayerPrefs.SetInt("Level" + i + "RestartFromGameOverData", 0);
            PlayerPrefs.SetInt("Level" + i + "RestartFromWinData", 0);
            PlayerPrefs.SetInt("Level" + i + "QuitFromPauseData", 0);
            PlayerPrefs.SetInt("Level" + i + "QuitFromGameOverData", 0);
        }

        freeModeStartData = PlayerPrefs.GetInt("Level0StartData");
        //freeModeWinData = PlayerPrefs.GetInt("Level0WinData");
        //freeModeGameOverData = PlayerPrefs.GetInt("Level0GameOverData");
        level1StartData = PlayerPrefs.GetInt("Level1StartData");
        level1WinData = PlayerPrefs.GetInt("Level1WinData");
        level1GameOverData = PlayerPrefs.GetInt("Level1GameOverData");
        level2StartData = PlayerPrefs.GetInt("Level2StartData");
        level2WinData = PlayerPrefs.GetInt("Level2WinData");
        level2GameOverData = PlayerPrefs.GetInt("Level2GameOverData");
    }
}
