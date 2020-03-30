using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : AIAgent // give current path to goal to enemy on spawn
{
    public GameObject enemyToSpawn;
    public MapTile associatedTile;
    public int count;
    public float spawnResetTime;
    public bool CanSpawn { get => spawnElapsedTime > spawnResetTime && count > 0; }

    public float spawnElapsedTime;

    FoundPath pathToGoal;

    private void Start()
    {
        stateMachine = new EnemySpawnerBrain(this);

        DefendState.Instance.openDefend += GetPath;     

        spawnElapsedTime = 0;
    }

    private void OnDestroy()
    {
        DefendState.Instance.openDefend -= GetPath;            
    }

    public void GetPath()
    {
        associatedTile = GetComponent<MapTile>(); // better system

        pathToGoal = Pathfinder.DjikstrasPath(
            PlaceState.Instance.Board.FindAssociatedNode(associatedTile),
            PlaceState.Instance.Board.GoalNode,
            PlaceState.Instance.Board.nodeMap);
    }

    public void SpawnEnemy()
    {
        spawnElapsedTime = 0;
        count--;

        HostileAgent agent = Instantiate(
            enemyToSpawn, transform.position, Quaternion.identity, transform)
            .GetComponent<HostileAgent>();
        agent.AssignPath(pathToGoal);

        Debug.Log(agent.name + " spawned");
    }
}

public class EnemySpawnerBrain : DecisionTree
{
    BooleanDecision shouldSpawn;

    Action spawnEnemy;
    Action rechargeTimer;

    public EnemySpawnerBrain(EnemySpawner agent)
    {
        spawnEnemy = new SpawnEnemy(agent);
        rechargeTimer = new RechargeTimer(agent);

        shouldSpawn = new BooleanDecision(spawnEnemy, rechargeTimer);

        start = shouldSpawn;
    }

    public override void Update(AIAgent agent)
    {
        shouldSpawn.Value = ((EnemySpawner)agent).CanSpawn;
    }
}

public class SpawnEnemy : Action
{
    public SpawnEnemy(EnemySpawner agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((EnemySpawner)agent).SpawnEnemy();
        return null;
    }
}

public class RechargeTimer : Action
{
    public RechargeTimer(EnemySpawner agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        ((EnemySpawner)agent).spawnElapsedTime += Time.deltaTime;
        return null;
    }
}