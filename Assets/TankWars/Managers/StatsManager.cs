using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{

    public int rank = 1;
    public int kills = 0;
    public int deaths = 0;
    public int damageDealt = 0;
    public int damageTaken = 0;
}

public class StatsManager : Singleton<StatsManager>
{
    private Dictionary<int, PlayerStats> stats = new Dictionary<int, PlayerStats>();

    public Dictionary<int, PlayerStats> GetStats()
    {
        return stats;
    }

    protected override void Awake()
    {
        InitializeStats();
        // Subscribe to PlayerManager events
        EventManager.OnPlayerAdded += AddPlayer;
        EventManager.OnPlayerRemoved += RemovePlayer;
        EventManager.OnPlayerDeath += PlayerDeath;
        EventManager.OnDamageTaken += PlayerDamageTaken;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.OnPlayerAdded -= AddPlayer;
        EventManager.OnPlayerRemoved -= RemovePlayer;
        EventManager.OnPlayerDeath -= PlayerDeath;
        EventManager.OnDamageTaken -= PlayerDamageTaken;
    }

    private void InitializeStats()
    {
        // Set all stats for each player to 0
        foreach (var player in PlayerManager.Instance.players.Values)
        {
            stats[player.playerID] = new PlayerStats();
        }
    }

    public void ResetStats()
    {
        // Reset all stats for each player to 0
        foreach (var player in PlayerManager.Instance.players.Values)
        {
            stats[player.playerID] = new PlayerStats();
        }
    }

    private void AddPlayer(Player player)
    {
        // Add player to stats
        stats[player.playerID] = new PlayerStats();
    }

    private void RemovePlayer(Player player)
    {
        // Remove player from stats
        stats.Remove(player.playerID);
    }

    public PlayerStats GetPlayerStats(int playerID)
    {
        return stats[playerID];
    }

    public void PlayerDamageTaken(Player player, GameObject damageDealer, float damage)
    {
        // Update damage taken for player
        stats[player.playerID].damageTaken += (int)damage;

        var damageDealerPlayer = damageDealer?.GetComponent<Player>();
        if (damageDealerPlayer != null)
        {
            // Update damage dealt for killer
            stats[damageDealerPlayer.playerID].damageDealt += (int)damage;
        }
    }

    public void PlayerDeath(Player player, GameObject killer)
    {
        // Update deaths for player
        stats[player.playerID].deaths++;

        var killerPlayer = killer?.GetComponent<Player>();
        if (killerPlayer != null)
        {
            // Update kills for killer
            stats[killerPlayer.playerID].kills++;
            EventManager.TriggerPlayerKillsChanged(killerPlayer, stats[killerPlayer.playerID].kills);
        }
    }

    public void SetPlayerRank(Player player, int rank)
    {
        // Update rank for player
        stats[player.playerID].rank = rank;
    }
}
