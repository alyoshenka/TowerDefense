using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class MapEditor : MonoBehaviour
{
    private static MapEditor instance;
    public static MapEditor Instance { get => instance; }

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public GameObject defaultTile;
    public string fileName = "basicLevel";
    [SerializeField] public Vector2 defaultGridSize;
    public bool visualizeGrid;
    public List<TileData> tiles;

    public bool save;
    public bool load;

    GameBoard gameBoard;
    public GameBoard Board { get => gameBoard; }
    int tileIdx;

    TileData currentData;

    public static Color color;

    private void Start()
    {
        tiles = TileData.Presets;
        List<TileData> defaultTiles = new List<TileData>();
        for (int i = 0; i < defaultGridSize.x * defaultGridSize.y; i++)
        {
            defaultTiles.Add(TileData.Basic);
        }
        NodeMap nodeMap = MapGenerator.GenerateNodeMap(defaultTiles, defaultGridSize);
        TileMap tileMap = MapGenerator.GenerateTileMap(nodeMap, transform);
        gameBoard = new GameBoard(nodeMap, tileMap);
        tileIdx = 0;
        currentData = tiles[tileIdx];        
    }

    private void Update()
    {
        float delta = Input.mouseScrollDelta.y;
        if(delta != 0)
        {
            if(delta > 0)
            {
                tileIdx++;
                if(tileIdx > tiles.Count - 1)
                {
                    tileIdx = 0;
                }
            }
            else
            {
                tileIdx--;
                if(tileIdx < 0) { tileIdx = tiles.Count - 1; }
            }

            currentData = tiles[tileIdx];
            color = currentData.DisplayColor;
        }

        if (Input.GetMouseButtonDown(0) && null != EditorTile.CurrentHover)
        {
            EditorTile.CurrentHover.AssignData(currentData, true);
        }

        if (save)
        {
            Debug.Log("save to " + fileName);
            SaveBoard(gameBoard, fileName);
            save = false;
        }

        if (load)
        {
            Debug.Log("load from " + fileName);
            LoadNewData(fileName);
            load = false;
        }
        
    }

    public static void SaveBoard(GameBoard board, string fileName)
    {
        SaveMap toSave = MapGenerator.ExtractData(board);

        fileName = "Assets/Maps/" + fileName + ".txt";
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, toSave);
        stream.Close();
    }

    public static NodeMap LoadNodeMap(string fileName)
    {
        fileName = "Assets/Maps/" + fileName + ".txt";
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        SaveMap readData = (SaveMap)formatter.Deserialize(stream);
        stream.Close();

        NodeMap nodeMap = MapGenerator.GenerateNodeMap(readData.tileData, readData.size.ToVec2());

        return nodeMap;
    }

    private void LoadNewData(string fileName)
    {
        gameBoard.Destroy();
        NodeMap newNodes = LoadNodeMap(fileName);
        TileMap newTiles = MapGenerator.GenerateTileMap(newNodes, transform);
        gameBoard = new GameBoard(newNodes, newTiles);
    }

    public void EditorTileClick(EditorTile tile)
    {
        bool isWall = tile.Type == TileType.wall || currentData.Type == TileType.wall 
            || tile.Type == TileType.turret || currentData.Type == TileType.turret;
        if (currentData.Type == TileType.goal)
        {          
            gameBoard.AssignGoal(gameBoard.FindAssociatedNode(tile));
        }
        gameBoard.AssignNewData(tile, currentData, true); // this
        if (isWall) { gameBoard.nodeMap = MapGenerator.AddNodeConnections(gameBoard.nodeMap); }  
    }

    private void OnDrawGizmos()
    {
        if (visualizeGrid && !Application.isPlaying)
        {
            Gizmos.color = new Color(0, 0, 0, 0.1f);
            for (int y = 0; y < defaultGridSize.y; y++)
            {
                for (int x = 0; x < defaultGridSize.x; x++)
                {
                    Gizmos.DrawCube(
                        new Vector3(x - (defaultGridSize.x / 2), -y + (defaultGridSize.y / 2), 0), 
                        new Vector3(0.8f, 0.8f, 0.8f));
                }
            }
        }        
    }
}

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

[System.Serializable]
public struct TileData
{
    private static List<TileData> presets;
    public static List<TileData> Presets
    {
        get
        {
            AssertPresetsInitialized();
            return presets;
        }
    }

    private Color_S displayColor;
    public Color DisplayColor { get => displayColor.ToColor(); set => displayColor = new Color_S(value); }

    [SerializeField] private TileType type;
    public TileType Type { get => type; }
    [SerializeField] private int buildCost;
    public int BuildCost { get => buildCost; }
    public int index;

    [Range(1, 25)] public int traversalCost;

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

    public TileData(TileType _type, Color _color, int _build, int _traverse)
    {
        type = _type;
        displayColor = new Color_S(_color);
        buildCost = _build;
        index = -1;
        traversalCost = _traverse;
    }

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

    public static TileData FindByType(TileType _type)
    {
        AssertPresetsInitialized();
        return presets.Find(tile => tile.type == _type);
    }
}

[System.Serializable]
public class SaveMap
{
    public List<TileData> tileData;
    public Vector2_S size;
}
