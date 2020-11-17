using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// separate roles between player and goaltile/castle

/// <summary>
/// manages player state
/// </summary>
[System.Serializable]
public class Player : IDamageable
{
    [SerializeField] [Tooltip("money balance")] private int money;    
    [Tooltip("maximum health")] public int maxHealth;
    [SerializeField] [Tooltip("current health")] private int currentHealth;

    public int Gold { get { return money; } private set { } } // get money
    public int CurrentHealth { get { return currentHealth; } private set { } } // get current health

    public Player()
    {
        money = 0;
        ResetHealth();
    }

    /// <summary>
    /// increase the players money amount
    /// </summary>
    /// <param name="gold">amount to increase</param>
    public void GiveGold(int gold)
    {
        Debug.Assert(gold > 0); 
        // max gold??

        money += gold;
        Debug.Log("got " + gold + " gold");
    }

    /// <summary>
    /// decrease the players money amount
    /// </summary>
    /// <param name="gold">amount to take</param>
    /// <param name="requireFunds">require balance to be sufficient</param>
    /// <returns>whether amount could be taken</returns>
    public bool TakeGold(int gold, bool requireFunds)
    {
        // could be rewritten but this is okay

        Debug.Assert(gold > 0);

        if (requireFunds)
        {
            if (money - gold < 0)
            {
                Debug.LogError("not enough gold condition");
                return false;
            }
            else
            {
                money -= gold;
                Debug.Log("lost " + gold + " gold");
                return true;
            }
        }
        else
        {
            int lost = money < gold ? money : gold;
            money -= lost;
            Debug.Log("lost " + gold + " gold");
            if (money == 0) { Debug.Log("no more gold"); }
            Debug.Assert(money >= 0);
            return true;
        }

    }

    public void ResetHealth() { currentHealth = maxHealth; }

    public void ApplyDamage(int damage)
    {
        Debug.Assert(damage >= 0);
        Debug.LogWarning("player damage thing");
        maxHealth -= damage;
        if(currentHealth <= 0) { OnDeath(); }
    }

    public void OnDeath()
    {
        throw new System.NotImplementedException();
        // gameover
    }

    /// <summary>
    /// save player data and destroy references
    /// </summary>
    public void SaveAndDestroy()
    {
        Debug.Log("save player data and destroy references");
    }
}
