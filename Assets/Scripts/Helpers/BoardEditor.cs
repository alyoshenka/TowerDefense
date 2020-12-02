using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardEditor : MonoBehaviour
{
    public static BoardEditor Instance { get; private set; }

    public GameBoard board;

    public string currentPlaceType = "default"; // ToDo: add UI
    public Color color; // not sure

    public TileManagerSO availableTiles;
    [SerializeField] private TileSO currentTile;

    int tileIdx;

    private void Awake() { Instance = this; } 

    private void Start()
    {
        tileIdx = 0;
        SetCurrentTile(tileIdx);
    }

    private void SetCurrentTile(int idx)
    {
        Debug.Assert(idx >= 0 && idx < availableTiles.Count);
        currentTile = availableTiles.allTiles[idx];
        color = currentTile.displayColor;
        currentPlaceType = currentTile.tileType.ToString();
    }

    private void Update()
    {
        float delta = Input.mouseScrollDelta.y;
        if(delta != 0)
        {
            if(delta > 0)
            {
                tileIdx++;
                if(tileIdx > availableTiles.Count - 1)
                {
                    tileIdx = 0;
                }
            }
            else
            {
                tileIdx--;
                if(tileIdx < 0) { tileIdx = availableTiles.Count - 1; }
            }

            SetCurrentTile(tileIdx);
        }

        if (Input.GetMouseButtonDown(0) && null != EditorTile.CurrentHover)
        {
            TileData td = availableTiles.allTiles[tileIdx].GenerateTileData();
            EditorTile.CurrentHover.AssignData(td, true);
        }        
    }

    public void EditorTileClick(EditorTile tile)
    {
        bool isWall = tile.Type == TileType.wall || currentTile.tileType == TileType.wall 
            || tile.Type == TileType.turret || currentTile.tileType == TileType.turret;
        if (currentTile.tileType == TileType.goal)
        {          
            board.AssignGoal(board.FindAssociatedNode(tile));
        }
        board.AssignNewData(tile, currentTile.GenerateTileData(), true); // this
        
        // if (isWall) { board.nodeMap = MapGenerator.AddNodeConnections(board.nodeMap); }  
    }
}

/// <summary>
/// serializable color
/// </summary>
[System.Serializable]
public struct Color_S
{
    float r, g, b, a;

    public Color_S(Color c)
    {
        r = c.r;
        g = c.g;
        b = c.b;
        a = c.a;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, a);
    }
}

/// <summary>
/// represents all needed data for a single game tile
/// </summary>
[System.Serializable]
public struct TileData
{
    private static List<TileData> presets; // all available tile presets
    public static List<TileData> Presets
    {
        get
        {
            AssertPresetsInitialized();
            return presets;
        }
    }

    public Color_S displayColor; // color in display (map editor)

    [Tooltip("tile type")] public TileType type;
    public TileType Type { get => type; } // get tile type
    [Tooltip("cost to place this tile")] public int buildCost;
    public int index; // honestly kinda not sure? ToDo: write better comment

    [Range(1, 25)] [Tooltip("cost/speed to traverse")] public int traversalCost;

    // tile presets
    #region Presets 
    public static TileData Basic { get => new TileData(TileType.basic, Color.gray, 0, 1); }
    public static TileData Dirt { get => new TileData(TileType.dirt, Color.yellow, 5, 2); }
    public static TileData Grass { get => new TileData(TileType.grass, Color.green, 10, 3); }
    public static TileData Water { get => new TileData(TileType.water, Color.blue, 50, 10); }
    public static TileData Wall { get => new TileData(TileType.wall, Color.black, 200, 10000); }
    public static TileData Goal { get => new TileData(TileType.goal, new Color(1, 0, 1, 1), 0, 0); }
    public static TileData Enemy { get => new TileData(TileType.enemy, Color.red, 0, 0); }
    public static TileData Turret { get => new TileData(TileType.turret, new Color(1, 0.5f, 0, 1), 500, 10000); }
    #endregion

    /// </summary>
    /// <param name="_type">tile type</param>
    /// <param name="_color">tile color</param>
    /// <param name="_build">build cost</param>
    /// <param name="_traverse">traversal cost</param>
    public TileData(TileType _type, Color _color, int _build, int _traverse)
    {
        type = _type;
        displayColor = new Color_S(_color);
        buildCost = _build;
        index = -1;
        traversalCost = _traverse;
    }

    /// <summary>
    /// assert presets are initialized, if not, initializes them
    /// </summary>
    private static void AssertPresetsInitialized()
    {
        if(null == presets)
        {
            presets = new List<TileData>();
            presets.Add(Basic);
            presets.Add(Dirt);
            presets.Add(Grass);
            presets.Add(Water);
            presets.Add(Wall);
            presets.Add(Goal);
            presets.Add(Enemy);
            presets.Add(Turret);
        }
    }

    /// <summary>
    /// find a preset tile by given type
    /// </summary>
    public static TileData FindByType(TileType _type)
    {
        AssertPresetsInitialized();
        return presets.Find(tile => tile.type == _type);
    }
}

/// <summary>
/// a saved tile map
/// </summary>
[System.Serializable]
public class SaveMap
{
    public List<TileData> tileData; // all the tiles in the map
    public Vector2_S size; // map size (w, h)
}
