using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public DefaultMap defaultMap;
    private string currentControlScheme;

    [Space]
    [Header("Rotation (ControlsRotation)")]
    public float rotationDirection;
    private bool negativeRotationFirstFrame;
    private bool positiveRotationFirstFrame;
    public bool isHoldingRotate;
    private InputAction rotateAction;

    [Space]
    [Header("Move (ControlsDirection)")]
    public Vector2 moveDirection;
    public bool isHoldingMove;
    private InputAction moveAction;

    [Space]
    [Header("Drift (for Gamepad/Mouse)")]
    public bool isHoldingDrift;
    private InputAction driftAction;

    [Space]
    [Header("Jump/Trick")]
    public bool isHoldingJumpTrick;
    private bool jumpTrickFirstFrame;
    private InputAction jumpTrickAction;

    [Space]
    [Header("Boost")]
    public bool isHoldingBoost;
    private bool boostFirstFrame;
    private InputAction boostAction;

    [Space]
    [Header("Shoot (for Mouse)")]
    public bool isHoldingShoot;
    private InputAction shootAction;

    [Space]
    [Header("Aim and Shoot (for Gamepad)")]
    public Vector2 aimAndShootDirection;
    public bool isHoldingAimAndShoot;
    private InputAction aimAndShootAction;

    [Space]
    [Header("Reload")]
    public bool reloaded;
    private InputAction reloadAction;

    [Space]
    [Header("Brake")]
    public bool isHoldingBrake;
    private InputAction brakeAction;

    [Space]
    [Header("Pause")]
    public bool paused;
    private InputAction pauseAction;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        currentControlScheme = playerInput.currentControlScheme;

        switch (playerInput.currentActionMap.name)
        {
            case "UI":
                defaultMap = DefaultMap.UI;
                break;
            case "ControlsRotation":
                defaultMap = DefaultMap.ControlsRotation;
                break;
            case "ControlsDirection":
                defaultMap = DefaultMap.ControlsDirection;
                break;
        }

        if(defaultMap == DefaultMap.UI)
        {

        }
        else
        {
            if (defaultMap == DefaultMap.ControlsRotation)
            {
                rotateAction = playerInput.actions.FindAction("Rotate");
                rotateAction.performed += OnRotatePerformed;
                rotateAction.canceled += OnRotateCanceled;
            }

            else if (defaultMap == DefaultMap.ControlsDirection)
            {
                moveAction = playerInput.actions.FindAction("Move");
                moveAction.performed += OnMovePerformed;
                moveAction.canceled += OnMoveCanceled;
            }

            driftAction = playerInput.actions.FindAction("Drift");
            jumpTrickAction = playerInput.actions.FindAction("JumpTrick");
            boostAction = playerInput.actions.FindAction("Boost");
            shootAction = playerInput.actions.FindAction("Shoot [Mouse]");
            aimAndShootAction = playerInput.actions.FindAction("AimAndShoot [Gamepad]");
            reloadAction = playerInput.actions.FindAction("Reload");
            brakeAction = playerInput.actions.FindAction("Brake");
            pauseAction = playerInput.actions.FindAction("Pause");

            driftAction.performed += OnDriftPerformed;
            driftAction.canceled += OnDriftCanceled;
            jumpTrickAction.performed += OnJumpTrickPerformed;
            jumpTrickAction.canceled += OnJumpTrickCanceled;
            boostAction.performed += OnBoostPerformed;
            boostAction.canceled += OnBoostCanceled;
            aimAndShootAction.performed += OnAimAndShootPerformed;
            aimAndShootAction.canceled += OnAimAndShootCanceled;

            shootAction.started += ctx => isHoldingShoot = true;
            shootAction.canceled += ctx => isHoldingShoot = false;
            reloadAction.started += ctx => reloaded = true;
            reloadAction.canceled += ctx => reloaded = false;
            brakeAction.started += ctx => isHoldingBrake = true;
            brakeAction.canceled += ctx => isHoldingBrake = false;
            pauseAction.started += ctx => paused = true;
            pauseAction.canceled += ctx => paused = false;
        }
    }


    void OnDestroy()
    {
        if (defaultMap == DefaultMap.UI)
        {

        }
        else
        {
            if (defaultMap == DefaultMap.ControlsRotation)
            {
                rotateAction.performed -= OnRotatePerformed;
                rotateAction.canceled -= OnRotateCanceled;
            }
            else if (defaultMap == DefaultMap.ControlsDirection)
            {
                moveAction.performed -= OnMovePerformed;
                moveAction.canceled -= OnMoveCanceled;
            }
            driftAction.performed -= OnDriftPerformed;
            driftAction.canceled -= OnDriftCanceled;
            jumpTrickAction.performed -= OnJumpTrickPerformed;
            jumpTrickAction.canceled -= OnJumpTrickCanceled;
            boostAction.performed -= OnBoostPerformed;
            boostAction.canceled -= OnBoostCanceled;
            shootAction.performed -= OnShootPerformed;
            shootAction.canceled -= OnShootCanceled;
            aimAndShootAction.performed -= OnAimAndShootPerformed;
            aimAndShootAction.canceled -= OnAimAndShootCanceled;
            brakeAction.performed -= OnBrakePerformed;
            brakeAction.performed -= OnBrakeCanceled;
        }
    }

    void Update()
    {
        if (defaultMap == DefaultMap.UI)
        {
            
        }
        else
        {
            if (defaultMap == DefaultMap.ControlsRotation)
            {
                if (!isHoldingRotate) rotationDirection = 0;
            }

            else if (defaultMap == DefaultMap.ControlsDirection)
            {
                if (!isHoldingMove) moveDirection = new Vector2(0, 0);
            }
        }

        if (isHoldingShoot)
        {
            aimAndShootDirection = new Vector2(0, 0);
        }
        else if (isHoldingAimAndShoot)
        {
            Vector2 aimAndShootDirectionValue = aimAndShootAction.ReadValue<Vector2>();
            if (aimAndShootDirectionValue.magnitude > 0.25f)
            {
                aimAndShootDirection = aimAndShootDirectionValue.normalized;
            }
        }
    }

    private void LateUpdate()
    {
        if (defaultMap == DefaultMap.UI)
        {

        }
        else
        {
            if (defaultMap == DefaultMap.ControlsRotation)
            {
                if (negativeRotationFirstFrame) negativeRotationFirstFrame = false;
                if (positiveRotationFirstFrame) positiveRotationFirstFrame = false;
            }
            if (jumpTrickFirstFrame) jumpTrickFirstFrame = false;
            if (boostFirstFrame && isHoldingBoost) boostFirstFrame = false;

            if (reloaded) reloaded = false;
            if (paused) paused = false;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveDirectionValue = moveAction.ReadValue<Vector2>();
        if(moveDirectionValue.magnitude > 0.25f)
        {
            moveDirection = moveDirectionValue.normalized;
        }
        isHoldingMove = true;
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveDirection = new Vector2(0, 0);
        isHoldingMove = false;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        float rotationDirectionValue = rotateAction.ReadValue<float>();
        if (rotationDirectionValue < 0.25f) rotationDirection = -1;
        else if (rotationDirectionValue > 0.25f) rotationDirection = 1;

        if (rotationDirection == -1 && !isHoldingRotate)
        {
            negativeRotationFirstFrame = true;
            positiveRotationFirstFrame = false;
        }
        else if (rotationDirection == 1 && !isHoldingRotate)
        {
            positiveRotationFirstFrame = true;
            negativeRotationFirstFrame = false;
        }
        else
        {
            negativeRotationFirstFrame = false;
            positiveRotationFirstFrame = false;
        }
        isHoldingRotate = true;
    }

    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        rotationDirection = 0;
        isHoldingRotate = false;
    }

    public bool IsNegativeRotationFirstFrame()
    {
        return negativeRotationFirstFrame;
    }

    public bool IsPositiveRotationFirstFrame()
    {
        return positiveRotationFirstFrame;
    }

    public void OnDriftPerformed(InputAction.CallbackContext context)
    {
        isHoldingDrift = true;
    }

    public void OnDriftCanceled(InputAction.CallbackContext context)
    {
        isHoldingDrift = false;
    }

    private void OnJumpTrickPerformed(InputAction.CallbackContext context)
    {
        if (Time.timeScale != 0)
        {
            jumpTrickFirstFrame = true;
            isHoldingJumpTrick = true;
        }
    }

    private void OnJumpTrickCanceled(InputAction.CallbackContext context)
    {
        isHoldingJumpTrick = false;
    }

    public bool IsJumpTrickFirstFrame()
    {
        return jumpTrickFirstFrame;
    }

    private void OnBoostPerformed(InputAction.CallbackContext context)
    {
        boostFirstFrame = true;
        isHoldingBoost = true;
    }

    private void OnBoostCanceled(InputAction.CallbackContext context)
    {
        isHoldingBoost = false;
    }

    public bool IsBoostFirstFrame()
    {
        return boostFirstFrame;
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        isHoldingShoot = true;
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isHoldingShoot = false;
    }
    private void OnAimAndShootPerformed(InputAction.CallbackContext context)
    {
        Vector2 aimAndShootDirectionValue = aimAndShootAction.ReadValue<Vector2>();
        if (aimAndShootDirectionValue.magnitude > 0.25f)
        {
            isHoldingAimAndShoot = true;
        }
    }

    private void OnAimAndShootCanceled(InputAction.CallbackContext context)
    {
        aimAndShootDirection = new Vector2(0, 0);
        isHoldingAimAndShoot = false;
    }
    private void OnBrakePerformed(InputAction.CallbackContext context)
    {
        isHoldingBrake = true;
    }

    private void OnBrakeCanceled(InputAction.CallbackContext context)
    {
        isHoldingBrake = false;
    }
}

[System.Serializable]
public enum DefaultMap
{
    ControlsRotation,
    ControlsDirection,
    UI
}