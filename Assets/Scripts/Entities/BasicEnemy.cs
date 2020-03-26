using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goal: Attack Base
/// </summary>
public class BasicEnemy : HostileAgent
{
    protected new void Start()
    {
        base.Start();
       
        stateMachine = new BasicEnemyDecisionTree(this);
    }

    protected new void Update()
    {
        base.Update();

         stateMachine.Update(this); // needed?
        // run state machine
        stateMachine.RunTree(this);
    }

    public override void AssignNewTarget()
    {
        /*
        target?.IndicateNone();
        int idx = path.path.IndexOf(target.Node) + 1;
        if (idx >= path.path.Count) { target = null; }
        else { target = path.path[idx].Tile; } // good fucking gawd
        target?.IndicateCurrent();

        restedTime = 0;
        */
        throw new System.NotImplementedException();
    }

    public override void OnDeath()
    {
        throw new System.NotImplementedException();
    }
}
