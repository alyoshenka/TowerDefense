using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Interfaces

/// <summary>
/// a choice to be made
/// </summary>
public interface IDecision
{
    /// <summary>
    /// choose action, given preset conditions
    /// </summary>
    /// <returns>outcome of decision</returns>
    IDecision MakeDecision();
}

#endregion

#region Decisions

/// <summary>
/// decision with two outcomes
/// </summary>
public class BooleanDecision : IDecision
{
    IDecision trueBranch;   // decision if condition is true
    IDecision falseBranch;  // decision if condition is false
  
    public bool Value { set; get; } // decision conditional

    /// <param name="_trueBranch">decision if condition</param>
    /// <param name="_falseBranch">decision if not condition</param>
    public BooleanDecision(IDecision _trueBranch, IDecision _falseBranch)
    {
        Debug.Assert(null != _trueBranch && null != _falseBranch);
        trueBranch = _trueBranch;
        falseBranch = _falseBranch;
    }

    public IDecision MakeDecision()
    {
        return Value ? trueBranch.MakeDecision() : falseBranch.MakeDecision();
    }
}

#endregion

#region Actions

/// <summary>
/// resulting effect of decision
/// </summary>
public abstract class Action : IDecision
{
    public AIAgent agent; // agent to act upon
    public Transform Target { get; set; } // current object of interest
    private Action() { }

    /// <param name="agent">the agent that will be acted upon</param>
    public Action(AIAgent agent) { this.agent = agent; }
    public virtual IDecision MakeDecision() { if (Debugger.Instance.AgentBrainMessages) { Debug.Log(GetType()); } return null; } // make abstract
}

/// <summary>
/// attack intruders
/// </summary>
public class Defend : Action
{
    public Defend(BasicEnemy agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

/// <summary>
/// attack target
/// </summary>
public class Attack : Action
{
    public Attack(AIAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        ((HostileAgent)agent).Attack();
        return null;
    }
}

/// <summary>
/// breakdown the target structure
/// </summary>
public class BreakWall : Action
{
    public BreakWall(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

/// <summary>
/// move forward
/// </summary>
public class Advance : Action
{
    public Advance(OrganicAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        ((OrganicAgent)agent).Advance();
        return null;
    }
}

/// <summary>
/// stop and recharge movement 
/// </summary>
public class Rest : Action
{
    public Rest(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        ((HostileAgent)agent).Rest();
        return null;
    }
}

/// <summary>
/// repair a broken structure
/// </summary>
public class Rebuild : Action
{
    public Rebuild(AIAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

/// <summary>
/// change direction agent is facing
/// </summary>
public class Turn : Action
{
    public Turn(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        ((HostileAgent)agent).Turn();
        return null;
    }
}

#endregion
