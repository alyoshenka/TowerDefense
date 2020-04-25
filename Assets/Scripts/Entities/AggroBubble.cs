using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AggroBubble : MonoBehaviour
{
    public delegate void AgentTriggerBubble();
    public event AgentTriggerBubble agentEnter;
    public event AgentTriggerBubble agentExit;

    public HostileAgent agent;
    public SphereCollider me; // make regular collider

    private List<HostileAgent> withinRange;
    public HostileAgent RequestTarget { get => withinRange.Count > 0 ? withinRange[0] : null; }

    private void Awake()
    {
        me.radius = agent.attackRange;
        me.isTrigger = true;

        withinRange = new List<HostileAgent>();
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

    public void RemoveAgent(HostileAgent agent) 
    { 
        withinRange.Remove(agent); 
        agent.RemoveAttacker(this);

        agentExit?.Invoke();

        if (Debugger.Instance.AggroTriggers) 
        { 
            Debug.Log(agent.name + " exited " + name + "'s aggro range"); 
        }

        if(withinRange.Count == 0) { Debug.Log("no more enemies in range"); }
    }
}


