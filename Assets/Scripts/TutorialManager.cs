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
    public GameObject tutorialStartUI;
    public GameObject tutorialTrickUI;
    public GameObject tutorialEndUI;
    public GameObject tutorialMeter;
    public Image[] tutorialMeterFills;
    public Animator tutorialKeysAnimator;

    private float tutorialTimer;
    private int tutorialJumpNumber;
    private bool tutorialIsPaused = false;
    private bool canUnpauseWithSpacebar = false;
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
        TutorialStartPause();
    }
    private void Update()
    {
        tutorialKeysAnimator.SetInteger("stepNumber", tutorialStepNumber);

        if (tutorialStepNumber >= 1 && tutorialStepNumber <= 4)
        {
            TutorialInputTimer();
            for (int i = 0; i < tutorialMeterFills.Length; i++)
            {
                tutorialMeterFills[i].fillAmount = tutorialTimer / 1.5f;
            }

            if (tutorialTimer > 1.5f)
            {
                TutorialNextStep();
            }
        }

        if (tutorialStepNumber == 5)
        {
            TutorialJumpCount();
            for (int i = 0; i < tutorialMeterFills.Length; i++)
            {
                tutorialMeterFills[i].fillAmount = (float)tutorialJumpNumber / 3;
            }

            if (tutorialJumpNumber >= 3)
            {
                TutorialNextStep();
            }
        }

        if ((tutorialStepNumber >= 1 && tutorialStepNumber <= 5) || tutorialStepNumber == 24)
        {
            tutorialMeter.gameObject.SetActive(true);
        }
        else
        {
            tutorialMeter.gameObject.SetActive(false);
        }

        if ((tutorialStepNumber >= 13 && tutorialStepNumber <= 16) || (tutorialStepNumber >= 20 && tutorialStepNumber <= 22))
        {
            TutorialTrickDetect();
        }

        if (tutorialStepNumber == 18)
        {
            TutorialBoostDetect();
        }

        if (tutorialStepNumber == 24)
        {
            TutorialShootDetect();
            for (int i = 0; i < tutorialMeterFills.Length; i++)
            {
                tutorialMeterFills[i].fillAmount = (float)tutorialTargetCounter / 3;
            }
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
            case 13:
                tutorialText.text = "[ESPACO] no ar\nFaca truques";
                break;
            case 16:
                tutorialText.text = "[ESPACO] no ar\nFaca truques";
                break;
            case 18:
                tutorialText.text = "[SHIFT]\nAtive o boost";
                break;
            case 20:
                tutorialText.text = "[ESPACO] no ar\nFaca truques";
                break;
            case 22:
                tutorialText.text = "[ESPACO] no ar\nFaca truques";
                break;
            case 24:
                tutorialText.text = "[BOTAO ESQUERDO DO MOUSE]\nAtire nos 3 alvos";
                break;
            default:
                tutorialText.text = null;
                break;
        }

        TutorialTrickResume();
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
            tutorialText.color = new Vector4(0.75f, 1, 0.75f, 1);
            tutorialText.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            tutorialTimer += Time.deltaTime;
        }
        else
        {
            tutorialText.color = new Vector4(1, 1, 1, 1);
            tutorialText.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TutorialJumpCount()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerMovement.isGrounded)
        {
            tutorialJumpNumber++;
            tutorialText.color = new Vector4(0.75f, 1, 0.75f, 1);
            tutorialText.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else
        {
            tutorialText.color = new Vector4(1, 1, 1, 1);
            tutorialText.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TutorialTrickDetect()
    {
        if (Input.GetKey(KeyCode.Space) && !playerMovement.isGrounded)
        {
            tutorialText.color = new Vector4(0.75f, 1, 0.75f, 1);
            tutorialText.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else
        {
            tutorialText.color = new Vector4(1, 1, 1, 1);
            tutorialText.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TutorialBoostDetect()
    {
        if (Input.GetKey(KeyCode.LeftShift) && playerMovement.isGrounded)
        {
            tutorialText.color = new Vector4(0.75f, 1, 0.75f, 1);
            tutorialText.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else
        {
            tutorialText.color = new Vector4(1, 1, 1, 1);
            tutorialText.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TutorialShootDetect()
    {
        if (Input.GetKey(KeyCode.Mouse0) && playerMovement.isGrounded)
        {
            tutorialText.color = new Vector4(0.75f, 1, 0.75f, 1);
            tutorialText.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else
        {
            tutorialText.color = new Vector4(1, 1, 1, 1);
            tutorialText.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TutorialNextStep()
    {
        tutorialStepNumber++;
        tutorialStep[tutorialStepNumber].stepActions.Invoke();
        tutorialTimer = 0;
    }

    public void TutorialStartPause()
    {
        tutorialIsPaused = true;
        Time.timeScale = 0;
        tutorialStartUI.SetActive(true);
        AllowTutorialPause(false);
    }

    public void TutorialStartResume()
    {
        if (tutorialIsPaused)
        {
            tutorialIsPaused = false;
            Time.timeScale = 1;
            tutorialStartUI.SetActive(false);
            AllowTutorialPause(true);
        }
    }

    public void TutorialTrickPause()
    {
        tutorialIsPaused = true;
        Time.timeScale = 0;
        tutorialTrickUI.SetActive(true);
        AllowTutorialPause(false);
    }

    public void TutorialTrickResume()
    {
        if (tutorialIsPaused && Input.GetKeyDown(KeyCode.Space) && tutorialStepNumber > 0 && canUnpauseWithSpacebar)
        {
            tutorialIsPaused = false;
            Time.timeScale = 1;
            tutorialTrickUI.SetActive(false);
            AllowTutorialPause(true);
        }
    }

    public void TutorialEndPause()
    {
        if (tutorialComplete)
        {
            tutorialIsPaused = true;
            Time.timeScale = 0;
            tutorialEndUI.SetActive(true);
            AllowTutorialPause(false);
        }
    }

    public void UnpauseWithSpacebar(bool canUnpause)
    {
        canUnpauseWithSpacebar = canUnpause;
    }

    private void AllowTutorialPause(bool canAllowPause)
    {
        GameManager.Instance.canPause = canAllowPause;
    }
}

[System.Serializable]
public class TutorialStep
{
    public string step;

    public UnityEvent stepActions;
}
