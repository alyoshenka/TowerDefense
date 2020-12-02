using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple level settings
/// </summary>
[System.Serializable]
public class LevelData
{
    public int number = 0;
    public string name = "basic level";
    public WinCondition winCon = WinCondition.defeatAllEnemies;
    public int expOnWin = 0;
    public int goldOnwin = 0;
}

/// <summary>
/// ways to beat a level
/// </summary>
public enum WinCondition
{
    defeatAllEnemies,
    collectXGold,
    connectXExp,
    surviveForXTime
};


/// <summary>
/// a game level
/// </summary>
[System.Serializable]
public class Level : ISaveable<Level_Save>
{
    public static readonly string ext = ".level";

    [Tooltip("level specific data")] [SerializeField]
    private LevelData levelData;
    [Tooltip("assiciated game board")] [SerializeField]
    private GameBoard board;
    [Tooltip("tiles that can be placed")] [SerializeField]
    private List<TileAllotment> allottedTiles;
    [Tooltip("all enemies in this level")] [SerializeField]
    private List<HostileAgent> allEnemies;
      
    public GameBoard Board { get => board; }
    public List<TileAllotment> AllottedTiles { get => allottedTiles; } 
    public string Name { get => levelData.name; set => levelData.name = value; }

    // public void InitializeLevel(int level) {} ??
    // where to put algorithm???
    // hardcode levels???

    // ToDo: change level creation implementation -> procgen or grab from list

    public Level(LevelData _levelData, GameBoard _board)
    {
        levelData = _levelData;
        board = _board;

        DefendState.Instance.openDefend += (() =>
        {
            allEnemies = board.GetAllEnemies();
        });
    }

    /// <summary>
    /// remove enemy from level, check wincon
    /// </summary>
    /// <param name="agent">agent to remove</param>
    public void DestroyEnemy(HostileAgent agent)
    {
        allEnemies.Remove(agent);

        Debugger.Instance.BL.text = "Total enemies: " + allEnemies.Count;
        
        if(LevelWin())
        { 
            Debug.Log("win cond");
            GameStateManager.Instance.GameOver(true);
        }
    }

    /// <returns>if the win condition(s) have been met</returns>
    public bool LevelWin()
    {
        return WinCondition.defeatAllEnemies == levelData.winCon && allEnemies.Count == 0; // ToDo: implement support for different win conditions
    }

    public Level_Save ToSave()
    {
        return new Level_Save
        {
            data = levelData,
            boardName = board.name
        };
    }

    public void FromLoad(Level_Save data)
    {
        levelData = data.data;
        if (null == board) { board = new GameBoard(); }
        board.FromLoad(board.Load(LevelBuilder.saveDir + data.boardName + GameBoard.ext));
    }

    public bool Save(string fileName, Level_Save data)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("saving level to: " + fileName); }
        System.IO.File.WriteAllText(@fileName, UnityEngine.JsonUtility.ToJson(data, true));
        return true;
    }

    public Level_Save Load(string fileName)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("loading level data from: " + fileName); }
        return JsonUtility.FromJson<Level_Save>(System.IO.File.ReadAllText(@fileName));
    }

    public IEnumerator<Level_Save> GetEnumerator() { throw new System.NotImplementedException(); }

    IEnumerator IEnumerable.GetEnumerator() { throw new System.NotImplementedException(); }
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
public class GameBoard : ISaveable<GameBoard_Save>
{
    public static readonly string ext = ".board";

    [Tooltip("board size (w, h)")] [SerializeField]
    private Vector2 size;
    [Tooltip("all nodes")] 
    public List<PathNode> nodes;
    [Tooltip("all tiles")] 
    public List<MapTile> tiles;
    public string name;

    public Vector2 Size { get => size; set => size = value; }
    public PathNode goalNode { get; private set; }
    public GoalTile goalTile { get => (GoalTile)FindAssociatedTile(goalNode); } // get goal tile
    public bool GoalAssigned { get => null != goalNode; } // get if goal is assigned

    public GameBoard() { }

