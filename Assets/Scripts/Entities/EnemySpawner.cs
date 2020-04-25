using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : AIAgent // give current path to goal to enemy on spawn
{
    public List<EnemyPack> enemySet;

    public MapTile associatedTile;
    public float spawnResetTime;
    public bool CanSpawn { get => spawnElapsedTime > spawnResetTime && allEnemies.Count > 0; }
    public float spawnElapsedTime;

    private FoundPath pathToGoal;
    private List<OrganicAgent> allEnemies;

    private void Start()
    {
        stateMachine = new EnemySpawnerBrain(this);

        DefendState.Instance.openDefend += GetPath;
        LoadAllEnemies();
        
        spawnElapsedTime = 0;
    }

    private void OnDestroy()
    {
        DefendState.Instance.openDefend -= GetPath;            
    }

    private void LoadAllEnemies()
    {
        allEnemies = new List<OrganicAgent>();

        foreach(EnemyPack pack in enemySet)
        {
            for(int i = 0; i < pack.count; i++)
            {
                OrganicAgent agent = Instantiate(
                    pack.enemy, transform.position, Quaternion.identity, transform)
                    .GetComponent<OrganicAgent>();
                allEnemies.Add(agent);
                agent.gameObject.SetActive(false);
            }
        }
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

        OrganicAgent agent = allEnemies[0];
        agent.gameObject.SetActive(true);
        agent.AssignPath(pathToGoal);
        allEnemies.RemoveAt(0);
        if (Debugger.Instance.EnemyMessages) { Debug.Log(agent.name + " spawned"); }

        if(allEnemies.Count == 0) { OnOutOfEnemies(); }
    }

    public void OnOutOfEnemies()
    {
        if (Debugger.Instance.EnemyMessages) { Debug.Log(name + " out of enemies"); }
    }

    // BAD
    public static List<EnemyPack> AllEnemyPacks()
    {
        List<EnemyPack> ret = new List<EnemyPack>();
        foreach(EnemySpawner spawner in FindObjectsOfType<EnemySpawner>())
        {
            foreach(EnemyPack pack in spawner.enemySet)
            {
                ret.Add(pack);
            }
        }
        return ret;
    }

    public static List<HostileAgent> AllEnemies()
    {
        List<HostileAgent> ret = new List<HostileAgent>();
        foreach(EnemySpawner spawner in FindObjectsOfType<EnemySpawner>())
        {
            foreach(HostileAgent agent in spawner.allEnemies)
            {
                ret.Add(agent);
            }
        }
        return ret;
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
        base.MakeDecision();
        ((EnemySpawner)agent).SpawnEnemy();
        return null;
    }
}

public class RechargeTimer : Action
{
    public RechargeTimer(EnemySpawner agent) : base(agent) { }

    public override IDecision MakeDecision()
    {
        base.MakeDecision();
        ((EnemySpawner)agent).spawnElapsedTime += Time.deltaTime;
        return null;
    }
}