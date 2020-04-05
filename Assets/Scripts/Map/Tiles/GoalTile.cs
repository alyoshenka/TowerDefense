using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTile : MapTile, IDamageable
{
    public float maxHealth;
    public float currentHealth;

    protected override void Awake()
    {
        base.Awake();
        ResetHealth();
    }

    public void ApplyDamage(int damage)
    {
        Debug.Assert(damage > 0);
        currentHealth -= damage;
        if(currentHealth <= 0) { OnDeath(); }
    }

    public void OnDeath()
    {
        GameStateManager.Instance.Transition(DefendState.Instance, GameOverState.Instance);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
