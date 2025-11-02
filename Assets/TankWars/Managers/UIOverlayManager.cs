// using System.Collections;
// using UnityEngine;
// using UnityEngine.UIElements;

// public class UIOverlayManager : Singleton<UIOverlayManager>
// {
//     // --- In-Game UI ---
//     VisualTreeAsset abilityUxml;
//     VisualTreeAsset statusEffectUxml;
//     VisualTreeAsset playerHeadUxml;

//     // --- Game Over UI ---
//     VisualTreeAsset playerStatsUxml;
//     VisualTreeAsset statUxml;
//     VisualElement gameOverUI;

//     // --- Player UI ---
//     VisualElement playerUI;

//     public void HideGamePlayUI()
//     {
//         playerUI.style.display = DisplayStyle.None;
//     }

//     public void ShowGamePlayUI()
//     {
//         playerUI.style.display = DisplayStyle.Flex;
//     }

//     public void HideGameOverUI()
//     {
//         gameOverUI.style.display = DisplayStyle.None;
//     }

//     public void ShowGameOverUI()
//     {
//         gameOverUI.style.display = DisplayStyle.Flex;
//     }


//     // Unity event handlers for ability changes
//     protected override void Awake()
//     {
//         base.Awake();
//         // Load UI elements
//         playerHeadUxml = Resources.Load<VisualTreeAsset>("UI/PlayerHeadUI");
//         abilityUxml = Resources.Load<VisualTreeAsset>("UI/AbilityUI");
//         statusEffectUxml = Resources.Load<VisualTreeAsset>("UI/StatusEffectUI");

//         playerStatsUxml = Resources.Load<VisualTreeAsset>("UI/PlayerStatsUI");
//         statUxml = Resources.Load<VisualTreeAsset>("UI/StatUI");

//         // Get the root element and playerUI
//         var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
//         playerUI = root.Q<VisualElement>("PlayerUI");
//         gameOverUI = root.Q<VisualElement>("GameOverUI");

//         // Subscribe to PlayerManager events
//         PlayerManager.Instance.OnPlayerAdded += AddPlayer;
//         PlayerManager.Instance.OnPlayerRemoved += RemovePlayer;
//     }

//     private VisualElement getPlayerWithID(int playerID)
//     {
//         return playerUI.Q<VisualElement>("PlayerHead_" + playerID.ToString());
//     }

//     public void AddStatusEffect(Player player, StatusEffect statusEffect) {
//         var playerID = player.playerID;
//         Debug.Log("Adding status effect UI " + statusEffect.name + " to player " + playerID);
//         Debug.Log("statusEffectUxml: " + statusEffectUxml);
//         var statusEffectUI = statusEffectUxml.Instantiate();
//         statusEffectUI.name = "StatusEffect_" + statusEffect.name;
//         // statusEffectUI.Q<Label>("StackNumber").text = "";
//         statusEffectUI.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(statusEffect.icon);
//         getPlayerWithID(playerID).Q<VisualElement>("StatusEffects").Add(statusEffectUI);
//     }

//     public void RemoveStatusEffect(Player player, StatusEffect statusEffect) {
//         var playerID = player.playerID;
//         getPlayerWithID(playerID).Q<VisualElement>("StatusEffects").Q<VisualElement>("StatusEffect_" + statusEffect.name).RemoveFromHierarchy();
//     }

//     // public void UpdateStatusEffectStack(int playerID, StatusEffect statusEffect, int stack) {
//     //     var statusEffectUI = getPlayerWithID(playerID).Q<VisualElement>("StatusEffects").Q<VisualElement>("StatusEffect_" + statusEffect.name);
//     //     statusEffectUI.Q<Label>("StackNumber").text = stack.ToString();
//     // }

//     public void AddAbility(Player player, Ability ability)
//     {
//         var playerID = player.playerID;
//         var abilityUI = abilityUxml.Instantiate();
//         abilityUI.name = "Ability_" + ability.name;
//         abilityUI.Q<Label>().text = ability.name;
//         abilityUI.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(ability.abilityIcon);
//         getPlayerWithID(playerID).Q<VisualElement>("Abilities").Add(abilityUI);
//     }

//     public void TriggerAbility(Player player, Ability ability)
//     {
//         var playerID = player.playerID;
//         var abilityUI = getPlayerWithID(playerID).Q<VisualElement>("Abilities").Q<VisualElement>("Ability_" + ability.name);
//         var abilityDuration = abilityUI.Q<VisualElement>("AbilityDuration");
//         abilityDuration.style.display = DisplayStyle.Flex;
//         StartCoroutine(DecreaseAbilityDurationUI(abilityDuration, ability.activeTime));

