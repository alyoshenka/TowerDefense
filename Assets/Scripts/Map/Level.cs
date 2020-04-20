using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    [SerializeField] private int number;
    public int Number { get => number; }
    public bool WinCon { get => allEnemies.Count == 0; }

    [SerializeField] private List<TileAllotment> allottedTiles;
    public List<TileAllotment> AllottedTiles { get => allottedTiles; }
    [SerializeField] private List<EnemyPack> enemyHorde;
    public List<EnemyPack> EnemyHorde { get => enemyHorde; }
    private List<HostileAgent> allEnemies;
    [SerializeField] private GameBoard board;
    public GameBoard Board { get => board; }

    // public void InitializeLevel(int level) {} ??
    // where to put algorithm???
    // hardcode levels???

    private Level() { } // so I can't make my own (??)

    public static Level CreateLevel(int level)
    {
        Level ret = new Level();
        DefendState.Instance.openDefend += (() => { ret.allEnemies = EnemySpawner.AllEnemies(); }); // cancer
        ret.number = level;

        if(level <= 0) { return ret; } // start

        ret.allottedTiles = new List<TileAllotment>();
        ret.enemyHorde = new List<EnemyPack>();

        ret.allottedTiles = AllotTiles(level);
        ret.enemyHorde = AssignEnemies(level);

        return ret;
    }

    // bad
    private static List<TileAllotment> AllotTiles(int level)
    {
        return new List<TileAllotment>();
    }

    private static List<EnemyPack> AssignEnemies(int level)
    {
        // return new List<EnemyPack>();
        return EnemySpawner.AllEnemyPacks();
    }

    public void DestroyEnemy(HostileAgent agent)
    {
        allEnemies.Remove(agent);
        
        if(WinCon) 
        { 
            Debug.Log("win cond");
            GameStateManager.Instance.GameOver(true);
        }
        else if(allEnemies.Count < 0) { Debug.Assert(false); }
    }

    public void AssignBoard(GameBoard givenBoard)
    {
        board = givenBoard;
    }

    public void Destroy()
    {
        allottedTiles.Clear();
        allottedTiles = null;
        enemyHorde.Clear();
        enemyHorde = null;
        board.Destroy();
    }
}

[System.Serializable]
public class TileAllotment
{
    public MapTile tile;
    public int count;
}

[System.Serializable]
public class EnemyPack
{
    public HostileAgent enemy;
    public int count;
}

[System.Serializable]
public class GameBoard
{
    private PathNode goalNode;
    public PathNode GoalNode { get => goalNode; }
    public GoalTile goalTile { get => (GoalTile)FindAssociatedTile(goalNode); }
    public bool GoalAssigned { get => null != goalNode; }
    public NodeMap nodeMap;
    public TileMap tileMap;

    private GameBoard() { }
    public GameBoard(NodeMap _nodeMap, TileMap _tileMap)
    {
        nodeMap = _nodeMap;
        tileMap = _tileMap;

        AssignGoal(nodeMap.Nodes.Find(node => node.Type == TileType.goal));
    }

    public void AssignGoal(PathNode newGoal)
    {
        goalNode?.RemoveFromGoal();
        goalNode = newGoal;
        goalNode?.AssignAsGoal();
    }

    public MapTile FindAssociatedTile(PathNode node)
    {
        if(null == node || node.Index < 0 || node.Index >= tileMap.Tiles.Count)
        {
            Debug.Assert(false);
            return null;
        }
        else {  return tileMap.Tiles[node.Index]; }
    }

    public PathNode FindAssociatedNode(MapTile tile)
    {
        if(null == tile || tile.Index < 0 || tile.Index >= nodeMap.Nodes.Count)
        {
            Debug.Assert(false);
            return null;
        }
        else { return nodeMap.Nodes[tile.Index]; }       
    }

    public void AssignNewData(PathNode node, TileData data)
    {
        node.AssignData(data);
        FindAssociatedTile(node).AssignData(data, false);
    }

    public void AssignNewData(MapTile tile, TileData data, bool preserveIndex = true)
    {
        tile.AssignData(data, preserveIndex);
        FindAssociatedNode(tile).AssignData(data);
    }

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

    public void RemoveNode(PathNode node, bool removeTile = true)
    {
        nodeMap.RemoveNode(node);
        if (removeTile) { RemoveTile(FindAssociatedTile(node), false); }
    }

    public void RemoveTile(MapTile tile, bool removeNode = true)
    {
        tileMap.RemoveTile(tile);
        if (removeNode) { RemoveNode(FindAssociatedNode(tile), false); }
    }

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
