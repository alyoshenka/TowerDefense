using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Interfaces

public interface IDecision
{
    IDecision MakeDecision();
}

#endregion

#region Decisions

public class BooleanDecision : IDecision
{
    IDecision trueBranch;
    IDecision falseBranch;

    public bool Value { set; get; }

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

public abstract class Action : IDecision
{
    public AIAgent agent; // aiagent
    public Transform Target { get; set; }
    private Action() { }
    public Action(AIAgent agent) { this.agent = agent; }
    public virtual IDecision MakeDecision() { if (Debugger.Instance.AgentBrainMessages) { Debug.Log(GetType()); } return null; } // make abstract
}

public class Defend : Action
{
    public Defend(BasicEnemy agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

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

public class BreakWall : Action
{
    public BreakWall(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

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

public class Rebuild : Action
{
    public Rebuild(AIAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        return null;
    }
}

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
