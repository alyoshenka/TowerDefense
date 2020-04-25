using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBubble : AggroBubble
{
    public BasicEnemy agent;

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
