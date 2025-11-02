using UnityEngine;
using System.Collections;

public class PosionCloud : MonoBehaviour
{
    public StatusEffect poisonStatusEffect; // Set this in Unity Editor
    public float duration = 10f;  // Total life time in seconds
    public float fadeDuration = 3f; // Last 3 seconds used for fading and shrinking
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(WaitForSeconds(duration - fadeDuration, BeginFadeAndShrink));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out StatusEffectsSystem statusEffectsSystem))
        {
            statusEffectsSystem.AddStatusEffect(gameObject, poisonStatusEffect);
        }
    }

    private IEnumerator WaitForSeconds(float seconds, System.Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    private void BeginFadeAndShrink()
    {
        StartCoroutine(ProgressiveAction(fadeDuration, (progress) =>
        {
            // Scale down
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
        },
        () =>
        {
            Destroy(gameObject);
        }));
    }

    private IEnumerator ProgressiveAction(float duration, System.Action<float> action, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            action(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        onComplete?.Invoke();
    }
}
