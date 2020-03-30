using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity that can make decisions
/// </summary>
public abstract class AIAgent : MonoBehaviour
{
    protected virtual void Update()
    {
        stateMachine.Update(this);
        stateMachine.RunTree(this);
    }

    public DecisionTree stateMachine;
}

/// <summary>
/// An agent that can move and die
/// </summary>
public abstract class OrganicAgent : AIAgent, IDamageable // every agent should be damageable! (maybe???)
{
    protected FoundPath foundPath;
    protected int targetIdx;

    [SerializeField] private int maxHealth = 10;
    public int MaxHealth { get => maxHealth; }

    [SerializeField] protected int currentHealth;

    public int CurrentHealth { get => currentHealth; }
    public static float targetEps = 0.1f;
    protected MapTile target;
    public bool ReachedTarget
    {
        get => null == target ||
            Vector3.Distance(transform.position, target.transform.position) < targetEps;
    }
    

    [Tooltip("How long until it can move again")]
    public float moveCooldown;
    protected float restedTime;
    public bool ShouldRest { get => restedTime < moveCooldown; }

    // public float attackCooldown;

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

    public void AssignNextTarget()
    {
        targetIdx++;
        restedTime = 0;

        if (targetIdx >= foundPath.path.Count)
        {
            target = null;
            Debug.Log(name + " reached goal");
            gameObject.SetActive(false);
        }
        else
        {
            target = PlaceState.Instance.Board.FindAssociatedTile(
            foundPath.path[targetIdx]);
        }
    }

    public void AssignPath(FoundPath path)
    {
        foundPath = path;
        targetIdx = -1;
        AssignNextTarget();
    }

    public abstract void Attack();
}

public abstract class HostileAgent : OrganicAgent
{
    public Projectile weapon;
    public int points;
    public float attackRange;

    public bool withinRangeOfGoal;

    public override abstract void OnDeath();
   
}

public abstract class FriendlyAgent : OrganicAgent
{
    public override abstract void OnDeath();
}