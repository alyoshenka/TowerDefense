﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using System.Text.Json;
using Newtonsoft.Json;


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
        board = GameBoard.LoadBoard(LevelBuilder.saveDir + data.boardName + GameBoard.ext);
    }

    public bool Save(string fileName, Level_Save data)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("saving level to: " + fileName); }
        // System.IO.File.WriteAllText(@fileName, JsonSerializer.Serialize(data));
        System.IO.File.WriteAllText(@fileName, JsonConvert.SerializeObject(data));
        return true;
    }

    public Level_Save Load(string fileName)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("loading level data from: " + fileName); }
        // return JsonSerializer.Deserialize<Level_Save>(System.IO.File.ReadAllText(@fileName));
        // return JsonUtility.FromJson<Level_Save>(System.IO.File.ReadAllText(@fileName));
        return JsonConvert.DeserializeObject<Level_Save>(System.IO.File.ReadAllText(@fileName));

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
    [Tooltip("tile object")] public TileSO tile; 
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
    public string name;
    [Tooltip("all tiles")] 
    public List<MapTile> tiles;   

    public Vector2 Size { get => size; set => size = value; }
    public MapTile goalTile { get => tiles.Find(t => t.Data.tileType == TileType.goal); }
    public bool GoalAssigned { get => null != goalTile; }

    private GameBoard() { }

    public static GameBoard LoadBoard(string fileName)
    {
        GameBoard board = new GameBoard();
        GameBoard_Save gbs = board.Load(fileName);
        board.FromLoad(gbs);
        return board;
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
        int len = (int)(size.x * size.y);

        gbs.name = name;
        gbs.size = size;
        gbs.tiles = new TileType[len];
        gbs.tilePositions = new Vector3[len];
        gbs.constantTiles = new List<int>();
        gbs.tileConnections = new List<int[]>();

        for(int i = 0; i < tiles.Count; i++)
        {
            MapTile t = tiles[i];
            gbs.tiles[i] = t.Data.tileType;
            gbs.tilePositions[i] = t.transform.position;

            gbs.tileConnections.Add(new int[t.Connections.Count]);
            if (t.Connections.Count > 0)
            {              
                for (int j = 0; j < t.Connections.Count; j++)
                {
                    gbs.tileConnections[i][j] = tiles.IndexOf(t.Connections[i]);
                }
                if (!t.CanBeChanged) { gbs.constantTiles.Add(i); }
            }
        }

        if (0 == tiles.Count) // initialize array if no tiles yet
        {
            for (int y = 0; y < size.y; y++)
            {
                for(int x = 0; x < size.x; x++)
                {
                    int i = (int)(y * size.x + x);
                    gbs.tilePositions[i] = new Vector3(x - size.x / 2, -y + size.y / 2, 0);
                }               
            }
        }

        Debug.Log("tiles with connections: " + gbs.tileConnections.Count);
        return gbs;
    }

    public void FromLoad(GameBoard_Save data)
    {
        name = data.name;
        size = data.size;
        tiles = new List<MapTile>();
        MapGenerator.InitializeBoardTiles(this, data, null);
        foreach(int i in data.constantTiles) { tiles[i].CanBeChanged = false; }
    }

    public IEnumerator<GameBoard_Save> GetEnumerator() { throw new System.NotImplementedException(); }

    IEnumerator IEnumerable.GetEnumerator() { throw new System.NotImplementedException(); }

    public bool Save(string fileName, GameBoard_Save data)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("saving game board to: " + fileName); }
        // System.IO.File.WriteAllText(@fileName, JsonSerializer.Serialize(data));
        // System.IO.File.WriteAllText(@fileName, UnityEngine.JsonUtility.ToJson(data, true));
        System.IO.File.WriteAllText(@fileName, JsonConvert.SerializeObject(data));

        return true;
    }

    public GameBoard_Save Load(string fileName)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("loading board from: " + fileName); }
        // return JsonSerializer.Deserialize<GameBoard_Save>(System.IO.File.ReadAllText(@fileName));
        // return JsonUtility.FromJson<GameBoard_Save>(System.IO.File.ReadAllText(@fileName));
        return JsonConvert.DeserializeObject<GameBoard_Save>(System.IO.File.ReadAllText(@fileName));

    }
}
