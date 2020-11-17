using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// shoots at targets
/// </summary>
public class BasicTurret : HostileAgent
{
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new BasicTurretBrain(this);

        // do better
        aggro.agentEnter += ((BasicTurretBrain)stateMachine).StopWaiting;
    }

    public override void AssignNextTarget()
    {
        target = aggro.RequestTarget?.gameObject; // design better
    }
}

/// <summary>
/// basic turrent "brain"
/// </summary>
public class BasicTurretBrain : DecisionTree
{
    BooleanDecision enemiesWithinRange; // there are enemies within range
    BooleanDecision hasTarget; // a target is assigned
    BooleanDecision lookingAtEnemy; // looking at target
    BooleanDecision canAttack; // able to attack target

    Action wait; // wait for interrupt (collision enter)
    Action reload; // are they the same?
    Action turn; // rotate
    Action attack; // attack target
    Action assignTarget; // assign new target

    bool waiting; // waiting to reset timer????

    /// <param name="agent">the agent to act upon</param>
    public BasicTurretBrain(BasicTurret agent)
    {
        reload = new Rest(agent);
        turn = new Turn(agent);
        attack = new Attack(agent); // override
        assignTarget = new AssignNextAttackTarget(agent);

        canAttack = new BooleanDecision(attack, reload);
        lookingAtEnemy = new BooleanDecision(canAttack, turn);
        hasTarget = new BooleanDecision(lookingAtEnemy, assignTarget);
        enemiesWithinRange = new BooleanDecision(hasTarget, reload);

        start = enemiesWithinRange;
    }

    /// <summary>
    /// wait for something(?)
    /// </summary>
    public void StartWaiting() { waiting = true; }
    /// <summary>
    /// stop waiting(?)
    /// </summary>
    public void StopWaiting() 
    { 
        waiting = false;
        if (Debugger.Instance.AggroTriggers) { Debug.Log("stopped waiting"); }
    }

    public override void Update(AIAgent _agent)
    {
        HostileAgent agent = (HostileAgent)_agent;

        enemiesWithinRange.Value = !waiting;
        lookingAtEnemy.Value = agent.LookingAtTarget;
        canAttack.Value = !agent.ShouldReload;
        hasTarget.Value = agent.HasTarget;
    }
}

/// <summary>
/// assign a new target to attack
/// </summary>
public class AssignNextAttackTarget : Action
{
    public AssignNextAttackTarget(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((BasicTurret)agent).AssignNextTarget();
        return null;
    }
}
