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
        base.Attack(); // take out later

        if (ShouldRest) { Rest(); }
        else
        {
            Instantiate(weapon, transform.position, Quaternion.Euler(transform.up), transform);
            restedTime = 0;
        }
    }
}

public class BasicEnemyBrain : DecisionTree
{
    BooleanDecision isTired_move;
    BooleanDecision isTired_reload;
    BooleanDecision isTired_turn;
    BooleanDecision atTarget;
    BooleanDecision lookingAtTarget;
    BooleanDecision posedForGoal;
    BooleanDecision withinRange;

    Action turn;
    Action advance;
    Action rest;
    Action assignNextTarget;
    Action attack;

    public BasicEnemyBrain(BasicEnemy agent)
    {
        turn = new Turn(agent);
        advance = new Advance(agent);
        rest = new Rest(agent);
        assignNextTarget = new AssignNextTarget(agent);
        attack = new Attack(agent);

        lookingAtTarget = new BooleanDecision(advance, turn);
        isTired_move = new BooleanDecision(rest, lookingAtTarget);
        atTarget = new BooleanDecision(assignNextTarget, isTired_move);
        isTired_reload = new BooleanDecision(rest, attack);
        isTired_turn = new BooleanDecision(rest, turn);
        posedForGoal = new BooleanDecision(isTired_reload, isTired_turn);
        withinRange = new BooleanDecision(posedForGoal, atTarget);

        start = withinRange;
    }

    public override void Update(AIAgent agent)
    {
        isTired_move.Value = isTired_turn.Value = ((OrganicAgent)agent).ShouldRest;
        isTired_reload.Value = ((OrganicAgent)agent).ShouldReload;
        atTarget.Value = ((OrganicAgent)agent).ReachedTarget;
        withinRange.Value = ((HostileAgent)agent).withinRangeOfGoal;
        lookingAtTarget.Value = ((OrganicAgent)agent).LookingAtTarget;
        posedForGoal.Value = lookingAtTarget.Value && withinRange.Value;
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
