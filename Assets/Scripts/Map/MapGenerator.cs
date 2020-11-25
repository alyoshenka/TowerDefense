using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// serializable Vector2
/// </summary>
[System.Serializable]
public struct Vector2_S
{
    public float x, y;

    public Vector2_S(Vector2 vec)
    {
        x = vec.x;
        y = vec.y;
    }

    public Vector2 ToVec2() { return new Vector2(x, y); }
}

/// <summary>
/// path from start to goal
/// </summary>
public struct FoundPath
{
    public PathNode start; // starting node
    public PathNode goal; // ending node
    public List<PathNode> path; // list of nodes connection them
}

/// <summary>
/// a map of connected nodes
/// </summary>
[System.Serializable]
public class NodeMap
{
    private List<PathNode> nodes; // all nodes
    public List<PathNode> Nodes { get => nodes; } // get nodes
    private Vector2_S size; // size (w, h)
    public Vector2 Size { get => size.ToVec2(); }

    private NodeMap() { }
    public NodeMap(List<PathNode> _nodes, Vector2 _size)
    {
        nodes = _nodes;
        size = new Vector2_S(_size);
    } 

    /// <summary>
    /// add a node to the list
    /// </summary>
    public void AddNode(PathNode node) { nodes.Add(node); }

    /// <summary>
    /// remove a node from the list, clearing all connections
    /// </summary>
    public void RemoveNode(PathNode node)
    {
        node.ClearConnections();
        Nodes.Remove(node);
    }

    /// <summary>
    /// get type of nodes[index]
    /// </summary>
    public TileType NodeType(int index)
    {
        if(index >= nodes.Count) { Debug.Log(index); }
        return nodes[index].Type;
    }

    /// <summary>
    /// set type of node to default type values
    /// </summary>
    public void SetNodeType(PathNode node, TileType newType)
    {
        node.AssignData(TileData.FindByType(newType));
    }

    /// <summary>
    /// reset node values for pathfinding
    /// </summary>
    public void ResetNodes()
    {
        foreach(PathNode node in nodes)
        {
            node.calculatedCost = Mathf.Infinity;
            node.previousNode = null;
        }
    }
}

/// <summary>
/// map of tiles
/// </summary>
public class TileMap
{
    public List<MapTile> Tiles { get; } // all tiles
    public Vector2 size; // map size (w, h)

    private TileMap() { }
    public TileMap(List<MapTile> _tiles, Vector2 _size)
    {
        Tiles = _tiles;
        size = _size;
    }

    /// <summary>
    /// add a new tile
    /// </summary>
    public void AddTile(MapTile tile) { Tiles.Add(tile); }

    /// <summary>
    /// remove a tile
    /// </summary>
    public void RemoveTile(MapTile tile) { Tiles.Remove(tile); }

    /// <summary>
    /// replace old tile with new tile, preserving index
    /// </summary>
    public void ReplaceTile(MapTile oldTile, MapTile newTile)
    {
        int idx = Tiles.IndexOf(oldTile);
        Tiles.RemoveAt(idx);
        Tiles.Insert(idx, newTile);
    }
}

/// <summary>
/// map type to prefab
/// </summary>
[System.Serializable]
public class GameObjectTypeMap
{
    [Tooltip("tile gameobject")] public GameObject tile;
    [Tooltip("tile type")] public TileType type;
}

