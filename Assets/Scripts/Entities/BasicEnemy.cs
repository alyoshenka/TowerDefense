using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Goal: Attack Base
[RequireComponent(typeof(BasicEnemyBubble))]
public class BasicEnemy : HostileAgent
{

    public bool yeet;

    protected new void Start()
    {
        base.Start();
       
        stateMachine = new BasicEnemyBrain(this);
    }

    protected override void Update()
    {
        stateMachine.Update(this);
        stateMachine.RunTree(this);

        if (yeet) { OnDeath(); }
    }

    public override void OnDeath()
    {
        GamePlayState.CurrentLevel.DestroyEnemy(this);
        Destroy(gameObject);
    }

    public override void Attack()
    {
        if (ShouldRest) { Rest(); }
        else
        {
            Instantiate(weapon, transform.position, transform.rotation, transform);
            restedTime = 0;
        }
    }
}

public class BasicEnemyBrain : DecisionTree
{
    BooleanDecision isTired;
    BooleanDecision atTarget;
    BooleanDecision withinRange;

    Action advance;
    Action rest;
    Action assignNextTarget;
    Action attack;

    public BasicEnemyBrain(BasicEnemy agent)
    {
        advance = new Advance(agent);
        rest = new Rest(agent);
        assignNextTarget = new AssignNextTarget(agent);
        attack = new Attack(agent);

        atTarget = new BooleanDecision(assignNextTarget, advance);
        withinRange = new BooleanDecision(attack, atTarget);
        isTired = new BooleanDecision(rest, withinRange);

        start = isTired;
    }

    public override void Update(AIAgent agent)
    {
        isTired.Value = ((OrganicAgent)agent).ShouldRest;
        atTarget.Value = ((OrganicAgent)agent).ReachedTarget;
        withinRange.Value = ((HostileAgent)agent).withinRangeOfGoal;
    }
}

public class AssignNextTarget : Action
{
    public AssignNextTarget(OrganicAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((OrganicAgent)agent).AssignNextTarget();
        return null;
    }
}
