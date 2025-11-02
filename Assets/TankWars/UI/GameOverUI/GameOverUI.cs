using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUI : UIController
{
    [SerializeField] private VisualTreeAsset playerStatsUxml;
    [SerializeField] private VisualTreeAsset statUxml;
    private VisualElement gameOverUI;

    void Awake()
    {
        var uidoc = GetComponent<UIDocument>();
        var root = uidoc.rootVisualElement;
        gameOverUI = root.Q<VisualElement>("GameOverUI");

        // Subscribe to relevant events
        EventManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.GameOver)
        {
            RenderPlayerStats();
        }
    }

    private void RenderPlayerStats()
    {
        // Clear the game over UI
        gameOverUI.Q<VisualElement>("Content").Clear();

        var stats = StatsManager.Instance.GetStats();
        // Iterate through each player and add their stats to the game over UI
        foreach (var playerStats in stats)
        {
            AddPlayerToGameOverUI(playerStats.Key, playerStats.Value);
        }

    }

    public void AddPlayerToGameOverUI(int playerID, PlayerStats playerStats)
    {
        var color = PlayerManager.Instance.players[playerID].color;
        Debug.Log("Adding player " + playerID + " to game over UI with color " + color);
        var playerStatsUI = playerStatsUxml.Instantiate();
        playerStatsUI.name = "PlayerStats_" + playerID.ToString();
        playerStatsUI.Q<VisualElement>("PlayerImage").style.backgroundColor = new StyleColor(color.WithAlpha(1f));
        playerStatsUI.Q<Label>("PlayerRank").text = playerStats.rank.ToString();
        playerStatsUI.Q<Label>("PlayerName").text = $"Player_{playerID}";

        foreach (var field in playerStats.GetType().GetFields())
        {
            var statUI = statUxml.Instantiate();
            statUI.name = "Stat_" + field.Name;
            statUI.Q<Label>("StatName").text = field.Name;
            statUI.Q<Label>("StatValue").text = field.GetValue(playerStats).ToString();
            playerStatsUI.Q<VisualElement>("StatsContainer").Add(statUI);
        }
        gameOverUI.Q<VisualElement>("Content").Add(playerStatsUI);
    }

    public override void Hide()
    {
        gameOverUI.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        gameOverUI.style.display = DisplayStyle.Flex;
    }
}
