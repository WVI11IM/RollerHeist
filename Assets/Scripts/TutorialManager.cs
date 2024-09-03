using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public MovementTest2 playerMovement;

    public int tutorialStepNumber;

    public TextMeshProUGUI tutorialText;
    public GameObject tutorialMeter;
    public Image[] tutorialMeterFills;
    private float tutorialTimer;
    private int tutorialJumpNumber;
    [HideInInspector] public int tutorialTargetCounter;

    public GameObject[] tutorialIndicators;

    public TutorialStep[] tutorialStep;
    public bool tutorialComplete = false;

    public static TutorialManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tutorialIndicators = GameObject.FindGameObjectsWithTag("TutorialGuide");
    }
    private void Update()
    {       
        if (tutorialStepNumber >= 1 && tutorialStepNumber <= 4)
        {
            TutorialInputTimer();
            for (int i = 0; i < tutorialMeterFills.Length; i++)
            {
                tutorialMeterFills[i].fillAmount = tutorialTimer / 1.25f;
            }

            if (tutorialTimer > 1.25f)
            {
                TutorialNextStep();
            }
        }

        if (tutorialStepNumber == 5)
        {
            TutorialInputCount();
            for (int i = 0; i < tutorialMeterFills.Length; i++)
            {
                tutorialMeterFills[i].fillAmount = (float)tutorialJumpNumber / 3;
            }

            if (tutorialJumpNumber >= 3)
            {
                TutorialNextStep();
            }
        }

        if (tutorialStepNumber < 1 || tutorialStepNumber > 5)
        {
            tutorialMeter.gameObject.SetActive(false);
        }
        else
        {
            tutorialMeter.gameObject.SetActive(true);
        }

        if (tutorialTargetCounter >= 3 && !tutorialComplete)
        {
            TutorialNextStep();
            tutorialComplete = true;
        }

        switch (tutorialStepNumber)
        {
            case 1:
                tutorialText.text = "[A]\nGire para o sentido anti - horario";
                break;
            case 2:
                tutorialText.text = "[D]\nGire para o sentido horario";
                break;
            case 3:
                tutorialText.text = "[A] x2\nFaca um drift pro sentido anti-horario";
                break;
            case 4:
                tutorialText.text = "[D] x2\nFaca um drift pro sentido horario";
                break;
            case 5:
                tutorialText.text = "[ESPACO]\nPule 3 vezes";
                break;
            default:
                tutorialText.text = null;
                break;
        }
    }

    public void TutorialInputTimer()
    {
        bool isInputting = false;

        switch (tutorialStepNumber)
        {
            case 1:
                isInputting = (Input.GetAxis("Horizontal") == -1);
                break;
            case 2:
                isInputting = (Input.GetAxis("Horizontal" ) == 1);
                break;
            case 3:
                isInputting = playerMovement.isDriftingA;
                break;
            case 4:
                isInputting = playerMovement.isDriftingD;
                break;
        }

        if (isInputting)
        {

            tutorialTimer += Time.deltaTime;
        }
        else
        {

        }
    }

    public void TutorialInputCount()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerMovement.isGrounded)
        {
            tutorialJumpNumber++;
        }
    }

    public void TutorialNextStep()
    {
        tutorialStepNumber++;
        tutorialStep[tutorialStepNumber].stepActions.Invoke();
        tutorialTimer = 0;
    }


    /*
    public void ResetTutorialInputTimer()
    {
        tutorialTimer = 0;
    }
    */
}

[System.Serializable]
public class TutorialStep
{
    public string step;

    public UnityEvent stepActions;
}
