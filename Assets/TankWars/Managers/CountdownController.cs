using System;
using UnityEngine;

public class CountdownController
{
    public event Action<int> OnCountdownUpdated;
    public event Action OnCountdownComplete;
    public event Action OnCountdownStopped;

    private float countdownTime;
    private bool countdownActive;
    private int lastCountdownSecond;

    public void Update()
    {
        if (countdownActive)
        {
            countdownTime -= Time.deltaTime;
            int currentSecond = Mathf.CeilToInt(countdownTime);
            if (currentSecond != lastCountdownSecond)
            {
                Debug.Log("Countdown: " + currentSecond);
                lastCountdownSecond = currentSecond;
                OnCountdownUpdated?.Invoke(currentSecond);
                EventManager.TriggerCountdownUpdated(currentSecond);
            }

            if (countdownTime <= 0)
            {
                Debug.Log("Countdown complete!");
                countdownActive = false;
                OnCountdownComplete?.Invoke();
                EventManager.TriggerCountdownComplete();
            }
        }
    }

    public void StartCountdown(float duration)
    {
        countdownTime = duration;
        countdownActive = true;
        lastCountdownSecond = Mathf.CeilToInt(countdownTime);
        OnCountdownUpdated?.Invoke(lastCountdownSecond); // Trigger initial countdown update
        EventManager.TriggerCountdownUpdated(lastCountdownSecond);
    }

    public void StopCountdown()
    {
        countdownActive = false;
        OnCountdownStopped?.Invoke();
        EventManager.TriggerCountdownStopped();
    }
}
