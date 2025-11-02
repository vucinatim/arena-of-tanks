using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LivesData
{
    public int lives;
    public int reborns;
}

public class LivesSystem : MonoBehaviour
{
    private Player owner;
    [SerializeField] private LivesData data;

    private int lives;
    private int reborns;

    public event Action OnZeroLives;

    public void Initialize(Player owner, LivesData data)
    {
        this.owner = owner;
        this.data = data;

        SetLives(data.lives);
        reborns = data.reborns;
    }

    public void TotalReset()
    {
        SetLives(data.lives);
        reborns = data.reborns;
    }

    public void SetLives(int newLives)
    {
        lives = Math.Max(0, newLives);
        EventManager.TriggerLivesChanged(owner, lives, data.lives);
    }

    public bool TakeALife(GameObject damageDealer)
    {
        if (reborns > 0)
        {
            reborns -= 1;
            EventManager.TriggerRebornsChanged(owner, reborns, data.reborns);
            return true;
        }

        SetLives(lives - 1);
        FXManager.Instance.SpawnFX("PlayerDeath", transform.position, Quaternion.identity, null, 2f);
        if (lives <= 0)
        {
            OnZeroLives?.Invoke();
            EventManager.TriggerPlayerEliminated(owner);
            return false;
        }
        else
        {
            EventManager.TriggerPlayerRespawnTimerStart(owner, 3.0f);
        }

        return true;
    }

    public void AddALife()
    {
        SetLives(lives + 1);
    }

    public void AddReborn()
    {
        reborns += 1;
        EventManager.TriggerRebornsChanged(owner, reborns, data.reborns);
    }
}

