using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity that can make decisions
/// </summary>
public abstract class AIAgent : MonoBehaviour
{
    public DecisionTree stateMachine;
}

/// <summary>
/// An agent that can move and die
/// </summary>
public abstract class OrganicAgent : AIAgent, IDamageable
{
    public FoundPath path;

    [SerializeField] private int maxHealth = 10;
    public int MaxHealth { get { return maxHealth; } private set { } }

    [SerializeField] protected int currentHealth;

    public int CurrentHealth { get { return currentHealth; } protected set { } }
    public static float targetEps = 0.1f;
    protected MapTile target;
    public MapTile Target { get { return target; } set { target = value; } } // maybe encapsulate?
    public bool ReachedTarget
    {
        get => Vector3.Distance(transform.position, target.transform.position) < targetEps;
    }

    [Tooltip("How long until it can move again")]
    public float moveCooldown;
    protected float restedTime;
    public bool ShouldRest { get => restedTime < moveCooldown; }

    public float moveSpeed;

    protected void Start()
    {
        ResetHealth();

        restedTime = moveCooldown;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) { OnDeath(); }
    }

    public abstract void OnDeath();

    public void Advance()
    {
        transform.LookAt(target.transform);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    public void Rest()
    {
        restedTime += Time.deltaTime;
    }

    protected void Update()
    {
        if (null != target && ReachedTarget)
        {
            AssignNewTarget();
        }
    }


    public abstract void AssignNewTarget();
}

public abstract class HostileAgent : OrganicAgent
{
    [Tooltip("how long the spawner must wait before it can spawn")]
    [SerializeField] private int spawnRechargeTime;

    public override abstract void OnDeath();
   
}

public abstract class FriendlyAgent : OrganicAgent
{
    public override abstract void OnDeath();
}