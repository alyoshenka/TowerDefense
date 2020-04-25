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

    protected Color brainDisplay = Color.black; // get rid of later

    public virtual void OnDrawGizmos()
    {
        if (Debugger.Instance != null && Debugger.Instance.AgentBrainMessages)
        {
            Gizmos.color = brainDisplay;
            Gizmos.DrawWireSphere(transform.position, 0.55f);
        }
    }
}


/// <summary>
/// an agent that can attack and die
/// </summary>
public abstract class HostileAgent : AIAgent, IDamageable
{
    // total allowable health
    [SerializeField] private int maxHealth = 10;
    // get maxHealth
    public int MaxHealth { get => maxHealth; }
    // current agent health
    [SerializeField] protected int currentHealth;
    // get currentHealth
    public int CurrentHealth { get => currentHealth; }
    // looking at target, true if no target assigned
    public bool LookingAtTarget { get => HasTarget ? Mathf.Abs(NeededRotationToTarget()) < targetLookEps : true; }
    // target assigned
    public bool HasTarget { get => null != target; }
    // get restedTime < reloadCooldown
    public bool ShouldReload { get => restedTime < reloadCooldown; }
    // current hostile target
    protected GameObject target;
    // time without moving
    protected float restedTime;
    // epsilon angle from looking at target (deg)
    private static float targetLookEps = 1;
    // index of terget
    protected int targetIdx;

    // turn in clockwise direction
    private int ccw;

    [Tooltip("Weapon to shoot")]
    public Projectile weapon;
    [Tooltip("Amount of points gained on death")]
    public int points;
    [Tooltip("Radius of aggro sphere")]
    public float attackRange;
    [Tooltip("aggro sphere")]
    public AggroBubble aggro;
    [Tooltip("Current health display image")]
    public UnityEngine.UI.Image healthBar;  
    [Tooltip("Rotation speed")]
    public float rotSpeed;
    [Tooltip("How long until I can attack again?")]
    public float reloadCooldown;


    private List<AggroBubble> attackers; // so i can destroy references

    protected virtual void Awake()
    {
        attackers = new List<AggroBubble>();
        ResetHealth();
        restedTime = reloadCooldown;

        aggro.Initialize(attackRange);
    }

    public void AddAttacker(AggroBubble bubble) { attackers.Add(bubble); }
    public void RemoveAttacker(AggroBubble bubble) { attackers.Remove(bubble); } // hopefully doesn't break stuff

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) { OnDeath(); }

        healthBar.fillAmount = 1.0f * currentHealth / maxHealth;
    }

    public virtual void OnDeath() 
    {
        foreach (AggroBubble bubble in attackers) { bubble.RemoveAgent(this); }
    }

    /// <summary>
    /// turn towards next target
    /// </summary>
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

    /// <summary>
    /// rotation in "game" space (deg), y is forward
    /// </summary>
    private float GameRot() { return ClampRot(transform.eulerAngles.z + 90); }

    /// <summary>
    /// how far to rotate to be looking at target (deg)
    /// </summary>
    private float NeededRotationToTarget()
    {
        if (null == target)
        {
            Debug.LogWarning(name + " has no target");
            return 0;
        }

        float ARot = GameRot();

        float yDif = target.transform.position.y - transform.position.y;
        float xDif = target.transform.position.x - transform.position.x;
        float ToBRot = Mathf.Atan2(yDif, xDif) * Mathf.Rad2Deg;

        float rotDif = ClampRot(ARot - ToBRot);

        return rotDif;
    }

    /// <summary>
    /// clamp degrees: -180 <= rot < 180
    /// </summary>
    private float ClampRot(float rot)
    {
        if(rot >= 180) { rot -= 360; }
        if(rot < -180) { rot += 360; }
        Debug.Assert(rot >= -180 && rot < 180); // check for really big or small numbers that didn't get clamped
        return rot;
    }

    /// <summary>
    /// increase restedTime
    /// </summary>
    public void Rest()
    {
        restedTime += Time.deltaTime;

        brainDisplay = Color.magenta;
    }

    public virtual void AssignNextTarget()
    {
        // abstract?
    }

    public virtual void Attack() 
    { 
        brainDisplay = Color.red; 

        Instantiate(weapon, transform.position, transform.rotation, transform);
        restedTime = 0;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        if(null == Debugger.Instance) { return; }

        if (Debugger.Instance.ShowAggroRanges)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        if(null == target) { return; }

        if (Debugger.Instance.AgentBrainMessages)
        {
            int len = 1;
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
   
}

/// <summary>
/// An agent that can move
/// </summary>
public abstract class OrganicAgent : HostileAgent
{
    protected FoundPath foundPath;       
    // allowable dist to target
    private static float targetEps = 0.1f;
    // has target and within allowable distance
    public bool ReachedTarget
    {
        get => HasTarget &&
            Vector3.Distance(transform.position, target.transform.position) < targetEps;
    }
    // get restedTime < moveCooldown
    public bool ShouldRest { get => restedTime < moveCooldown; }

    [Tooltip("Movement speed")]
    public float moveSpeed;
    [Tooltip("How long until I can move again?")]
    public float moveCooldown;


    protected override void Awake()
    {
        base.Awake();

        // set to max
        restedTime = reloadCooldown > moveCooldown ? reloadCooldown : moveCooldown;
    }
    
    /// <summary>
    /// move forwards
    /// </summary>
    public void Advance()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;

        brainDisplay = Color.blue;
    }

    public override void AssignNextTarget()
    {
        targetIdx++;
        restedTime = 0;

        if (targetIdx >= foundPath.path.Count)
        {
            target = null;

            gameObject.SetActive(false);
        }
        else
        {
            // bad
            target = PlaceState.Instance.Board.FindAssociatedTile(
            foundPath.path[targetIdx]).gameObject;
        }

        brainDisplay = Color.green;
    }

    public void AssignPath(FoundPath path)
    {
        foundPath = path;
        targetIdx = -1;
        AssignNextTarget();
    }    
}