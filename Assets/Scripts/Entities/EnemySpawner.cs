using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// creates enemies, and gives them the current path to the goal
/// </summary>
public class EnemySpawner : AIAgent
{
    [Tooltip("enemies to spawn")] public List<EnemyPack> enemySet;
    [Tooltip("tile object")] public MapTile associatedTile;
    [Tooltip("time to wait between enemy spawning")] public float spawnResetTime;  
    [Tooltip("time since last enemyspawn")] public float spawnElapsedTime;
    public bool CanSpawn { get => spawnElapsedTime > spawnResetTime && allEnemies.Count > 0; } // return if new enemy can be spawned

    private FoundPath pathToGoal; // path from associated tile to goal tile
    private List<OrganicAgent> allEnemies; // all enemies that are going to be/have been spawned

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

    /// <summary>
    /// instantiate all enemies
    /// </summary>
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

    /// <summary>
    /// get path from tile to goal
    /// </summary>
    public void GetPath()
    {
        associatedTile = GetComponent<MapTile>(); // better system

        pathToGoal = Pathfinder.DjikstrasPath(
            PlaceState.Instance.Board.FindAssociatedNode(associatedTile),
            PlaceState.Instance.Board.GoalNode,
            PlaceState.Instance.Board.nodeMap);
    }

    /// <summary>
    /// create a new enemy, give it path to goal
    /// </summary>
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

    /// <summary>
    /// action when no more enemies to spawn
    /// </summary>
    public void OnOutOfEnemies()
    {
        if (Debugger.Instance.EnemyMessages) { Debug.Log(name + " out of enemies"); }
    }

    // ToDo: BAD
    /// <summary>
    /// compile and return a list of all enemy packs in scene
    /// </summary>
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

    /// <summary>
    /// compile and return a list of all hostile agents (enemies) in scene
    /// </summary
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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(null != pathToGoal.start)
        {
            Gizmos.color = Color.red;
            foreach(PathNode n in pathToGoal.path)
            {
                GameObject g = GamePlayState.CurrentLevel.Board.FindAssociatedTile(n).gameObject;
                Vector3 p = g.transform.position;
                p.z -= 1;
                Gizmos.DrawSphere(p, 0.2f);
            }
        }
    }
}

/// <summary>
/// driving state maching for enemy spawner
/// </summary>
public class EnemySpawnerBrain : DecisionTree
{
    BooleanDecision shouldSpawn; // should an enemy be spawned

    Action spawnEnemy; // create a new enemy object
    Action rechargeTimer; // recharge spawn timer

    /// <param name="agent">the agent to act upon</param>
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

/// <summary>
/// spawn a new enemy
/// </summary>
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

/// <summary>
/// recharge a timer with deltatime
/// </summary>
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