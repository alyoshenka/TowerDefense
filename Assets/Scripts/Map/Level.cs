using System.Collections;
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

    public List<TileAllotment_Save> tileAllotment;
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
public class Level : ISaveable<Level_Save>, IRecyclable
{
    public static readonly string ext = ".level";

    [Tooltip("level specific data")] [SerializeField]
    private LevelData levelData;
    [Tooltip("assiciated game board")] [SerializeField]
    private Board board;
    [Tooltip("tiles that can be placed")] [SerializeField]
    private List<TileAllotment> allottedTiles;
    [Tooltip("all enemies in this level")] [SerializeField]
    private List<HostileAgent> allEnemies;
      
    public Board Board { get => board; }
    public List<TileAllotment> AllottedTiles { get => allottedTiles; } 
    public string Name { get => levelData.name; set => levelData.name = value; }
    public int Number { get => levelData.number; }

    // public void InitializeLevel(int level) {} ??
    // where to put algorithm???
    // hardcode levels???

    // ToDo: change level creation implementation -> procgen or grab from list

    public Level(LevelData _levelData, Board _board)
    {
        levelData = _levelData;
        board = _board;

        allEnemies = new List<HostileAgent>(); // initialize empty
    }

    public Level(Level_Save _levelSave)
    {
        FromLoad(_levelSave);

        Debug.Assert(null != board);
    }

    /// <summary>
    /// adds enemy to list
    /// </summary>
    /// <returns>whether enemy successfully added</returns>
    public bool AddEnemy(HostileAgent agent)
    {
        if (allEnemies.Contains(agent)) { Debug.Assert(false); }
        allEnemies.Add(agent);
        return true;
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

    /// <summary>
    /// clear all tiles
    /// </summary>
    public void Recycle()
    {
        board.Recycle();
        board = null;
    }

    /// <returns>if the win condition(s) have been met</returns>
    public bool LevelWin()
    {
        return WinCondition.defeatAllEnemies == levelData.winCon && allEnemies.Count == 0; // ToDo: implement support for different win conditions
    }

    #region IO
    public Level_Save ToSave()
    {       
        Level_Save ret = new Level_Save
        {
            data = levelData,
            boardName = board.name
        };

        List<TileAllotment_Save> tas = new List<TileAllotment_Save>();
        foreach(TileAllotment ta in allottedTiles) { tas.Add(new TileAllotment_Save { type = ta.tile.tileType, count = ta.count }); }
        ret.data.tileAllotment = tas;

        return ret;
    }

    public void FromLoad(Level_Save data)
    {
        levelData = data.data;
        board = Board.LoadBoard(LevelBuilder.saveDir + data.boardName + Board.ext);

        List<TileAllotment> ta = new List<TileAllotment>();
        foreach(TileAllotment_Save tas in data.data.tileAllotment)
        {
         
            ta.Add(new TileAllotment(
                (TileSO)((ICollectionManager<TileType>)MapGenerator.tileManager).Get(tas.type), 
                tas.count));
        }
        allottedTiles = ta;
        TilePlacement.Instance?.AllotTiles(allottedTiles);

        allEnemies = new List<HostileAgent>();
        Debug.Assert(null != board);
    }

    public bool Save(string fileName, Level_Save data)
    {
        return Save_s(fileName, data);
    }

    // get around static restriction
    public static bool Save_s(string fileName, Level_Save data)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("saving level to: " + fileName); }
        System.IO.File.WriteAllText(@fileName, JsonConvert.SerializeObject(data));
        return true;
    }

    public Level_Save Load(string fileName)
    {
        return Load_s(fileName);
    }

    // get around static restriciton
    public static Level_Save Load_s(string fileName)
    {
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("loading level data from: " + fileName); }
        return JsonConvert.DeserializeObject<Level_Save>(System.IO.File.ReadAllText(@fileName));
    }

    public IEnumerator<Level_Save> GetEnumerator() { throw new System.NotImplementedException(); }

    IEnumerator IEnumerable.GetEnumerator() { throw new System.NotImplementedException(); }

    #endregion
}

/// <summary>
/// tiles allowed in level
/// </summary>
[System.Serializable]
public class TileAllotment
{
    [Tooltip("tile object")] public TileSO tile; 
    [Tooltip("number of given tiles allowed")] public int count;

    public TileAllotment(TileSO _tile, int _count) { tile = _tile; count = _count; }

    public void Decr() { count--; }
}

/// <summary>
/// basic data of tile allotment
/// </summary>
public class TileAllotment_Save
{
    public TileType type;
    public int count;
}

/// <summary>
/// number of enemies in level
/// </summary>
[System.Serializable]
public class EnemyPack
{
    [Tooltip("enemy object")] public EnemyType enemy;
    [Tooltip("number allotted")] public int count;
    [Tooltip("spawn reset time (s)")] public float buildupTime;
}

