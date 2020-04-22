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

// todo: switch order of hostile and organic agents in class hierarchy

/// <summary>
/// An agent that can move and die
/// </summary>
public abstract class OrganicAgent : AIAgent, IDamageable
{
    protected FoundPath foundPath;
    protected int targetIdx;

    [SerializeField] private int maxHealth = 10;
    public int MaxHealth { get => maxHealth; }

    [SerializeField] protected int currentHealth;

    public int CurrentHealth { get => currentHealth; }

    protected MapTile target;
    public bool LookingAtTarget { get => Mathf.Abs(NeededRotationToTarget()) < targetLookEps; }
    public bool ReachedTarget
    {
        get => null == target ||
            Vector3.Distance(transform.position, target.transform.position) < targetEps;
    }   

    public UnityEngine.UI.Image healthBar;   

    [Tooltip("How long until I can move again?")]
    public float moveCooldown;

    protected float restedTime;

    public bool ShouldRest { get => restedTime < moveCooldown; }


    // public float attackCooldown;

    public float moveSpeed;
    public float rotSpeed;
    private int ccw; // turn in clockwise direction
    private static float targetEps = 0.1f;
    private static float targetLookEps = 1; // deg

    protected virtual void Start()
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
        float rotDif = NeededRotationToTarget();

        int newCCW = rotDif < 0 ? 1 : -1;
        if(newCCW != ccw && Mathf.Abs(rotDif) < targetLookEps) 
        { 
            Debug.LogWarning("overshot");

            float neededRot = ClampRot(rotDif - GameRot()); // amount to rotate back
            transform.Rotate(Vector3.forward * neededRot);
        }
        else { transform.Rotate(Vector3.forward * Time.deltaTime * rotSpeed * newCCW); }
        ccw = newCCW;

        brainDisplay = Color.grey;
    }

    // y is "forward"
    private float GameRot() { return ClampRot(transform.eulerAngles.z + 90); }

    // SIMPLIFY
    // deg, for now
    private float NeededRotationToTarget()
    {
        if (null == target)
        {
            Debug.LogWarning("no target");
            return 0;
        }

        float ARot = GameRot();

        float yDif = target.transform.position.y - transform.position.y;
        float xDif = target.transform.position.x - transform.position.x;
        float ToBRot = Mathf.Atan2(yDif, xDif) * Mathf.Rad2Deg;

        float rotDif = ClampRot(ARot - ToBRot);

        return rotDif;
    }

    // clamp degrees --180 <= rot < 180
    private float ClampRot(float rot)
    {
        if(rot >= 180) { rot -= 360; }
        if(rot < -180) { rot += 360; }
        Debug.Assert(rot >= -180 && rot < 180); // check for really big or small numbers that didn't get clamped
        return rot;
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
            // bad
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

    [Tooltip("How long until I can attack again?")]
    public float reloadCooldown;
    public bool ShouldReload { get => restedTime < reloadCooldown; }

    private List<AggroBubble> attackers; // so i can destroy references

    protected override void Start()
    {
        base.Start();
        attackers = new List<AggroBubble>();
    }

    public void AddAttacker(AggroBubble bubble) { attackers.Add(bubble); }
    public void RemoveAttacker(AggroBubble bubble) { attackers.Remove(bubble); } // hopefully doesn't break stuff


    public override void OnDeath() 
    {
        foreach (AggroBubble bubble in attackers) { bubble.RemoveAgent(this); }
    }
   
}