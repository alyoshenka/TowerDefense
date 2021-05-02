using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// creates enemies, and gives them the current path to the goal
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Tooltip("enemies to spawn")] 
    public List<EnemyPack> enemySet;
    [Tooltip("tile object")] 
    public MapTile associatedTile;
    [Tooltip("Pathfinding object")]
    public PathShower pathShower;

    [SerializeField]
    [Tooltip("time since last enemyspawn")] 
    private float spawnElapsedTime;



    private FoundPath pathToGoal; // path from associated tile to goal tile


    private void Start()
    {
        if(null == pathShower) 
        { 
            Debug.LogWarning("set path shower field");
            pathShower = Resources.FindObjectsOfTypeAll<PathShower>()[0]; // BAD
        }
        Debug.Assert(null != pathShower);

        DefendState.Instance.openDefend += GetPath;
        pathShower.pathFound += OnPathToGoalFound;

        // stop coroutine on exit??

        if(null == associatedTile)
        {
            associatedTile = GetComponent<MapTile>(); // ToDo: better system
        }
        spawnElapsedTime = 0;
    }

    private void OnDestroy()
    {
        DefendState.Instance.openDefend -= GetPath;            
    }

    /// <summary>
    /// get path from tile to goal
    /// </summary>
    public void GetPath()
    {


        /*
        pathToGoal = Pathfinder.DjikstrasPath(
            associatedTile,
            PlaceState.Instance.Board.goalTile,
            PlaceState.Instance.Board.tiles
        );
        */

       
        pathShower.VisualizePathfinding(
            associatedTile,
            PlaceState.Instance.Board.GoalTile,
            PlaceState.Instance.Board.tiles
        );

       
    }

    /// <summary>
    /// called when path from here to goal is found
    /// </summary>
    private void OnPathToGoalFound()
    {
        pathToGoal = pathShower.pathSave;

        // placing this here makes it so that enemy spawn
        // loop only begins after path is found
        StartCoroutine(EnemySpawnLoop());
    }

    /// <summary>
    /// spawns the enemy at the beginning of the set, 
    /// adds it to the current level
    /// </summary>
    private void SpawnEnemy()
    {
        EnemySO eso = 
            (EnemySO)
            ((ICollectionManager<EnemyType>)DefendState.Instance.enemyManager)
            .Get(enemySet[0].enemy);

        GameObject prefab = eso.prefab;

        HostileAgent ho = Instantiate(
            prefab, 
            transform.position, 
            Quaternion.identity, 
            DefendState.Instance.enemyParent)
        .GetComponent<HostileAgent>();

        // assign enemy path
        ((OrganicAgent)ho).AssignPath(pathToGoal);

        DefendState.CurrentLevel.AddEnemy(ho); // To Do: lots of casting bad
    }

    /// <summary>
    /// spawns enemies from set at given time
    /// </summary>
    private IEnumerator EnemySpawnLoop()
    {
        yield return new WaitForSeconds(enemySet[0].buildupTime);
        SpawnEnemy();

        enemySet[0].count--;
        if(enemySet[0].count <= 0) 
        { 
            enemySet.RemoveAt(0); 
            if(enemySet.Count == 0) { OnOutOfEnemies(); }
        }
    }

    /// <summary>
    /// action when no more enemies to spawn
    /// </summary>
    public void OnOutOfEnemies()
    {
        if (Debugger.Instance.EnemyMessages) { Debug.Log(name + " out of enemies"); }
    }

    public void OnDrawGizmos()
    {
        if(null != pathToGoal.start)
        {
            Gizmos.color = Color.red;
            foreach(MapTile t in pathToGoal.path)
            {
                GameObject g = t.gameObject;
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
/*
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
*/

/// <summary>
/// spawn a new enemy
/// </summary>
/*
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
*/

/// <summary>
/// recharge a timer with deltatime
/// </summary>
/*
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
*/