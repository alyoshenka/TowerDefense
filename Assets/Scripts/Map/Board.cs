using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

/// <summary>
/// tile board
/// </summary>
[System.Serializable]
public class Board : ISaveable<GameBoard_Save>, IRecyclable
{
    public static readonly string ext = ".board";

    [Tooltip("board size (w, h)")] [SerializeField]
    private Vector2 size;
    public string name;
    [Tooltip("all tiles")]
    public List<MapTile> tiles;

    private List<int[]> tileConnections;

    public Vector2 Size { get => size; set => size = value; }
    private MapTile goalTile;
    public MapTile GoalTile 
    {
        get
        {
            if (null == goalTile)
            {
                goalTile = tiles.Find(t => t.Data.tileType == TileType.goal);
            }
            return goalTile;
        }
    } 
    public bool GoalAssigned { get => null != GoalTile; } // is this ok?

    private Board() { }

    public static Board LoadBoard(string fileName)
    {
        Board board = new Board();
        GameBoard_Save gbs = board.Load(fileName);
        board.FromLoad(gbs);
        return board;
    }

    /// <summary>
    /// delete all tiles
    /// </summary>
    public void Recycle()
    {
        foreach (MapTile tile in tiles) { tile.Recycle(); }
    }

    #region IO
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

        for (int i = 0; i < tiles.Count; i++)
        {
            MapTile t = tiles[i];
            gbs.tiles[i] = (t.Data.tileType == TileType.editor ? TileType.basic : t.Data.tileType);
            gbs.tilePositions[i] = t.transform.position;
            if (!t.CanBeChanged) { gbs.constantTiles.Add(i); }

            int[] connectionList = new int[t.Connections.Count];
            if (t.Connections.Count > 0)
            {
                for (int j = 0; j < t.Connections.Count; j++)
                {
                    connectionList[j] = tiles.IndexOf(t.Connections[j]);
                }
            }
            gbs.tileConnections.Add(connectionList);
        }

        if (0 == tiles.Count) // initialize array if no tiles yet
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    int i = (int)(y * size.x + x);
                    gbs.tilePositions[i] = new Vector3(x - size.x / 2, -y + size.y / 2, 0);
                }
            }
        }

        return gbs;
    }

    public void FromLoad(GameBoard_Save data)
    {
        name = data.name;
        size = data.size;
        tiles = new List<MapTile>();    
        MapGenerator.InitializeBoardTiles(this, data, null);
        foreach (int i in data.constantTiles) { tiles[i].CanBeChanged = false; }

        Debug.Assert(data.tileConnections.Count == tiles.Count);
        tileConnections = data.tileConnections;
        for(int i = 0; i < tiles.Count; i++)
        {
            MapTile tile = tiles[i];
            int[] connections = tileConnections[i];
            for(int j = 0; j < connections.Length; j++)
            {
                Debug.Assert(tiles.IndexOf(tile) != connections[j]);
                tile.AddConnection(tiles[connections[j]]);
            }
        }
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

    #endregion

    /// <summary>
    /// connect all tiles in a grid pattern (u, d, l, r)
    /// </summary>
    public void AddGridConnections()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            MapTile tile = tiles[i];

            int u = i - (int)size.x;
            int d = i + (int)size.x;
            int l = i - 1;
            int r = i + 1;
            if (u >= 0)
            {
                MapTile connection = tiles[u];
                tile.AddConnection(connection);
            }
            if (d < tiles.Count)
            {
                MapTile connection = tiles[d];
                tile.AddConnection(connection);
            }
            if (i % (int)size.x != 0)
            {
                MapTile connection = tiles[l];
                tile.AddConnection(connection);
            }
            if ((i + 1) % (int)size.x != 0)
            {
                MapTile connection = tiles[r];
                tile.AddConnection(connection);
            }
        }
    }

    public void ClearAllConnections()
    {
        foreach (MapTile tile in tiles) { tile.ClearConnections(); }
    }

    /// <summary>
    /// assert that all connections are two-way
    /// </summary>
    public void AssertTwoWayConnections()
    {
        foreach (MapTile tile in tiles)
        {
            foreach (MapTile connection in tile.Connections)
            {
                if (!connection.Connections.Contains(tile))
                {
                    Debug.LogWarning(connection.name + " connections does not contain " + tile.name);
                }
            }
        }
    }

    /// <summary>
    /// clear tiles, reset to blank using given size
    /// </summary>
    public void GenerateNewTiles()
    {
        foreach (MapTile tile in tiles)
        {
            tile.Recycle();
        }
        tiles.Clear();
        tiles = MapGenerator.GenerateNewBlankTiles(size, null, true);
    }
}

