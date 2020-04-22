using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : HostileAgent
{
    protected new BasicTurretBrain stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new BasicTurretBrain(this);

        // do better
        GetComponent<AggroBubble>().agentEnter += stateMachine.StopWaiting;
    }
}

public class BasicTurretBrain : DecisionTree
{
    BooleanDecision enemiesWithinRange;
    BooleanDecision lookingAtEnemy;
    BooleanDecision canAttack;

    Action wait; // wait for interrupt (collision enter)
    Action reload; // are they the same?
    Action turn;
    Action attack;

    bool waiting;

    public BasicTurretBrain(BasicTurret agent)
    {
        reload = new Rest(agent);
        turn = new Turn(agent);
        attack = new Attack(agent); // override

        canAttack = new BooleanDecision(attack, reload);
        lookingAtEnemy = new BooleanDecision(canAttack, turn);
        enemiesWithinRange = new BooleanDecision(lookingAtEnemy, reload);

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
    }
}
