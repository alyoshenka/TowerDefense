using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// tile represeting enemy objective/player base
/// </summary>
public class GoalTile : MapTile, IDamageable
{
    private static GoalTile instance; // singleton instance
    public static bool LoseCon { get => instance.currentHealth <= 0; } // return if game is lost

    public delegate void GoalDeathEvent();
    public static event GoalDeathEvent goalDeath; // invoked upon goal death

    private TMP_Text castleHealth; // castle/player health display
    public TMP_Text CastleHealth 
    {
        get => castleHealth; 
        set
        {
            castleHealth = value;
            UpdateHealthDisplay();
        }
    }

    [Tooltip("max health")] public float maxHealth;
    [Tooltip("current health")] public float currentHealth;

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

    /// <summary>
    /// update display to reflect current health
    /// </summary>
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
