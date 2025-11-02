using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour
{
    public StatusEffect burnStatusEffect;  // Set this in Unity Editor
    public float duration = 5f;  // Total life time in seconds
    public float fadeDuration = 1f; // Last 1 second used for fading
    private Vector3 originalScale;

    private CollisionSystem collisionSystem;

    private void Start()
    {
        originalScale = transform.localScale;
        collisionSystem = transform.Find("Trigger").GetComponent<CollisionSystem>();
        collisionSystem.OnCollision += ApplyBurnEffect;
        StartCoroutine(WaitForSeconds(duration - fadeDuration, BeginFadeAndShrink));
    }

    private void ApplyBurnEffect(GameObject flame, GameObject collider)
    {
        if (collider.TryGetComponent(out StatusEffectsSystem statusEffectsSystem))
        {
            statusEffectsSystem.AddStatusEffect(null, burnStatusEffect);
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
            collisionSystem.OnCollision -= ApplyBurnEffect;
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
