using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayersUI : UIController
{
    [SerializeField]
    private VisualTreeAsset playerHeadUxml;

    [SerializeField]
    private VisualTreeAsset abilityUxml;

    [SerializeField]
    private VisualTreeAsset statusEffectUxml;
    private VisualElement playerUI;

    void Awake()
    {
        var uidoc = GetComponent<UIDocument>();
        var root = uidoc.rootVisualElement;
        playerUI = root.Q<VisualElement>("PlayerUI");

        // Subscribe to relevant events
        EventManager.OnPlayerAdded += AddPlayer;
        EventManager.OnPlayerRemoved += RemovePlayer;
        EventManager.OnHealthChanged += UpdatePlayerHealth;
        EventManager.OnAbilityQueued += AddAbility;
        EventManager.OnAbilityActivated += TriggerAbility;
        EventManager.OnAbilityDequeued += RemoveAbility;
        EventManager.OnStatusEffectAdded += AddStatusEffect;
        EventManager.OnStatusEffectRemoved += RemoveStatusEffect;
        EventManager.OnLivesChanged += UpdatePlayerLivesCount;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.OnPlayerAdded -= AddPlayer;
        EventManager.OnPlayerRemoved -= RemovePlayer;
        EventManager.OnHealthChanged -= UpdatePlayerHealth;
        EventManager.OnAbilityQueued -= AddAbility;
        EventManager.OnAbilityActivated -= TriggerAbility;
        EventManager.OnAbilityDequeued -= RemoveAbility;
        EventManager.OnStatusEffectAdded -= AddStatusEffect;
        EventManager.OnStatusEffectRemoved -= RemoveStatusEffect;
        EventManager.OnLivesChanged -= UpdatePlayerLivesCount;
    }

    private VisualElement getPlayerWithID(int playerID)
    {
        return playerUI.Q<VisualElement>("PlayerHead_" + playerID.ToString());
    }

    private void AddStatusEffect(Player player, StatusEffect statusEffect)
    {
        var playerID = player.playerID;
        Debug.Log("Adding status effect UI " + statusEffect.name + " to player " + playerID);
        Debug.Log("statusEffectUxml: " + statusEffectUxml);
        var statusEffectUI = statusEffectUxml.Instantiate();
        statusEffectUI.name = "StatusEffect_" + statusEffect.name;
        statusEffectUI.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(
            statusEffect.icon
        );
        getPlayerWithID(playerID).Q<VisualElement>("StatusEffects").Add(statusEffectUI);
    }

    private void RemoveStatusEffect(Player player, StatusEffect statusEffect)
    {
        var playerID = player.playerID;
        getPlayerWithID(playerID)
            .Q<VisualElement>("StatusEffects")
            .Q<VisualElement>("StatusEffect_" + statusEffect.name)
            .RemoveFromHierarchy();
    }

    private void AddAbility(Player player, Ability ability)
    {
        var playerID = player.playerID;
        var abilityUI = abilityUxml.Instantiate();
        abilityUI.name = "Ability_" + ability.name;
        abilityUI.Q<Label>().text = ability.name;
        abilityUI.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(
            ability.abilityIcon
        );
        getPlayerWithID(playerID).Q<VisualElement>("Abilities").Add(abilityUI);
    }

    private void TriggerAbility(Player player, Ability ability)
    {
        var playerID = player.playerID;
        var abilityUI = getPlayerWithID(playerID)
            .Q<VisualElement>("Abilities")
            .Q<VisualElement>("Ability_" + ability.name);
        var abilityDuration = abilityUI.Q<VisualElement>("AbilityDuration");
        abilityDuration.style.display = DisplayStyle.Flex;
        StartCoroutine(DecreaseAbilityDurationUI(abilityDuration, ability.activeTime));
    }

    // Enumerate to decrease the opacity of the ability icon based on the ability duration
    IEnumerator DecreaseAbilityDurationUI(VisualElement abilityDurationUI, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            abilityDurationUI.style.height = Length.Percent((1 - elapsedTime / duration) * 100f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void RemoveAbility(Player player, Ability ability)
    {
        var playerID = player.playerID;
        getPlayerWithID(playerID)
            .Q<VisualElement>("Abilities")
            .Q<VisualElement>("Ability_" + ability.name)
            .RemoveFromHierarchy();
    }

    private void AddPlayer(Player player)
    {
        // Add the player head
        var playerHead = playerHeadUxml.Instantiate();
        playerHead.name = "PlayerHead_" + player.playerID.ToString();
        playerHead.Q<Label>().text = $"{player.playerData.playerName} ({player.playerID})";
        playerUI.Add(playerHead);

        var abilityCount = player.playerData.abilityQueueData.abilityCount;
        // Set the length of abilities bar
        playerHead.Q<VisualElement>("AbilityBar").style.width = new StyleLength(
            abilityCount * 42 - 2
        );

        // Add the player's RenderTexture icon
        if (player.playerCameraRenderTexture != null)
        {
            playerHead.Q<VisualElement>("PlayerIcon").style.backgroundImage =
                Background.FromRenderTexture(player.playerCameraRenderTexture);
        }

        // Add the player's empty ability spots
        for (int i = 0; i < abilityCount; i++)
        {
            var emptyAbility = abilityUxml.Instantiate();
            emptyAbility.name = "EmptyAbility";
            emptyAbility.Q<Label>().text = "";
            playerHead.Q<VisualElement>("EmptyAbilities").Add(emptyAbility);
        }
    }

    private void RemovePlayer(Player player)
    {
        getPlayerWithID(player.playerID).RemoveFromHierarchy();
    }

    private void UpdatePlayerHealth(Player player, float health, float maxHealth)
    {
        var playerID = player.playerID;
        var healthPercent = health / maxHealth * 100f;
        var playerHead = getPlayerWithID(playerID);
        playerHead.Q<VisualElement>("HealthRed").style.width = Length.Percent(healthPercent);
        playerHead.Q<VisualElement>("HealthGreen").style.width = Length.Percent(healthPercent);
        playerHead.Q<Label>("HealthNumber").text = health.ToString();
    }

    private void UpdatePlayerLivesCount(Player player, int lives, int maxLives)
    {
        var playerID = player.playerID;
        getPlayerWithID(playerID).Q<Label>("LivesNumber").text = lives.ToString();

        if (lives == 0)
        {
            getPlayerWithID(playerID).style.opacity = 0.5f;
        }
        else
        {
            getPlayerWithID(playerID).style.opacity = 1f;
        }
    }

    private void UpdatePlayerKillCount(Player player, int kills)
    {
        var playerID = player.playerID;
        getPlayerWithID(playerID).Q<Label>("KillsNumber").text = kills.ToString();
    }

    public override void Hide()
    {
        Debug.Log("Hiding player UI");
        playerUI.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        playerUI.style.display = DisplayStyle.Flex;
    }
}
