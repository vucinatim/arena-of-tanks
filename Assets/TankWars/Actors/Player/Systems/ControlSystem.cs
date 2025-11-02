using System;
using UnityEngine;

public interface IInputHandler
{
    float GetVerticalAxis();
    float GetHorizontalAxis();
    bool GetAbilityTrigger();
    void ButtonInput(string input);
}

public class AirConsoleInputHandler : IInputHandler
{
    private bool upPressed = false;
    private bool downPressed = false;
    private bool leftPressed = false;
    private bool rightPressed = false;
    private float verticalInput;
    private float horizontalInput;
    private bool abilityTrigger;
    public event Action OnAbilityTrigger;

    public float GetVerticalAxis()
    {
        return verticalInput;
    }

    public float GetHorizontalAxis()
    {
        return horizontalInput;
    }

    public bool GetAbilityTrigger()
    {
        return abilityTrigger;
    }

    public void ButtonInput(string input)
    {
        switch (input)
        {
            case "up":
                upPressed = true;
                break;
            case "down":
                downPressed = true;
                break;
            case "left":
                leftPressed = true;
                break;
            case "right":
                rightPressed = true;
                break;
            case "up-up":
                upPressed = false;
                break;
            case "down-up":
                downPressed = false;
                break;
            case "left-up":
                leftPressed = false;
                break;
            case "right-up":
                rightPressed = false;
                break;
            case "ability":
                OnAbilityTrigger?.Invoke();
                break;
        }

        UpdateMovementInputs();
    }

    private void UpdateMovementInputs()
    {
        // Update vertical input
        if (upPressed && !downPressed)
            verticalInput = 1f;
        else if (downPressed && !upPressed)
            verticalInput = -1f;
        else
            verticalInput = 0f;

        // Update horizontal input
        if (leftPressed && !rightPressed)
            horizontalInput = -1f;
        else if (rightPressed && !leftPressed)
            horizontalInput = 1f;
        else
            horizontalInput = 0f;
    }
}

[System.Serializable]
public class ControlsData
{
    public bool useAirConsoleInput = true;
}

public class ControlSystem : MonoBehaviour
{
    private Player owner;

    [SerializeField]
    private PhysicsMovementSystem movementSystem;

    [SerializeField]
    private AbilityQueueSystem abilityQueueSystem;

    [SerializeField]
    private TankAudioSystem tankAudioSystem;

    private IInputHandler inputHandler;

    public void Initialize(Player owner, ControlsData data)
    {
        this.owner = owner;

        movementSystem = GetComponent<PhysicsMovementSystem>();
        abilityQueueSystem = GetComponent<AbilityQueueSystem>();
        tankAudioSystem = GetComponent<TankAudioSystem>();

        inputHandler = new AirConsoleInputHandler();
        ((AirConsoleInputHandler)inputHandler).OnAbilityTrigger +=
            abilityQueueSystem.TriggerAbility;
    }

    void FixedUpdate()
    {
        if (inputHandler == null)
        {
            Debug.LogError("No input handler found for " + gameObject.name);
            return;
        }
        float transAmount = inputHandler.GetVerticalAxis();
        float rotateAmount = inputHandler.GetHorizontalAxis();
        movementSystem.Move(transform, transAmount, rotateAmount);
        tankAudioSystem.HandleMovementAudio(transAmount, rotateAmount);
    }

    public void ButtonInput(string input)
    {
        if (inputHandler != null)
        {
            inputHandler.ButtonInput(input);
        }
    }
}
