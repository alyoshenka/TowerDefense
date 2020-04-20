using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionTree
{
    protected IDecision start; // starting decision
    private IDecision current;

    public abstract void Update(AIAgent agent); // update values


    public void RunTree(AIAgent agent)
    {
        if (PauseState.Instance.Paused) { return; }

        current = start;
        while (null != current) { current = current.MakeDecision(); }
    }
}

public class EnemyDecisionTree : DecisionTree
{
    // decisions
    BooleanDecision defenseInRange; // start
    BooleanDecision defenderAttacking;
    BooleanDecision wallInWay;

    // actions
    Defend defend;
    Attack attack;
    BreakWall breakWall;
    Advance advance;

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

public class DefenderDecisionTree : DecisionTree
{
    // decisions
    BooleanDecision tired;
    BooleanDecision enemyInRange;
    BooleanDecision wallBroken;

    // actions
    Rest rest;
    Attack attack;
    Rebuild rebuild;
    Advance patrol;

    public DefenderDecisionTree(BasicDefender agent)
    {
        patrol = new Advance(agent);
        rebuild = new Rebuild(agent);
        attack = new Attack(agent);
        rest = new Rest(agent);

        wallBroken = new BooleanDecision(rebuild, patrol);
        enemyInRange = new BooleanDecision(attack, wallBroken);
        tired = new BooleanDecision(rest, enemyInRange);

        start = tired;
    }

    public override void Update(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}