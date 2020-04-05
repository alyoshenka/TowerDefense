using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTurretBrain : MonoBehaviour
{
    public Projectile projectile;
    public float shotResetTime;

    float shotElapsedTime;

    private void Start()
    {
        shotElapsedTime = 0;
    }

    private void Update()
    {
        shotElapsedTime += Time.deltaTime;
        if(shotElapsedTime > shotResetTime) { Shoot(); }
    }

    private void Shoot()
    {
        shotElapsedTime = 0;
        HostileAgent agent = NearestEnemy();
        if(null == agent) { return; }

        GameObject proj = Instantiate(projectile, transform.position, transform.rotation, transform).gameObject;
        proj.transform.LookAt(agent.transform);
    }

    private HostileAgent NearestEnemy()
    {
        HostileAgent[] enemies = FindObjectsOfType<HostileAgent>();
        if(enemies.Length == 0) { return null; }

        HostileAgent ret = enemies[0];
        float dist = Vector3.Distance(transform.position, ret.transform.position);
        foreach(HostileAgent agent in enemies)
        {
            float n = Vector3.Distance(transform.position, agent.transform.position);
            if(n < dist)
            {
                dist = n;
                ret = agent;
            }
        }
        return ret;
    }
}
