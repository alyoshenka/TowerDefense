using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Goal: Attack Base
[RequireComponent(typeof(BasicEnemyBubble))]
public class BasicEnemy : OrganicAgent
{
    public bool withinRangeOfGoal;
    public bool yeet;

    protected new void Awake()
    {
        base.Awake();
       
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
        base.OnDeath();
        GamePlayState.CurrentLevel.DestroyEnemy(this);
        Destroy(gameObject);
    }

    public void SetTargetAsGoal()
    {
        withinRangeOfGoal = true;
        targetIdx = foundPath.path.Count - 1;
        target = PlaceState.Instance.Board.FindAssociatedTile(foundPath.path[targetIdx]).gameObject; // bad
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

    public override void Update(AIAgent _agent)
    {
        BasicEnemy agent = (BasicEnemy)_agent; // better?

        isTired_move.Value = isTired_turn.Value = agent.ShouldRest;
        isTired_reload.Value = agent.ShouldReload;
        atTarget.Value = agent.ReachedTarget;
        withinRange.Value = agent.withinRangeOfGoal;
        lookingAtTarget.Value = agent.LookingAtTarget;
        posedForGoal.Value = lookingAtTarget.Value && withinRange.Value;
    }
}

public class AssignNextTarget : Action
{
    public AssignNextTarget(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((OrganicAgent)agent).AssignNextTarget();
        return null;
    }
}
