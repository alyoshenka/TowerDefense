using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a game level
/// </summary>
[System.Serializable]
public class Level
{
    [SerializeField] [Tooltip("level number")] private int number;
    public int Number { get => number; } // get level number
    public bool WinCon { get => allEnemies.Count == 0; } // get level win

    [SerializeField] [Tooltip("tiles that can be placed")] private List<TileAllotment> allottedTiles;
    public List<TileAllotment> AllottedTiles { get => allottedTiles; } // get allotted tiles
    [SerializeField] [Tooltip("all enemies in this level")] private List<EnemyPack> enemyHorde;
    public List<EnemyPack> EnemyHorde { get => enemyHorde; } // get enemy horde
    private List<HostileAgent> allEnemies; // all enemies in this level
    [SerializeField] [Tooltip("assiciated game board")] private GameBoard board;
    public GameBoard Board { get => board; } // get board

    // public void InitializeLevel(int level) {} ??
    // where to put algorithm???
    // hardcode levels???

    private Level() { } // so I can't make my own (??)

    // ToDo: change level creation implementation -> procgen or grab from list
    /// <summary>
    /// create a new level
    /// </summary>
    /// <param name="level">level number</param>
    public static Level CreateLevel(int level)
    {
        Level lev = new Level();
        DefendState.Instance.openDefend += (() => { lev.allEnemies = EnemySpawner.AllEnemies(); }); // cancer
        lev.number = level;

        if(level <= 0) { return lev; } // start

        lev.allottedTiles = new List<TileAllotment>();
        lev.enemyHorde = new List<EnemyPack>();

        lev.allottedTiles = AllotTiles(level);
        lev.enemyHorde = AssignEnemies(level);

        return lev;
    }

    // ToDo: BAD

    /// <summary>
    /// allot tiles for the level, not finished!
    /// </summary>
    /// <param name="level">level number</param>
    /// <returns>allotted tiles for level</returns>
    private static List<TileAllotment> AllotTiles(int level)
    {
        return new List<TileAllotment>();
    }

    /// <param name="level">level number</param>
    /// <returns>get all enemy packs in level</returns>
    private static List<EnemyPack> AssignEnemies(int level)
    {
        // return new List<EnemyPack>();
        return EnemySpawner.AllEnemyPacks();
    }

    /// <summary>
    /// remove enemy from level, check wincon
    /// </summary>
    /// <param name="agent">agent to remove</param>
    public void DestroyEnemy(HostileAgent agent)
    {
        allEnemies.Remove(agent);

        Debugger.Instance.BL.text = "Total enemies: " + allEnemies.Count;
        
        if(WinCon) 
        { 
            Debug.Log("win cond");
            GameStateManager.Instance.GameOver(true);
        }
        else if(allEnemies.Count < 0) { Debug.Assert(false); }
    }

    /// <summary>
    /// assign board to level
    /// </summary>
    public void AssignBoard(GameBoard givenBoard)
    {
        board = givenBoard;
    }

    /// <summary>
    /// properly dispose of level
    /// </summary>
    public void Destroy()
    {
        allottedTiles.Clear();
        allottedTiles = null;
        enemyHorde.Clear();
        enemyHorde = null;
        board.Destroy();
    }
}

/// <summary>
/// tiles allowed in level
/// </summary>
[System.Serializable]
public class TileAllotment
{
    [Tooltip("tile object")] public MapTile tile; 
    [Tooltip("number of given tiles allowed")] public int count;
}

/// <summary>
/// number of enemies in level
/// </summary>
[System.Serializable]
public class EnemyPack
{
    [Tooltip("enemy object")] public HostileAgent enemy;
    [Tooltip("number allotted")] public int count;
}

/// <summary>
/// tile board
/// </summary>
[System.Serializable]
public class GameBoard
{
    private PathNode goalNode; // castle/base/goal node
    public PathNode GoalNode { get => goalNode; } // get goal node
    public GoalTile goalTile { get => (GoalTile)FindAssociatedTile(goalNode); } // get goal tile
    public bool GoalAssigned { get => null != goalNode; } // get if goal is assigned

    [Tooltip("map of all nodes")] public NodeMap nodeMap;
    [Tooltip("map of all tiles")] public TileMap tileMap;

