using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankAnimationData {
    public float animationDuration;

    public float animationIntesity;

    public float wheelRotationSpeed;
}

public class TankAnimationSystem : MonoBehaviour
{
    private Player owner;
    private TankAnimationData data;
    private GameObject playerTank; 
    public float springForce = 50.0f;
    private List<GameObject> leftWheels = new List<GameObject>();
    private List<GameObject> rightWheels = new List<GameObject>();

    public void Initialize(Player owner, TankAnimationData data)
    {
        this.owner = owner;
        this.data = data;
        
        playerTank = transform.Find("PlayerTank").gameObject;
        leftWheels.Add(playerTank.transform.Find("TankFree_Wheel_b_left").gameObject);
        leftWheels.Add(playerTank.transform.Find("TankFree_Wheel_f_left").gameObject);
        rightWheels.Add(playerTank.transform.Find("TankFree_Wheel_b_right").gameObject);
        rightWheels.Add(playerTank.transform.Find("TankFree_Wheel_f_right").gameObject);

        PlayAnimation(AnimationState.NotMoving, RotationState.NotRotating);
    }

    // This gets called each FixedUpdate by the MovementSystem
    public void AnimateMovement(float verticalInput, float horizontalInput, Vector3 velocity)
    {
        float leftWheelRotation, rightWheelRotation;
        CalculateWheelRotations(verticalInput, horizontalInput, out leftWheelRotation, out rightWheelRotation);

        RotateWheels(leftWheels, leftWheelRotation);
        RotateWheels(rightWheels, rightWheelRotation);
    }


    private void RotateWheels(List<GameObject> wheels, float rotationDirection)
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            float rotationAmount = rotationDirection * data.wheelRotationSpeed * Time.deltaTime;
            wheels[i].transform.Rotate(new Vector3(rotationAmount, 0, 0));
        }
    }

    private void CalculateWheelRotations(float verticalInput, float horizontalInput, out float leftRotation, out float rightRotation)
    {
        float inputScaleFactor = 10f;
        verticalInput *= inputScaleFactor;
        horizontalInput *= inputScaleFactor;

        if (Mathf.Approximately(verticalInput, 0) && !Mathf.Approximately(horizontalInput, 0))
        {
            // Turning without moving forward or backward
            leftRotation = horizontalInput;
            rightRotation = -horizontalInput;
        }
        else
        {
            float turningRadius = Mathf.Abs(horizontalInput);
            float insideWheelSpeedModifier = 1f - turningRadius * 0.5f;

            if (horizontalInput > 0)
            {
                leftRotation = verticalInput + horizontalInput;
                rightRotation = verticalInput * insideWheelSpeedModifier - horizontalInput;
            }
            else
            {
                leftRotation = verticalInput * insideWheelSpeedModifier + horizontalInput;
                rightRotation = verticalInput - horizontalInput;
            }
        }
    }

    private void PlayAnimation(AnimationState animState, RotationState rotState)
    {
        // Stop all animations
        LeanTween.cancel(gameObject);

        var animationIntesity = data.animationIntesity;
        var animationDuration = data.animationDuration;

        // Play the appropriate animation based on the given state
        switch (animState)
        {
            case AnimationState.MovingForward:
                LeanTween.rotateX(gameObject, -animationIntesity, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
            case AnimationState.MovingBackward:
                LeanTween.rotateX(gameObject, animationIntesity, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
            case AnimationState.NotMoving:
                LeanTween.rotateX(gameObject, 0, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
        }

        switch (rotState)
        {
            case RotationState.RotatingLeft:
                LeanTween.rotateZ(gameObject, -animationIntesity, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
            case RotationState.RotatingRight:
                LeanTween.rotateZ(gameObject, animationIntesity, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
            case RotationState.NotRotating:
                LeanTween.rotateZ(gameObject, 0f, animationDuration).setEase(LeanTweenType.easeOutCubic).setLoopPingPong();
                break;
        }
    }

    // Call this method to update the animations based on the current state
    public void UpdateAnimations(bool isMovingForward, bool isMovingBackward, bool isRotatingLeft, bool isRotatingRight)
    {
        if (isMovingForward)
        {
            PlayAnimation(AnimationState.MovingForward, RotationState.NotRotating);
        }
        else if (isMovingBackward)
        {
            PlayAnimation(AnimationState.MovingBackward, RotationState.NotRotating);
        }
        else
        {
            if (isRotatingLeft)
            {
                PlayAnimation(AnimationState.NotMoving, RotationState.RotatingLeft);
            }
            else if (isRotatingRight)
            {
                PlayAnimation(AnimationState.NotMoving, RotationState.RotatingRight);
            }
            else
            {
                PlayAnimation(AnimationState.NotMoving, RotationState.NotRotating);
            }
        }
    }

    // Define the possible animation states
    private enum AnimationState
    {
        MovingForward,
        MovingBackward,
        NotMoving
    }

    // Define the possible rotation states
    private enum RotationState
    {
        RotatingLeft,
        RotatingRight,
        NotRotating
    }
}
