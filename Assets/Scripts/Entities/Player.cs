using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// separate roles between player and goaltile/castle

// manages the player values
[System.Serializable]
public class Player : IDamageable
{
    [SerializeField] private int money;
    public int Gold { get { return money; } private set { } }
    public int maxHealth;
    [SerializeField] private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } private set { } }

    public Player()
    {
        money = 0;
        ResetHealth();
    }

    public void GiveGold(int gold)
    {
        Debug.Assert(gold > 0); 
        // max gold??

        money += gold;
        Debug.Log("got " + gold + " gold");
    }

    // returns whether the gold could be taken
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

    public void SaveAndDestroy()
    {
        Debug.Log("save player data and destroy references");
    }
}
