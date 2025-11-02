using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class CountdownUI : UIController
{
    private VisualElement countdownUI;
    private Label countdownLabel;

    void Awake()
    {
        var uidoc = GetComponent<UIDocument>();
        countdownUI = uidoc.rootVisualElement.Q<VisualElement>("CountdownContainer");
        Hide();

        countdownLabel = countdownUI.Q<Label>("CountdownTimerLabel");

        // Subscribe to relevant events
        EventManager.OnCountdownUpdated += OnCountdownUpdated;
        EventManager.OnCountdownComplete += Hide;
        EventManager.OnCountdownStopped += Hide;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.OnCountdownUpdated -= OnCountdownUpdated;
        EventManager.OnCountdownComplete -= Hide;
        EventManager.OnCountdownStopped -= Hide;
    }

    private void OnCountdownUpdated(int countdown)
    {
        // Show countdown timer UI
        if (countdownUI.style.display == DisplayStyle.None)
        {
            Show();
        }

        // Update countdown timer UI
        countdownLabel.text = countdown.ToString();

        // Start bounce animation
        StopAllCoroutines(); // Stop any existing animations
        StartCoroutine(BounceAnimation(countdownLabel));
    }

    private IEnumerator BounceAnimation(VisualElement element)
    {
        float animationTime = 0.3f; // Duration of the animation
        float scaleMax = 1.2f; // Maximum scale
        float scaleMin = 1f; // Minimum scale
        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float percentComplete = elapsedTime / animationTime;
            float scale = Mathf.Lerp(scaleMax, scaleMin, percentComplete);

            element.transform.scale = new Vector3(scale, scale, 1);
            yield return null;
        }

        element.transform.scale = new Vector3(1, 1, 1); // Reset scale
    }

    public override void Hide()
    {
        countdownUI.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        countdownUI.style.display = DisplayStyle.Flex;
    }
}
