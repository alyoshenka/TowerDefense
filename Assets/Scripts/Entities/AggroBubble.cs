using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// aggro sphere trigger collider
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AggroBubble : MonoBehaviour
{
    public delegate void AgentTriggerBubble();
    public event AgentTriggerBubble agentEnter; // agent enters trigger
    public event AgentTriggerBubble agentExit;  // agent exits trigger

    // ToDo: make regular collider and allow inheritance
    public SphereCollider me; // collider to trigger action

    private List<HostileAgent> withinRange; // agents that are within range of action
    // get first agent in range (queue-style)
    public HostileAgent RequestTarget { get => withinRange.Count > 0 ? withinRange[0] : null; }

    private void Awake()
    {
        withinRange = new List<HostileAgent>();
    }

    /// <summary>
    /// set values
    /// </summary>
    /// <param name="rad">aggro radius</param>
    public void Initialize(float rad)
    {
        me.isTrigger = true;
        me.radius = rad;

        // BAD
        DefendState.Instance.openDefend += (() => { me.enabled = true; });
        me.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        HostileAgent ag = other.GetComponentInParent<HostileAgent>();
        if(null != ag) { AddAgent(ag); }        
    }

    public virtual void OnTriggerExit(Collider other)
    {
        HostileAgent ag = other.GetComponentInParent<HostileAgent>();
        if(null != ag) { RemoveAgent(ag); }
    }

    /// <summary>
    /// add a new agent to the list in range
    /// </summary>
    /// <param name="agent">the new agent to add</param>
    public void AddAgent(HostileAgent agent)
    {
        withinRange.Add(agent); // fires in order (not distance)
        agent.AddAttacker(this);

        agentEnter?.Invoke();

        if (Debugger.Instance.AggroTriggers) 
        {
            Debug.Log(agent.name + " entered " + name + "'s aggro range"); 
        }
    }

    /// <summary>
    /// remove an agent from the list in range
    /// </summary>
    /// <param name="agent">the agent to remove</param>
    public void RemoveAgent(HostileAgent agent) 
    { 
        withinRange.Remove(agent); 
        // agent.RemoveAttacker(this); // causes snum interation error

        agentExit?.Invoke();

        if (Debugger.Instance.AggroTriggers) 
        { 
            Debug.Log(agent.name + " exited " + name + "'s aggro range"); 
        }

        if(withinRange.Count == 0) { Debug.Log("no more enemies in range"); }
    }
}