/// <summary>
/// generate a grid of tiles and associated nodes
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Tooltip("list of tile object/type options")] public List<GameObjectTypeMap> tileToTypeMap;
    public static List<GameObjectTypeMap> staticTileToTypeMap; // tile to type map

    private void Awake()
    {
        Debug.Assert(null == staticTileToTypeMap);        
        staticTileToTypeMap = tileToTypeMap;
        Debug.Assert(tileToTypeMap.Count == TileData.Presets.Count);
        for (int i = 0; i < tileToTypeMap.Count; i++)
        {
            staticTileToTypeMap[i].tile.GetComponent<MapTile>().AssignData(TileData.Presets[i], false);
        }
    }

    /// <summary>
    /// generate a node map from tile data, of size
    /// </summary
    public static NodeMap GenerateNodeMap(List<TileData> tiles, Vector2 size)
    {
        NodeMap nodeMap = new NodeMap(new List<PathNode>(), size);
        for (int i = 0; i < tiles.Count; i++)
        {
            TileData setData = tiles[i];
            setData.index = i;
            nodeMap.AddNode(new PathNode(setData));
        }
        nodeMap = AddNodeConnections(nodeMap);
        return nodeMap;
    }

    /// <summary>
    /// generate a tile map from node map, childed to parent
    /// </summary>
    public static TileMap GenerateTileMap(NodeMap nodeMap, Transform parent)
    {
        TileMap tileMap = new TileMap(new List<MapTile>(), nodeMap.Size);
        Vector2 size = nodeMap.Size;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                GameObject toCopy = staticTileToTypeMap.Find(
                        go => go.type == nodeMap.NodeType((int)(y * size.x + x))).tile;

                if(null == toCopy)
                {
                    Debug.LogError(
                        "could not find node of type: " 
                        + nodeMap.NodeType((int)(y * size.y + x)));
                }

                GameObject newTileObject = GameObject.Instantiate(
                    toCopy, 
                    new Vector3(x - (size.x / 2), -y + (size.y / 2), 0), 
                    Quaternion.identity, null);

                MapTile newTile = newTileObject.GetComponent<MapTile>();
                int idx = y * Mathf.RoundToInt(size.x) + x;
                newTile.Index = idx;
                tileMap.AddTile(newTile);
                newTile.AssignData(nodeMap.Nodes[idx].Data, true);

                newTileObject.name = newTile.Index.ToString() + "-" + newTile.Type;
            }
        }

        return tileMap;
    }

    /// <summary>
    /// get data to save from a gameboard
    /// </summary>
    /*
    public static SaveMap ExtractData(GameBoard gameBoard)
    {
        SaveMap map = new SaveMap();

        map.size = new Vector2_S(gameBoard.nodeMap.Size);

        List<TileData> tiles = new List<TileData>();
        foreach(PathNode node in gameBoard.nodeMap.Nodes)
        {
            tiles.Add(node.Data);
            Debug.Log(node.Type);
        }
        map.tileData = tiles;

        return map;
    }
    */

    /// <summary>
    /// get associated object from static map given type
    /// </summary>
    public static GameObject FindAssociatedObject(TileType type)
    {
        return staticTileToTypeMap.Find(obj => obj.type == type).tile;
    }

    // ToDo: basic, needs filename/randomness
    /// <summary>
    /// load a given level, child to parent
    /// </summary>
    /*
    public static GameBoard GenerateBoard(int level, Vector2 size, Transform parent)
    {
        NodeMap nodeMap =  MapEditor.LoadNodeMap(level.ToString());
        TileMap tileMap = GenerateTileMap(nodeMap, parent);
        return new GameBoard(nodeMap, tileMap);
    }
    */

    /// <summary>
    /// connect the given node map
    /// </summary>
    /// <param name="reset">reset node values for pathfinding?</param>
    public static NodeMap AddNodeConnections(NodeMap nodeMap, bool reset = true)
    {
        if (reset) { nodeMap.ResetNodes(); }
        if (nodeMap.Nodes.Count == 0) { return nodeMap; }

        int X = (int)nodeMap.Size.x;
        int Y = (int)nodeMap.Size.y;

        // top left
        nodeMap.Nodes[0].AddConnection(nodeMap.Nodes[1]); // right
        nodeMap.Nodes[0].AddConnection(nodeMap.Nodes[X]); // bottom

        // top right
        nodeMap.Nodes[X - 1].AddConnection(nodeMap.Nodes[X - 2]); // left
        nodeMap.Nodes[X - 1].AddConnection(nodeMap.Nodes[X * 2 - 1]); // bottom

        // bottom left
        nodeMap.Nodes[X * (Y - 1)].AddConnection(nodeMap.Nodes[X * (Y - 2)]); // top
        nodeMap.Nodes[X * (Y - 1)].AddConnection(nodeMap.Nodes[X * (Y - 1) + 1]); // right

        // bottom right
        nodeMap.Nodes[X * Y - 1].AddConnection(nodeMap.Nodes[X * (Y - 1) - 1]);
        nodeMap.Nodes[X * Y - 1].AddConnection(nodeMap.Nodes[X * Y - 2]);

        // top row
        for (int x = 1; x < X - 1; x++)
        {
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x - 1]); // left
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x + 1]); // right
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x + X]); // bottom
        }

        // bottom row
        for (int x = X * (Y - 1) + 1; x < X * Y - 1; x++)
        {
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x - 1]); // left
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x + 1]); // right
            nodeMap.Nodes[x].AddConnection(nodeMap.Nodes[x - X]); // top
        }

        // columns
        for (int y = 1; y < Y - 1; y++)
        {
            // left
            nodeMap.Nodes[y * X].AddConnection(nodeMap.Nodes[(y - 1) * X]); // top
            nodeMap.Nodes[y * X].AddConnection(nodeMap.Nodes[(y + 1) * X]); // bottom
            nodeMap.Nodes[y * X].AddConnection(nodeMap.Nodes[y * X + 1]); // right

            // right
            nodeMap.Nodes[(y + 1) * X - 1].AddConnection(nodeMap.Nodes[y * X - 1]); // top
            nodeMap.Nodes[(y + 1) * X - 1].AddConnection(nodeMap.Nodes[(y + 2) * X - 1]); // bottom
            nodeMap.Nodes[(y + 1) * X - 1].AddConnection(nodeMap.Nodes[(y + 1) * X - 2]); // left
        }

        // center
        for (int y = 1; y < Y - 1; y++)
        {
            for (int x = 1; x < X - 1; x++)
            {
                int idx = y * X + x;
                nodeMap.Nodes[idx].AddConnection(nodeMap.Nodes[idx - 1]); // left
                nodeMap.Nodes[idx].AddConnection(nodeMap.Nodes[idx + 1]); // right
                nodeMap.Nodes[idx].AddConnection(nodeMap.Nodes[idx - X]); // top
                nodeMap.Nodes[idx].AddConnection(nodeMap.Nodes[idx + X]); // bottom
            }
        }

        foreach (PathNode node in nodeMap.Nodes.FindAll(node => node.Type == TileType.wall 
            || node.Type == TileType.turret))
        { node.ClearConnections(); }

        return nodeMap;
    }
}
