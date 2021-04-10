using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.SceneManagement;


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
    public static TileManagerSO tileManager { get; private set; }

    private void Awake()
    {
        tileManager = tileManagerObject;
    }

    /// <summary>
    /// generate a tile map from node map, childed to parent
    /// </summary>
    public static void InitializeBoardTiles(GameBoard board, GameBoard_Save gbs, Transform parent)
    {
        // check and see if editor scene
        bool editor = SceneManager.GetActiveScene().name == "LevelBuilder"; // bad

        Vector2 size = board.Size;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int idx = (int)(size.x * y + x);
                TileType tt = gbs.tiles[idx];
                
                TileSO tileData = tileManager.allTiles.Find(
                        tile => tile.tileType == (editor ? TileType.editor : tt));
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
                if(editor)
                {
                    EditorTile ed = (EditorTile)newTile;
                    ed.setData = tileManager.allTiles.Find(
                        tile => tile.tileType == tt);
                    ed.idx = idx;
                }

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
    /// generate (size.x * size.y) blank map tiles
    /// </summary>
    public static List<MapTile> GenerateNewBlankTiles(Vector2 size, Transform parent, bool editor=false)
    {
        List<MapTile> tiles = new List<MapTile>();

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int idx = (int)(size.x * y + x);

                TileSO tileData = tileManager.allTiles.Find(
                        tile => tile.tileType == (editor ? TileType.editor : TileType.basic));
                GameObject toCopy = tileData.prefab;

                if (null == toCopy)
                {
                    Debug.LogError("could not find tile of type: " + TileType.basic.ToString());
                }

                Vector3 pos = new Vector3(x - size.x / 2, -(y + size.y / 2), 0);
                GameObject newTileObject = GameObject.Instantiate(
                    toCopy,
                    pos,
                    Quaternion.identity,
                    parent
                );

                MapTile newTile = newTileObject.GetComponent<MapTile>();
                newTile.AssignData(tileData, new List<MapTile>());
                newTile.CanBeChanged = TileType.basic == newTile.Data.tileType;
                newTile.placedByPlayer = false;

                newTileObject.name = idx + "-" + newTile.Data.tileType.ToString();

                tiles.Add(newTile);
            }
        }

        return tiles;
    }

}