//     }

//     // Enumerate to decrease the opacity of the ability icon based on the ability duration
//     IEnumerator DecreaseAbilityDurationUI(VisualElement abilityDurationUI, float duration) {
//         float elapsedTime = 0f;
//         while (elapsedTime < duration) {
//             abilityDurationUI.style.height = Length.Percent((1 - elapsedTime / duration) * 100f);
//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }
//     }

//     public void RemoveAbility(Player player, Ability ability)
//     {
//         var playerID = player.playerID;
//         getPlayerWithID(playerID).Q<VisualElement>("Abilities").Q<VisualElement>("Ability_" + ability.name).RemoveFromHierarchy();
//     }

//     public void AddPlayer(Player player)
//     {
//         // Add the player head
//         var playerHead = playerHeadUxml.Instantiate();
//         playerHead.name = "PlayerHead_" + player.playerID.ToString();
//         playerHead.Q<Label>().text = $"{player.playerData.playerName} ({player.playerID})";
//         playerUI.Add(playerHead);

//         var abilityCount = player.playerData.abilityQueueData.abilityCount;
//         // Set the length of abilities bar
//         playerHead.Q<VisualElement>("AbilityBar").style.width = new StyleLength(abilityCount * 42 - 2);

//         // Add the player's RenderTexture icon
//         if (player.playerCameraRenderTexture != null)
//         {
//             playerHead.Q<VisualElement>("PlayerIcon").style.backgroundImage = Background.FromRenderTexture(player.playerCameraRenderTexture);
//         }

//         // Add the player's empty ability spots
//         for (int i = 0; i < abilityCount; i++)
//         {
//             var emptyAbility =  abilityUxml.Instantiate();
//             emptyAbility.name = "EmptyAbility";
//             emptyAbility.Q<Label>().text = "";
//             playerHead.Q<VisualElement>("EmptyAbilities").Add(emptyAbility);
//         }
//     }

//     public void RemovePlayer(Player player)
//     {
//         getPlayerWithID(player.playerID).RemoveFromHierarchy();
//     }

//     public void UpdatePlayerHealth(Player player, float health, float maxHealth)
//     {
//         var playerID = player.playerID;
//         var healthPercent = health / maxHealth * 100f;
//         var playerHead = getPlayerWithID(playerID);
//         playerHead.Q<VisualElement>("HealthRed").style.width = Length.Percent(healthPercent);
//         playerHead.Q<VisualElement>("HealthGreen").style.width = Length.Percent(healthPercent);
//         playerHead.Q<Label>("HealthNumber").text = health.ToString();
//     }

//     public void UpdatePlayerLivesCount(Player player, int lives) 
//     {
//         var playerID = player.playerID;
//         getPlayerWithID(playerID).Q<Label>("LivesNumber").text = lives.ToString();
//     }

//     public void UpdatePlayerKillCount(Player player, int kills) 
//     {
//         var playerID = player.playerID;
//         getPlayerWithID(playerID).Q<Label>("KillsNumber").text = kills.ToString();
//     }

//     public void UpdatePlayerZeroLives(Player player) {
//         var playerID = player.playerID;
//         getPlayerWithID(playerID).style.opacity = 0.5f;
//     }

//     public void AddPlayerToGameOverUI(Player player) {
//         Debug.Log("Adding player " + player.playerID + " to game over UI");
//         var playerStats = StatsManager.Instance.GetPlayerStats(player.playerID);
//         var playerStatsUI = playerStatsUxml.Instantiate();
//         playerStatsUI.name = "PlayerStats_" + player.playerID.ToString();
//         playerStatsUI.Q<VisualElement>("PlayerImage").style.backgroundImage = Background.FromRenderTexture(player.playerCameraRenderTexture);
//         playerStatsUI.Q<Label>("PlayerRank").text = playerStats.rank.ToString();
//         playerStatsUI.Q<Label>("PlayerName").text = $"{player.playerData.playerName} ({player.playerID})";

//         foreach (var field in playerStats.GetType().GetFields())
//         {
//             var statUI = statUxml.Instantiate();
//             statUI.name = "Stat_" + field.Name;
//             statUI.Q<Label>("StatName").text = field.Name;
//             statUI.Q<Label>("StatValue").text = field.GetValue(playerStats).ToString();
//             playerStatsUI.Q<VisualElement>("StatsContainer").Add(statUI);
//         }
//         gameOverUI.Q<VisualElement>("StatsContainer").Add(playerStatsUI);
//     }
// }