    /// <summary>
    /// make a new game board
    /// </summary>
    /// <param name="_nodeMap">associated nodes</param>
    /// <param name="_tileMap">associated tiles</param>
    public GameBoard(Vector2 _size, List<PathNode> _nodes, List<MapTile> _tiles)
    {
        Debug.Assert(_nodes.Count == _tiles.Count && _size.x * _size.y == _nodes.Count);

        size = _size;
        nodes = _nodes;
        tiles = _tiles;

        AssignGoal(nodes.Find(node => node.Type == TileType.goal));
    }

    /// <summary>
    /// set goal node, handles null argument
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
        Debug.Assert(null != node && node.Index >= 0 && node.Index < tiles.Count);
        return tiles[node.Index];
    }

    /// <summary>
    /// return node associated with given tile
    /// </summary>
    public PathNode FindAssociatedNode(MapTile tile)
    {
        Debug.Assert(null != tile && tile.Index >= 0 && tile.Index < nodes.Count);
        return nodes[tile.Index];
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
        Debug.Assert(null != oldTile && null != tileModel && oldTile != tileModel);

        MapTile newTile = oldTile.InstantiateInPlace(tileModel); // preserves index
        newTile.AssignData(tileModel.Data, true);
        newTile.placedByPlayer = true;

        PathNode node = FindAssociatedNode(newTile);
        node.AssignData(TileData.FindByType(tileModel.Type)); // ToDo: could cause problems when presets are no longer used
        Debug.Assert(node.Index == newTile.Index);          
        tiles.Insert(node.Index, newTile);

        // ToDo: system for connections
        if (newTile.Type == TileType.wall || newTile.Type == TileType.turret) { FindAssociatedNode(newTile).ClearConnections(); } // wall
        if (oldTile.Type == TileType.goal) { PlaceState.Instance.Board.AssignGoal(null); }
        if (newTile.Type == TileType.goal) { PlaceState.Instance.Board.AssignGoal(FindAssociatedNode(newTile)); }

        return newTile;
    }

    /// <returns>list of all enemies in the board</returns>
    public List<HostileAgent> GetAllEnemies()
    {
        List<HostileAgent> ret = new List<HostileAgent>();
        foreach(MapTile t in tiles)
        {
            EnemySpawner es = t.GetComponent<EnemySpawner>(); // ToDo: rewrite
            if (es) { ret.AddRange(es.allEnemies); }
        }
        return ret;
    }

    public GameBoard_Save ToSave()
    {
        GameBoard_Save gbs = new GameBoard_Save();

        gbs.name = name;
        gbs.size = size;
        gbs.tiles = new TileType[(int)(size.x * size.y)];
        gbs.tilePositions = new Vector3[(int)(size.x * size.y)];
        gbs.constantTiles = new List<int>();
        for(int i = 0; i < tiles.Count; i++)
        {
            MapTile t = tiles[i];
            gbs.tiles[i] = t.Type;
            gbs.tilePositions[i] = t.transform.position;
            if (!t.CanBeChanged) { gbs.constantTiles.Add(i); }
        }

        return gbs;
    }

    public void FromLoad(GameBoard_Save data)
    {
        name = data.name;
        size = data.size;
        tiles = new List<MapTile>();
        // initialize tiles
        List<TileData> tileData = new List<TileData>();
        foreach(TileType tt in data.tiles) { tileData.Add(TileData.FindByType(tt)); }
        NodeMap nodeMap = MapGenerator.GenerateNodeMap(tileData, size);
        TileMap tileMap = MapGenerator.GenerateTileMap(nodeMap, null);
        nodes = nodeMap.Nodes;
        tiles = tileMap.Tiles;
        foreach(int i in data.constantTiles) { tiles[i].CanBeChanged = false; }
    }

    public IEnumerator<GameBoard_Save> GetEnumerator() { throw new System.NotImplementedException(); }

    IEnumerator IEnumerable.GetEnumerator() { throw new System.NotImplementedException(); }

    public bool Save(string fileName, GameBoard_Save data)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("saving game board to: " + fileName); }
        System.IO.File.WriteAllText(@fileName, UnityEngine.JsonUtility.ToJson(data, true));
        return true;
    }

    public GameBoard_Save Load(string fileName)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("loading board from: " + fileName); }
        return JsonUtility.FromJson<GameBoard_Save>(System.IO.File.ReadAllText(@fileName));
    }
}
