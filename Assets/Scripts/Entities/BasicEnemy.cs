using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// goal: attack base(castle) ONLY
/// </summary>
[RequireComponent(typeof(BasicEnemyBubble))]
public class BasicEnemy : OrganicAgent
{
    public bool withinRangeOfGoal; // close enough to goal to attack
    public bool yeet; // should be destroyed (just for debugging i think)

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

/// <summary>
/// decision structure for a basic enemy
/// </summary>
public class BasicEnemyBrain : DecisionTree
{
    #region Decisions
    BooleanDecision isTired_move;       // is tired from moving
    BooleanDecision isTired_reload;     // is tired from reloading
    BooleanDecision isTired_turn;       // is tired from turning
    BooleanDecision atTarget;           // very close to target
    BooleanDecision lookingAtTarget;    // facing terget
    BooleanDecision posedForGoal;       // ready to attack goal
    BooleanDecision withinRange;        // within range of target
    #endregion

    #region Actions
    Action turn;                // rotate
    Action advance;             // move forwards
    Action rest;                // refill rest counter
    Action assignNextTarget;    // get next target in list
    Action attack;              // attack target
    #endregion


    /// <param name="agent">the agent to act upon</param>
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

/// <summary>
/// assign the next target as the current target
/// </summary>
public class AssignNextTarget : Action
{
    public AssignNextTarget(HostileAgent agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((OrganicAgent)agent).AssignNextTarget();
        return null;
    }
}
