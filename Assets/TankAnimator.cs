using UnityEngine;

public class TankAnimator : MonoBehaviour
{
    public float animationDuration = 1f; // Duration of the animations

    public float animationIntesity = 5f; // Intensity of the animations

    private void Start()
    {
        PlayAnimation(AnimationState.NotMoving, RotationState.NotRotating);
    }

    private void PlayAnimation(AnimationState animState, RotationState rotState)
{
    // Stop all animations
    LeanTween.cancel(gameObject);

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