    private GameBoard() { }
    /// <summary>
    /// make a new game board
    /// </summary>
    /// <param name="_nodeMap">associated nodes</param>
    /// <param name="_tileMap">associated tiles</param>
    public GameBoard(NodeMap _nodeMap, TileMap _tileMap)
    {
        nodeMap = _nodeMap;
        tileMap = _tileMap;

        AssignGoal(nodeMap.Nodes.Find(node => node.Type == TileType.goal));
    }

    /// <summary>
    /// set goal node
    /// </summary>
    public void AssignGoal(PathNode newGoal)
    {
        goalNode?.RemoveFromGoal();
        goalNode = newGoal;
        goalNode?.AssignAsGoal();
    }

    /// <summary>
    /// return tile object associated with given node
    /// </summary>
    public MapTile FindAssociatedTile(PathNode node)
    {
        if(null == node || node.Index < 0 || node.Index >= tileMap.Tiles.Count)
        {
            Debug.Assert(false);
            return null;
        }
        else {  return tileMap.Tiles[node.Index]; }
    }

    /// <summary>
    /// return node associated with given tile
    /// </summary>
    public PathNode FindAssociatedNode(MapTile tile)
    {
        if(null == tile || tile.Index < 0 || tile.Index >= nodeMap.Nodes.Count)
        {
            Debug.Assert(false);
            return null;
        }
        else { return nodeMap.Nodes[tile.Index]; }       
    }

    /// <summary>
    /// assign new tile data to given node and associated tile
    /// </summary>

    public void AssignNewData(PathNode node, TileData data)
    {
        node.AssignData(data);
        FindAssociatedTile(node).AssignData(data, false);
    }

    /// <summary>
    /// assign new tile data to given node and associated tile
    /// preserve index, if applicable
    /// </summary>
    public void AssignNewData(MapTile tile, TileData data, bool preserveIndex = true)
    {
        tile.AssignData(data, preserveIndex);
        FindAssociatedNode(tile).AssignData(data);
    }

    /// <summary>
    /// assign a new model to a given map tile, copying and/or preserving
    /// all associated data
    /// </summary>
    /// <param name="oldTile">tile to "edit"</param>
    /// <param name="tileModel">new tile model prefab</param>
    /// <returns></returns>
    public MapTile AssignNewTile(MapTile oldTile, MapTile tileModel)
    {
        Debug.Assert(null != oldTile);

        MapTile newTile = oldTile.InstantiateInPlace(tileModel); // preserves index
        newTile.AssignData(tileModel.Data, true);
        newTile.placedByPlayer = true;

        nodeMap.SetNodeType(FindAssociatedNode(newTile), newTile.Type); // could cause problems when presets are no longer used
        tileMap.ReplaceTile(oldTile, newTile); // here

        if (newTile.Type == TileType.wall || newTile.Type == TileType.turret) { FindAssociatedNode(newTile).ClearConnections(); } // wall
        if (oldTile.Type == TileType.goal) { PlaceState.Instance.Board.AssignGoal(null); }
        if (newTile.Type == TileType.goal) { PlaceState.Instance.Board.AssignGoal(FindAssociatedNode(newTile)); }

        return newTile;
    }

    /// <summary>
    /// remove node from map
    /// </summary>
    /// <param name="removeTile">remove associated tile?</param>
    public void RemoveNode(PathNode node, bool removeTile = true)
    {
        nodeMap.RemoveNode(node);
        if (removeTile) { RemoveTile(FindAssociatedTile(node), false); }
    }

    /// <summary>
    /// remove tile from map
    /// </summary>
    /// <param name="removeNode">remove associated node?</param>
    public void RemoveTile(MapTile tile, bool removeNode = true)
    {
        tileMap.RemoveTile(tile);
        if (removeNode) { RemoveNode(FindAssociatedNode(tile), false); }
    }

    /// <summary>
    /// properly dispose of object and all references
    /// </summary>
    public void Destroy()
    {      
        while(tileMap.Tiles.Count > 0)
        {
            MapTile tile = tileMap.Tiles[0];
            tileMap.RemoveTile(tile);
            tile.Destroy();
        }
        tileMap.Tiles.Clear();

        goalNode = null;
        tileMap = null;
        nodeMap = null;
    }
}
