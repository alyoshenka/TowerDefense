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
        stateMachine?.Update(this);
        stateMachine?.RunTree(this);
    }

    public DecisionTree stateMachine;

    protected Color brainDisplay = Color.black; // get rid of later

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = brainDisplay;
        Gizmos.DrawWireSphere(transform.position, 0.55f);
    }
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

    public UnityEngine.UI.Image healthBar;   

    [Tooltip("How long until I can move again?")]
    public float moveCooldown;
    [Tooltip("How long until I can attack again?")]
    public float reloadCooldown;

    protected float restedTime;

    public bool ShouldRest { get => restedTime < moveCooldown; }
    public bool ShouldReload { get => restedTime < reloadCooldown; }

    // public float attackCooldown;

    public float moveSpeed;
    public float rotSpeed;

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

        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public abstract void OnDeath();

    public void Turn()
    {
        float ARot = transform.eulerAngles.z;
        float ToBRot = Mathf.Atan2(
            target.transform.position.y - transform.position.y, 
            target.transform.position.x - transform.position.x) 
            * Mathf.Rad2Deg;

        transform.Rotate(Vector3.forward * Time.deltaTime * rotSpeed);
        // bool cw;

        brainDisplay = Color.grey;
    }

    public void Advance()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;

        brainDisplay = Color.blue;
    }

    public void Rest()
    {
        restedTime += Time.deltaTime;

        brainDisplay = Color.magenta;
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

        brainDisplay = Color.green;
    }

    public void AssignPath(FoundPath path)
    {
        foundPath = path;
        targetIdx = -1;
        AssignNextTarget();
    }

    public virtual void Attack() { brainDisplay = Color.red; } // abstract

    public virtual bool LookingAtTarget()
    {
        float ARot = transform.eulerAngles.z;
        float ToBRot = Mathf.Atan2(
            target.transform.position.y - transform.position.y, 
            target.transform.position.x - transform.position.x) 
            * Mathf.Rad2Deg;
        return Mathf.Abs(ARot - ToBRot) < 1;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(null == target) { return; }

        int len = 2;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * len);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * len);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * len);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
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