using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/// <summary>
/// path from start to goal
/// </summary>
public struct FoundPath
{
    public MapTile start; // starting node
    public MapTile goal; // ending node
    public List<MapTile> path; // list of nodes connection them
}

/// <summary>
/// generate tile gameobjects
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public TileManagerSO tileManagerObject;
    private static TileManagerSO tileManager;

    private void Awake()
    {
        tileManager = tileManagerObject;
    }

    /// <summary>
    /// generate a tile map from node map, childed to parent
    /// </summary>
    public static void InitializeBoardTiles(GameBoard board, GameBoard_Save gbs, Transform parent)
    {
        Vector2 size = board.Size;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int idx = (int)(size.x * y + x);
                TileType tt = gbs.tiles[idx];
                
                TileSO tileData = tileManager.allTiles.Find(
                        tile => tile.tileType == tt);
                GameObject toCopy = tileData.prefab;

                if(null == toCopy)
                {
                    Debug.LogError("could not find tile of type: " + tt.ToString());
                }
                
                GameObject newTileObject = GameObject.Instantiate(
                    toCopy, 
                    gbs.tilePositions[idx],
                    Quaternion.identity, 
                    parent
                );
             
                MapTile newTile = newTileObject.GetComponent<MapTile>();
                newTile.AssignData(tileData, new List<MapTile>());
                newTile.CanBeChanged = TileType.basic == newTile.Data.tileType;
                newTile.placedByPlayer = false;

                newTileObject.name = idx + "-" + newTile.Data.tileType.ToString();

                board.tiles.Add(newTile);
            }
        }

        // add connections
        if(null == gbs.tileConnections)
        {
            Debug.LogError("connection list is null");
            gbs.tileConnections = new List<int[]>();
        }
        if(gbs.tileConnections.Count != board.tiles.Count) { Debug.LogError("tiles != connections"); }
        else
        {
            for (int i = 0; i < gbs.tileConnections.Count; i++)
            {
                MapTile baseTile = board.tiles[i];
                int[] connectionList = gbs.tileConnections[i];
                for (int j = 0; j < connectionList.Length; j++)
                {
                    baseTile.AddConnection(board.tiles[connectionList[j]]);
                }
            }
        }
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

}
