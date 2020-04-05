using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// serializable Vector2
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

// path from start to goal
public struct FoundPath
{
    public PathNode start;
    public PathNode goal;
    public List<PathNode> path;
}

[System.Serializable]
public class NodeMap
{
    private List<PathNode> nodes;
    public List<PathNode> Nodes { get => nodes; }
    private Vector2_S size;
    public Vector2 Size { get => size.ToVec2(); }

    private NodeMap() { }
    public NodeMap(List<PathNode> _nodes, Vector2 _size)
    {
        nodes = _nodes;
        size = new Vector2_S(_size);
    } 

    public void AddNode(PathNode node) { nodes.Add(node); }

    public void RemoveNode(PathNode node)
    {
        node.ClearConnections();
        Nodes.Remove(node);
    }

    public TileType NodeType(int index)
    {
        if(index >= nodes.Count) { Debug.Log(index); }
        return nodes[index].Type;
    }

    public void SetNodeType(PathNode node, TileType newType)
    {
        node.AssignData(TileData.FindByType(newType));
    }

    public void ResetNodes()
    {
        foreach(PathNode node in nodes)
        {
            node.calculatedCost = Mathf.Infinity;
            node.previousNode = null;
        }
    }
}

public class TileMap
{
    private List<MapTile> tiles;
    public List<MapTile> Tiles { get => tiles; }
    public Vector2 size;

    private TileMap() { }
    public TileMap(List<MapTile> _tiles, Vector2 _size)
    {
        tiles = _tiles;
        size = _size;
    }

    public void AddTile(MapTile tile) { tiles.Add(tile); }

    public void RemoveTile(MapTile tile) { tiles.Remove(tile); }

    public void ReplaceTile(MapTile oldTile, MapTile newTile)
    {
        int idx = tiles.IndexOf(oldTile);
        tiles.RemoveAt(idx);
        tiles.Insert(idx, newTile);
    }
}

// map type to prefab
[System.Serializable]
public class GameObjectTypeMap
{
    public GameObject tile;
    public TileType type;
}

// generate a grid of tiles and associated Nodes
public class MapGenerator : MonoBehaviour
{
    public List<GameObjectTypeMap> tileToTypeMap;
    private static List<GameObjectTypeMap> staticTileToTypeMap;

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

    public static TileMap GenerateTileMap(NodeMap nodeMap, Transform parent)
    {
        TileMap tileMap = new TileMap(new List<MapTile>(), nodeMap.Size);
        Vector2 size = nodeMap.Size;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                GameObject toCopy = staticTileToTypeMap.Find(
                        go => go.type == nodeMap.NodeType((int)(y * size.y + x))).tile;

                GameObject newTileObject = GameObject.Instantiate(
                    toCopy, 
                    new Vector3(x - (size.x / 2), -y + (size.y / 2), 0), 
                    Quaternion.identity, PlaceState.Instance.tileParent);

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

    public static GameObject FindAssociatedObject(TileType type)
    {
        return staticTileToTypeMap.Find(obj => obj.type == type).tile;
    }

    // basic, needs filename/randomness
    public static GameBoard GenerateBoard(int level, Vector2 size, Transform parent)
    {
        NodeMap nodeMap =  MapEditor.LoadNodeMap(level.ToString());
        TileMap tileMap = GenerateTileMap(nodeMap, parent);
        return new GameBoard(nodeMap, tileMap);
    }

    // connect the grid
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

        foreach (PathNode wall in nodeMap.Nodes.FindAll(node => node.Type == TileType.wall))
            { wall.ClearConnections(); }

        return nodeMap;
    }
}
