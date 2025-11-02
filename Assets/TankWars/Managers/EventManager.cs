using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // --- GAME EVENTS ---
    public static event Action<GameState> OnGameStateChanged;
    public static void TriggerGameStateChanged(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    // --- TIMER EVENTS ---
    public static event Action<int> OnCountdownUpdated;
    public static event Action OnCountdownComplete;
    public static event Action OnCountdownStopped;

    public static void TriggerCountdownUpdated(int time)
    {
        OnCountdownUpdated?.Invoke(time);
    }

    public static void TriggerCountdownComplete()
    {
        OnCountdownComplete?.Invoke();
    }

    public static void TriggerCountdownStopped()
    {
        OnCountdownStopped?.Invoke();
    }

    // --- PLAYER EVENTS ---
    public static event Action<Player> OnPlayerAdded;
    public static event Action<Player> OnPlayerRemoved;
    public static event Action<Player, bool> OnPlayerReadyToggle;
    public static event Action<Player, float> OnPlayerRespawnTimerStart;
    public static event Action<Player> OnPlayerRespawned;
    public static event Action<Player> OnPlayerEliminated;

    public static void TriggerPlayerAdded(Player player)
    {
        OnPlayerAdded?.Invoke(player);
    }

    public static void TriggerPlayerRemoved(Player player)
    {
        OnPlayerRemoved?.Invoke(player);
    }

    public static void TriggerPlayerReadyToggle(Player player, bool isReady)
    {
        OnPlayerReadyToggle?.Invoke(player, isReady);
    }

    public static void TriggerPlayerRespawnTimerStart(Player player, float respawnTime)
    {
        OnPlayerRespawnTimerStart?.Invoke(player, respawnTime);
    }

    public static void TriggerPlayerRespawned(Player player)
    {
        OnPlayerRespawned?.Invoke(player);
    }

    public static void TriggerPlayerEliminated(Player player)
    {
        OnPlayerEliminated?.Invoke(player);
    }

    // --- HEALTH EVENTS ---
    public static event Action<Player, GameObject, float> OnDamageTaken;
    public static event Action<Player, GameObject, float> OnHealingTaken;
    public static event Action<Player, float, float> OnHealthChanged;
    public static event Action<Player, GameObject> OnPlayerDeath;

    public static void TriggerDamageTaken(Player player, GameObject damageDealer, float damage)
    {
        OnDamageTaken?.Invoke(player, damageDealer, damage);
    }

    public static void TriggerHealingTaken(Player player, GameObject healer, float healing)
    {
        OnHealingTaken?.Invoke(player, healer, healing);
    }

    public static void TriggerHealthChanged(Player player, float health, float maxHealth)
    {
        OnHealthChanged?.Invoke(player, health, maxHealth);
    }

    public static void TriggerPlayerDeath(Player player, GameObject killer)
    {
        OnPlayerDeath?.Invoke(player, killer);
    }

    // --- LIVES EVENTS ---
    public static event Action<Player, int, int> OnLivesChanged;
    public static event Action<Player, int, int> OnRebornsChanged;

    public static void TriggerLivesChanged(Player player, int lives, int maxLives)
    {
        OnLivesChanged?.Invoke(player, lives, maxLives);
    }

    public static void TriggerRebornsChanged(Player player, int reborns, int maxReborns)
    {
        OnRebornsChanged?.Invoke(player, reborns, maxReborns);
    }

    // --- ABILITY EVENTS ---
    public static event Action<Player, Ability> OnAbilityQueued;
    public static event Action<Player, Ability> OnAbilityDequeued;
    public static event Action<Player, Ability> OnAbilityActivated;
    public static event Action<Player, Ability> OnAbilityDeactivated;
    public static event Action<Player, Ability> OnCurrentAbilityChanged;

    public static void TriggerAbilityQueued(Player player, Ability ability)
    {
        OnAbilityQueued?.Invoke(player, ability);
    }

    public static void TriggerAbilityDequeued(Player player, Ability ability)
    {
        OnAbilityDequeued?.Invoke(player, ability);
    }

    public static void TriggerAbilityActivated(Player player, Ability ability)
    {
        OnAbilityActivated?.Invoke(player, ability);
    }

    public static void TriggerAbilityDeactivated(Player player, Ability ability)
    {
        OnAbilityDeactivated?.Invoke(player, ability);
    }

    public static void TriggerCurrentAbilityChanged(Player player, Ability ability)
    {
        OnCurrentAbilityChanged?.Invoke(player, ability);
    }

    // --- STATUS EFFECT EVENTS ---
    public static event Action<Player, StatusEffect> OnStatusEffectAdded;
    public static event Action<Player, StatusEffect> OnStatusEffectRemoved;

    public static void TriggerStatusEffectAdded(Player player, StatusEffect statusEffect)
    {
        OnStatusEffectAdded?.Invoke(player, statusEffect);
    }

    public static void TriggerStatusEffectRemoved(Player player, StatusEffect statusEffect)
    {
        OnStatusEffectRemoved?.Invoke(player, statusEffect);
    }

    // --- KNOCKBACK EVENTS ---
    public static event Action<Player, GameObject, Vector3> OnKnockbackTaken;

    public static void TriggerKnockbackTaken(Player player, GameObject source, Vector3 knockback)
    {
        OnKnockbackTaken?.Invoke(player, source, knockback);
    }

    // --- MOVEMENT EVENTS ---
    public static event Action<Player, float, float> OnSpeedChanged;
    public static event Action<Player, float, float> OnRotateSpeedChanged;

    public static void TriggerSpeedChanged(Player player, float speed, float oldSpeed)
    {
        OnSpeedChanged?.Invoke(player, speed, oldSpeed);
    }

    public static void TriggerRotateSpeedChanged(Player player, float rotateSpeed, float oldRotateSpeed)
    {
        OnRotateSpeedChanged?.Invoke(player, rotateSpeed, oldRotateSpeed);
    }

    // --- PICKUP EVENTS ---
    public static event Action<Player, PickupableData> OnPickupCollected;

    public static void TriggerPickupCollected(Player player, PickupableData pickupData)
    {
        OnPickupCollected?.Invoke(player, pickupData);
    }

    // --- STATS EVENTS ---
    public static event Action<Dictionary<int, PlayerStats>> OnStatsChanged;
    public static event Action<Player, int> OnPlayerKillsChanged;

    public static void TriggerStatsChanged(Dictionary<int, PlayerStats> stats)
    {
        OnStatsChanged?.Invoke(stats);
    }

    public static void TriggerPlayerKillsChanged(Player player, int kills)
    {
        OnPlayerKillsChanged?.Invoke(player, kills);
    }
}