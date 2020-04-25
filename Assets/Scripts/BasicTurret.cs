using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : HostileAgent
{
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new BasicTurretBrain(this);

        // do better
        GetComponent<AggroBubble>().agentEnter += ((BasicTurretBrain)stateMachine).StopWaiting;
    }
}

public class BasicTurretBrain : DecisionTree
{
    BooleanDecision enemiesWithinRange;
    BooleanDecision hasTarget;
    BooleanDecision lookingAtEnemy;
    BooleanDecision canAttack;

    Action wait; // wait for interrupt (collision enter)
    Action reload; // are they the same?
    Action turn;
    Action attack;
    Action assignTarget;

    bool waiting;

    public BasicTurretBrain(BasicTurret agent)
    {
        reload = new Rest(agent);
        turn = new Turn(agent);
        attack = new Attack(agent); // override
        assignTarget = new AssignNextTarget(agent);

        canAttack = new BooleanDecision(attack, reload);
        lookingAtEnemy = new BooleanDecision(canAttack, turn);
        hasTarget = new BooleanDecision(lookingAtEnemy, assignTarget);
        enemiesWithinRange = new BooleanDecision(hasTarget, reload);

        start = enemiesWithinRange;
    }

    public void StartWaiting() { waiting = true; }
    public void StopWaiting() { waiting = false; }

    public override void Update(AIAgent _agent)
    {
        HostileAgent agent = (HostileAgent)_agent;

        enemiesWithinRange.Value = waiting;
        lookingAtEnemy.Value = agent.LookingAtTarget;
        canAttack.Value = !agent.ShouldReload;
        hasTarget.Value = agent.HasTarget;
    }
}
