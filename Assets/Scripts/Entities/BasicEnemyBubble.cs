using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBubble : AggroBubble
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            agent.withinRangeOfGoal = true;
        }
    }
}
