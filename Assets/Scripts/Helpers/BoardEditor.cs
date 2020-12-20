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

    public TileSO CurrentTile { get => currentTile; }

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
            EditorTile.CurrentHover.AssignData(currentTile);
        }        
    }

    public void EditorTileClick(EditorTile tile)
    {
        // ToDo: add connections
        // board.AssignNewData(tile, currentTile.GenerateTileData(), true); // this
        
        // if (isWall) { board.nodeMap = MapGenerator.AddNodeConnections(board.nodeMap); }  
    }
}

