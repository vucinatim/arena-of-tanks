using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverheadDisplaySystem : MonoBehaviour
{
    private Player owner;
    private GameObject namePlate;
    private GameObject currentAbilityIcon;
    private RectTransform overlayRectTransform;
    private Coroutine abilityDurationCoroutine;

    // Cached Components
    private TextMeshProUGUI namePlateText;
    private Image currentAbilityImage;

    public void Initialize(Player owner)
    {
        this.owner = owner;
        
        Transform canvas = transform.Find("Canvas");
        CacheComponents(canvas);

        SetNamePlateText($"Player {owner.playerID}");
    }

    private void CacheComponents(Transform canvas)
    {
        namePlate = canvas.Find("NamePlate").gameObject;
        namePlateText = namePlate.GetComponent<TextMeshProUGUI>();

        currentAbilityIcon = canvas.Find("CurrentAbilityIcon").gameObject;
        currentAbilityImage = currentAbilityIcon.GetComponent<Image>();
        
        overlayRectTransform = currentAbilityIcon.transform.Find("CurrentAbilityOverlay").GetComponent<RectTransform>();
    }

    public void ShowNamePlate(bool show) => namePlate.SetActive(show);

    public void ShowCurrentAbilityIcon(bool show) => currentAbilityIcon.SetActive(show);

    public void SetNamePlateText(string text) => namePlateText.SetText(text);

    public void SetCurrentAbilityIcon(Sprite sprite)
    {
        StopAbilityCoroutineIfNeeded();

        currentAbilityImage.enabled = sprite != null;
        currentAbilityImage.sprite = sprite;

        ResetOverlay();
    }

    private void StopAbilityCoroutineIfNeeded()
    {
        if (abilityDurationCoroutine != null)
        {
            StopCoroutine(abilityDurationCoroutine);
            abilityDurationCoroutine = null;
        }
    }

    private void ResetOverlay() => overlayRectTransform.sizeDelta = new Vector2(overlayRectTransform.sizeDelta.x, 0f);

    public void TriggerCurrentAbility(float duration)
    {
        if (duration <= 0f) return;

        overlayRectTransform.sizeDelta = new Vector2(overlayRectTransform.sizeDelta.x, 3f);
        abilityDurationCoroutine = StartCoroutine(DecreaseAbilityDurationUI(duration));
    }

    private IEnumerator DecreaseAbilityDurationUI(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            overlayRectTransform.sizeDelta = new Vector2(overlayRectTransform.sizeDelta.x, (1 - elapsedTime / duration) * 3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
