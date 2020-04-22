using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTurretBrain : MonoBehaviour
{
    public GameObject projectile;
    // public Transform barrel;
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

    // thing rotates until it can get a shot
    // rotation speed determines a good part of firing speed
    // but firing speed is still a thing

    private void Shoot()
    {
        shotElapsedTime = 0;
        HostileAgent agent = NearestEnemy();
        if(null == agent) { return; }

        GameObject proj = Instantiate(projectile, transform.position, transform.rotation, transform);
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
