using UnityEngine;

public class TankAudioSystem : MonoBehaviour
{
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private float baseVolume = 0.6f;
    [SerializeField] private float timeToReachConstantSpeed = 0.5f;
    [SerializeField] private float overshootFactor = 1.1f;
    [SerializeField] private float overshootDuration = 0.1f;
    
    private AudioSource engineAudioSource;
    private TankAudioState currentState = TankAudioState.Still;
    private float timeSinceStateChange = 0.0f;
    private float targetPitch = 1f;
    private float targetVolume = 1f;

    public enum TankAudioState
    {
        Still,
        Accelerating,
        Moving,
        Rotating,
        Decelerating
    }

    void Start()
    {
        engineAudioSource = gameObject.AddComponent<AudioSource>();
        engineAudioSource.clip = engineClip;
        engineAudioSource.loop = true;
        engineAudioSource.Play();
    }

    void Update()
    {
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 2f);
        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, targetVolume * baseVolume, Time.deltaTime * 2f);
    }

    public void HandleMovementAudio(float transAmount, float rotateAmount)
    {
        TankAudioState newState = DetermineTankState(transAmount, rotateAmount);

        if (newState != currentState)
        {
            currentState = newState;
            timeSinceStateChange = 0f;
        }
        else
        {
            timeSinceStateChange += Time.fixedDeltaTime;
        }

        UpdateAudioForState(currentState);
    }

    private TankAudioState DetermineTankState(float transAmount, float rotateAmount)
    {
        if (transAmount == 0 && rotateAmount != 0) return TankAudioState.Rotating;
        if (transAmount == 0 && rotateAmount == 0) return currentState == TankAudioState.Moving ? TankAudioState.Decelerating : TankAudioState.Still;
        if (currentState == TankAudioState.Still || currentState == TankAudioState.Decelerating) return TankAudioState.Accelerating;
        return timeSinceStateChange >= timeToReachConstantSpeed ? TankAudioState.Moving : currentState;
    }

    private void UpdateAudioForState(TankAudioState state)
    {
        switch (state)
        {
            case TankAudioState.Still:
                targetPitch = 0.6f;
                targetVolume = 0.5f;
                break;
                
            case TankAudioState.Rotating:
                targetPitch = 0.8f;
                targetVolume = 0.8f;
                break;

            case TankAudioState.Accelerating:
                if (timeSinceStateChange < timeToReachConstantSpeed)
                {
                    float lerpFactor = timeSinceStateChange / timeToReachConstantSpeed;

                    // Overshoot logic
                    if (timeSinceStateChange < overshootDuration)
                    {
                        targetPitch = 1f + (overshootFactor - 1f) * (1f - lerpFactor);
                    }
                    else
                    {
                        targetPitch = 1f;
                    }
                    
                    targetVolume = 0.5f + 0.5f * lerpFactor;
                }
                break;

            case TankAudioState.Decelerating:
                // ... similar to Accelerating but in reverse
                break;

            case TankAudioState.Moving:
                targetPitch = 1f;
                targetVolume = 1f;
                break;
        }
    }
}
