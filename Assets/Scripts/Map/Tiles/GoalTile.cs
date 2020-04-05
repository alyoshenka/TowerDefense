using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalTile : MapTile, IDamageable
{
    private TMP_Text castleHealth;
    public TMP_Text CastleHealth 
    {
        get => castleHealth; 
        set
        {
            castleHealth = value;
            UpdateHealthDisplay();
        }
    }

    public float maxHealth;
    public float currentHealth;

    protected override void Awake()
    {
        base.Awake();
        ResetHealth();
    }

    public void ApplyDamage(int damage)
    {
        // Debug.Assert(damage > 0);
        currentHealth -= damage;
        UpdateHealthDisplay();
        if(currentHealth <= 0) { OnDeath(); }
    }

    private void UpdateHealthDisplay()
    {
        castleHealth.text = currentHealth.ToString();
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
