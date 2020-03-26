using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    [SerializeField] private int number;
    public int Number { get { return number; } private set { } }

    [SerializeField] private List<TileAllotment> allottedTiles;
    public List<TileAllotment> AllottedTiles { get { return allottedTiles; } private set { } }
    [SerializeField] private List<EnemyPack> enemyHorde;
    public List<EnemyPack> EnemyHorde { get { return enemyHorde; } private set { } }
    [SerializeField] private GameBoard board;
    public GameBoard Board { get { return board; } private set { } }

    // public void InitializeLevel(int level) {} ??
    // where to put algorithm???
    // hardcode levels???

    private Level() { } // so I can't make my own (??)

    public static Level CreateLevel(int level)
    {
        Level ret = new Level();
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
        return new List<EnemyPack>();
    }

    public void AssignBoard(GameBoard givenBoard)
    {
        board = givenBoard;
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
    public PathNode goalNode;
    public bool GoalAssigned { get { return null != goalNode; } private set { } }
    public NodeMap nodeMap;
    public TileMap tileMap;

    private GameBoard() { }
    public GameBoard(NodeMap _nodeMap, TileMap _tileMap)
    {
        nodeMap = _nodeMap;
        tileMap = _tileMap;
    }

    public void AssignGoal(PathNode newGoal)
    {
        goalNode?.RemoveFromGoal();
        goalNode = newGoal;
        goalNode?.AssignAsGoal();
    }

    public MapTile FindAssociatedTile(PathNode node)
    {
        Debug.Assert(tileMap.Tiles[node.Index] != null);
        return node == null ? null : tileMap.Tiles[node.Index];
    }

    public PathNode FindAssociatedNode(MapTile tile)
    {
        if(tile.Index < 0 || tile.Index >= nodeMap.Nodes.Count) { Debug.Log(tile.name + " " + tile.Index); }
        return nodeMap.Nodes[tile.Index];
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
        // if(goalNode == FindAssociatedNode(oldTile)) { goalNode = null; } 

        Debug.Assert(null != oldTile);

        MapTile newTile = oldTile.InstantiateInPlace(tileModel); // preserves index
        newTile.AssignData(tileModel.Data, true);
        newTile.placedByPlayer = true;

        NodeMap.ReplaceConnections(FindAssociatedNode(oldTile), FindAssociatedNode(newTile));

        if(newTile.Type == TileType.wall) { FindAssociatedNode(tileModel).ClearConnections(); } // wall
        if(oldTile.Type == TileType.goal) { PlaceState.Instance.Board.AssignGoal(null); }
        if (newTile.Type == TileType.goal)
        {
            PlaceState.Instance.Board.AssignGoal(FindAssociatedNode(newTile));
        }

        return newTile;
    }

    public void RemoveNode(PathNode node, bool removeTile = true)
    {
        nodeMap.RemoveNode(node);
        if (removeTile) { RemoveTile(FindAssociatedTile(node)); }
    }

    public void RemoveTile(MapTile tile, bool removeNode = true)
    {
        tileMap.RemoveTile(tile);
        if (removeNode) { RemoveNode(FindAssociatedNode(tile)); }
    }

    public void Destroy()
    {
        foreach(MapTile tile in tileMap.Tiles) { tile.Destroy(); }
        tileMap.Tiles.Clear();

        goalNode = null;
        tileMap = null;
        nodeMap = null;
    }
}
