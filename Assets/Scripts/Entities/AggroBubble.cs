using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class AggroBubble : MonoBehaviour
{
    public HostileAgent agent;
    public SphereCollider me;
    private void Start()
    {
        me.radius = agent.attackRange;
        me.isTrigger = true;
    }

    protected abstract void OnTriggerEnter(Collider other);
}


