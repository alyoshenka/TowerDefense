using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity that can make decisions
/// </summary>
public abstract class AIAgent : MonoBehaviour, IRecyclable
{
    protected virtual void Update()
    {
        if(null == stateMachine) { Debug.LogError("null state machine"); return; }
        stateMachine.Update(this);
        stateMachine.RunTree(this);
    }

    public DecisionTree stateMachine; // brain

    // ToDo: get rid of/ revide debug strategy
    protected Color brainDisplay = Color.black; // gizmo display color

    public virtual void OnDrawGizmos()
    {
        if (Debugger.Instance != null && Debugger.Instance.AgentBrainMessages)
        {
            Gizmos.color = brainDisplay;
            Gizmos.DrawWireSphere(transform.position, 0.55f);
        }
    }

    public void Recycle()
    {
        throw new System.NotImplementedException();
    }
}


/// <summary>
/// an agent that can attack and die
/// </summary>
public abstract class HostileAgent : AIAgent, IDamageable
{
    public int MaxHealth { get => maxHealth; } // get maxHealth
    public int CurrentHealth { get => currentHealth; } // get currentHealth
    // looking at target, true if no target assigned
    public bool LookingAtTarget { get => HasTarget ? Mathf.Abs(NeededRotationToTarget()) < targetLookEps : true; }   
    public bool HasTarget { get => null != target; } // target assigned
    public bool ShouldReload { get => restedTime < reloadCooldown; } // get restedTime < reloadCooldown
    
    protected GameObject target; // current hostile target   
    protected float restedTime; // time without moving
    protected int targetIdx; // index of target

    private static float targetLookEps = 1; // epsilon angle from looking at target (deg)
    
    private int ccw; // turn in clockwise direction?

    [SerializeField] [Tooltip("total allowable health")] private int maxHealth = 10;
    [SerializeField] [Tooltip("current health value")] protected int currentHealth;

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

    // so i can destroy references
    private List<AggroBubble> attackers; // enemies within range

    protected virtual void Awake()
    {
        attackers = new List<AggroBubble>();
        ResetHealth();
        restedTime = reloadCooldown;

        aggro.Initialize(attackRange);
    }

    /// <summary>
    /// add a new attacker to the list
    /// </summary>
    /// <param name="bubble">the attackers assosciated aggro bubble</param>
    public void AddAttacker(AggroBubble bubble) { attackers.Add(bubble); }
    /// <summary>
    /// remove an attacker from the list
    /// </summary>
    /// <param name="bubble">the attackers assosciated aggro bubble</param>
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
        Destroy(gameObject); // something else must call this somewhere else
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

    /// <summary>
    /// get the next target and assign it to relevant fields
    /// </summary>
    public virtual void AssignNextTarget()
    {
        // abstract?
    }

    /// <summary>
    /// attack assigned target
    /// </summary>
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
    protected FoundPath foundPath; // path to goal     
    private static float targetEps = 0.1f; // allowable dist to target

    /// <summary>
    /// returns has target and within allowable distance
    /// </summary>
    public bool ReachedTarget
    {
        get => HasTarget &&
            Vector3.Distance(transform.position, target.transform.position) < targetEps;
    }
    
    public bool ShouldRest { get => restedTime < moveCooldown; } // get restedTime < moveCooldown

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
            // ToDo: wtf?
            /*
            target = PlaceState.Instance.Board.FindAssociatedTile(
            foundPath.path[targetIdx]).gameObject;
            */
        }

        brainDisplay = Color.green;
    }

    /// <summary>
    /// assign a new path
    /// </summary>
    /// <param name="path">the new path to assign</param>
    public void AssignPath(FoundPath path)
    {
        foundPath = path;
        targetIdx = -1;
        AssignNextTarget();
    }    
}