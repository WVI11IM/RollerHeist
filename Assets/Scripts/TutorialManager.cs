using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public MovementTest2 playerMovement;

    public int tutorialStepNumber;

    public float tutorialTimer;
    public int tutorialJumpNumber;
    public int tutorialTargetCounter;

    public GameObject[] tutorialIndicators;

    public TutorialStep[] tutorialStep;

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

            if (tutorialTimer > 2)
            {
                TutorialNextStep();
            }
        }

        if (tutorialStepNumber == 5)
        {
            TutorialInputCount();

            if (tutorialJumpNumber >= 3)
            {
                TutorialNextStep();
            }
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
