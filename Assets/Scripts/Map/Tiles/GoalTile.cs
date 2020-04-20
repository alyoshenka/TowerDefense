using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalTile : MapTile, IDamageable
{
    private static GoalTile instance;
    public static bool LoseCon { get => instance.currentHealth <= 0; } // take out

    public delegate void GoalDeathEvent();
    public static event GoalDeathEvent goalDeath; // lose state

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
        if(null != instance) { Debug.LogWarning("instance = " + instance.name); }
        instance = this;

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
        if(null == goalDeath) { Debug.LogError("nothing happens on game over. nothing"); }
        if(this != instance) { Debug.LogError("goal tile instance != this dead goal tile"); }
        goalDeath.Invoke(); 
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
