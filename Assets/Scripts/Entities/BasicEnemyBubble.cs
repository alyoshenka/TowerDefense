using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// aggro sphere for a basic enemy
/// </summary>
public class BasicEnemyBubble : AggroBubble
{
    public BasicEnemy agent; // agent who owns this aggro bubble

    private void Start()
    {
        me.radius = agent.attackRange;
        me.isTrigger = true;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Goal")) { agent.SetTargetAsGoal(); }
    }
}
