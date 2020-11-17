using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// an AI decision tree
/// </summary>
public abstract class DecisionTree
{
    protected IDecision start; // starting decision
    private IDecision current; // current decision (placement in tree)

    /// <summary>
    /// update the values required for decision making
    /// </summary>
    /// <param name="agent">the agent to act upon</param>
    public abstract void Update(AIAgent agent); // update values

    /// <summary>
    /// make decision
    /// </summary>
    /// <param name="agent">the agent to act upon</param>
    public void RunTree(AIAgent agent)
    {
        if (PauseState.Instance.Paused) { return; }

        current = start;
        while (null != current) { current = current.MakeDecision(); }
    }
}

/// <summary>
/// base decision class for an enemy NPC
/// </summary>
public class EnemyDecisionTree : DecisionTree
{
    // conditions that need to be evaluated
    #region Decisions 
    // start
    BooleanDecision defenseInRange;     // there is a defender in attacking range
    BooleanDecision defenderAttacking;  // a defender is attacking them
    BooleanDecision wallInWay;          // there is a wall in front
    #endregion

    // end actions to take
    #region Actions
    Defend defend; // defend themself from attack
    Attack attack; // attack target
    BreakWall breakWall; // attack a wall
    Advance advance; // move towards next target
    #endregion

    /// <param name="agent">the agent to act upon</param>
    public EnemyDecisionTree(BasicEnemy agent)
    {
        advance = new Advance(agent);
        breakWall = new BreakWall(agent);
        attack = new Attack(agent);
        defend = new Defend(agent);

        wallInWay = new BooleanDecision(breakWall, advance);
        defenderAttacking = new BooleanDecision(defend, attack);
        defenseInRange = new BooleanDecision(defenderAttacking, wallInWay);

        start = defenseInRange;

        // subscribe update method to game events
    }

    public override void Update(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}