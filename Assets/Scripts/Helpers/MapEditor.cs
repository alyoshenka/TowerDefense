﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance { get; private set; } // singleton instance

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    [Tooltip("default placing tile")] public GameObject defaultTile;
    [Tooltip("file to save level")] public string fileName = "basicLevel";
    [SerializeField] [Tooltip("(width, height) of grid")] public Vector2 defaultGridSize;
    [Tooltip("show grid of (w, h) size")] public bool visualizeGrid;
    [Tooltip("available tiles")] public List<TileData> tiles;

    [Tooltip("save level to file")] public bool save;
    [Tooltip("load level from file")] public bool load;

    public GameBoard Board { get; private set; } // singleton board instance

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
        Board = new GameBoard(nodeMap, tileMap);
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
            SaveBoard(Board, fileName);
            save = false;
        }

        if (load)
        {
            Debug.Log("load from " + fileName);
            LoadNewData(fileName);
            load = false;
        }
        
    }

    /// <summary>
    /// save board to file
    /// </summary>
    /// <param name="board">board to save</param>
    /// <param name="fileName">filename to use</param>
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
        Board.Destroy();
        NodeMap newNodes = LoadNodeMap(fileName);
        TileMap newTiles = MapGenerator.GenerateTileMap(newNodes, transform);
        Board = new GameBoard(newNodes, newTiles);
    }

    public void EditorTileClick(EditorTile tile)
    {
        bool isWall = tile.Type == TileType.wall || currentData.Type == TileType.wall 
            || tile.Type == TileType.turret || currentData.Type == TileType.turret;
        if (currentData.Type == TileType.goal)
        {          
            Board.AssignGoal(Board.FindAssociatedNode(tile));
        }
        Board.AssignNewData(tile, currentData, true); // this
        if (isWall) { Board.nodeMap = MapGenerator.AddNodeConnections(Board.nodeMap); }  
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

    private Color_S displayColor; // color in display (map editor)
    public Color DisplayColor { get => displayColor.ToColor(); set => displayColor = new Color_S(value); } // get serializable display color

    [SerializeField] [Tooltip("tile type")] private TileType type;
    public TileType Type { get => type; } // get tile type
    [SerializeField] [Tooltip("cost to place this tile")] private int buildCost;
    public int BuildCost { get => buildCost; } // get build cost
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
