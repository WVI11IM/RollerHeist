using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class InactivitySceneChange : MonoBehaviour
{
    public float timeBeforeReset = 300;
    private float timer;

    private InputSystemUIInputModule uiInputModule;
    private List<InputAction> inputActions = new List<InputAction>();

    void Start()
    {
        timer = timeBeforeReset;
        uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();

        if (uiInputModule != null)
        {
            // Collect all relevant input actions
            //inputActions.Add(uiInputModule.point.action);
            inputActions.Add(uiInputModule.leftClick.action);
            inputActions.Add(uiInputModule.rightClick.action);
            inputActions.Add(uiInputModule.middleClick.action);
            inputActions.Add(uiInputModule.scrollWheel.action);
            inputActions.Add(uiInputModule.move.action);
            inputActions.Add(uiInputModule.submit.action);
            inputActions.Add(uiInputModule.cancel.action);
            inputActions.Add(uiInputModule.trackedDevicePosition.action);
            inputActions.Add(uiInputModule.trackedDeviceOrientation.action);
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        foreach (var action in inputActions)
        {
            if (action == null)
                continue;

            if (action.triggered)
            {
                // Special check for move action
                if (action == uiInputModule.move.action)
                {
                    Vector2 moveValue = action.ReadValue<Vector2>();
                    if (moveValue.magnitude < 0.5f) // Ignore tiny movements
                        continue;
                }
                timer = timeBeforeReset;
                Debug.Log($"Input triggered: {action.name}");
            }
        }

        if(timer<= 0)
        {
            timer = timeBeforeReset;
            PlayerPrefs.SetInt("titleScreenActivated", 0);
            PlayerPrefs.SetInt("justPlayedLevel", 0);
            PlayerPrefs.SetInt("justPlayedTutorial", 0);
            SceneManager.LoadScene("Intro");
        }
    }
}
