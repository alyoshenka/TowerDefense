using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardEditor : MonoBehaviour
{ 
    public static BoardEditor Instance { get; private set; }

    [HideInInspector] public Board board;

    public string currentPlaceType = "default"; // ToDo: add UI
    public Color color; // not sure

    public TileManagerSO availableTiles;
    [SerializeField] private TileSO currentTile;
    public List<EnemyTileData> enemyTileData;

    int tileIdx;

    public TileSO CurrentTile { get => currentTile; }

    private void Awake()
    {
        if(null != Instance) { Destroy(Instance); }
        Instance = this;
    }

    private void Start()
    {
        tileIdx = 0;
        SetCurrentTile(tileIdx);
    }

    private void SetCurrentTile(int idx)
    {
        Debug.Assert(idx >= 0 && idx < availableTiles.Count);
        currentTile = availableTiles.At(idx);
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
            EditorTile.CurrentHover.AssignData(currentTile);
        }        
    }

    public void EditorTileClick(EditorTile tile)
    {
        // if tile originally enemy tile
        if(tile.Data.tileType == TileType.enemy)
        {
            // remove associated enemy allotment from list
            enemyTileData.Remove(enemyTileData.Find(dat => dat.tileIdx == tile.idx));
        }


        if(currentTile.tileType == TileType.enemy)
        {
            EnemyTileData dat = new EnemyTileData{ tileIdx=tile.idx, enemyType=EnemyType.basic, count=0 };
            enemyTileData.Add(dat);
        }

        if(currentTile.tileType == TileType.wall) { tile.RemoveAllConnections(); }


        // ToDo: add connections
        // board.AssignNewData(tile, currentTile.GenerateTileData(), true); // this
        
        // if (isWall) { board.nodeMap = MapGenerator.AddNodeConnections(board.nodeMap); }  
    }
}

