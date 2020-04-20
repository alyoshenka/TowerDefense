using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBubble : AggroBubble
{

    private void Start()
    {
        me.radius = agent.attackRange;
        me.isTrigger = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Debug.Log(name);
            agent.withinRangeOfGoal = true;
        }
    }
}
