using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ToDo: display info of hovered tile??
/// <summary>
/// manages placement of tiles in tile place state
/// </summary>
public class TilePlacement : MonoBehaviour
{
    public static TilePlacement Instance { get; private set; } // singleton instance
    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }

        GameStateManager.Instance.cleanupGameplay += (() => 
        { 
            DisplayTile.SelectedTile?.DeselectTile();
            display?.DeselectTile();
            UpdateDisplay();
        });
    }

    public bool AllUsed { get => availableTiles.FindAll(t => t.count > 0).Count == 0; } // all the tiles have been used

    [Tooltip("currently selected tile display")] public DisplayTile display;
    // ToDo: make dynamic later
    [Tooltip("display tile slots")] public List<DisplayTile> tileSlots; 

    [SerializeField] [Tooltip("tiles available for placing")] private List<TileAllotment> availableTiles;

    [Tooltip("object to parent new tiles to")] public GameObject tileParent;
    // ToDo(?): set old goal
    [Tooltip("base tile object to revert to")] public TileSO defaultTile;

    private void Start()
    {
        // for now, manual connection of display tiles and tile allotments
        Debug.Assert(tileSlots.Count == availableTiles.Count);
        for (int i = 0; i < tileSlots.Count; i++)
        {
            tileSlots[i].AssignData(availableTiles[i].tile);
            tileSlots[i].Allotment = availableTiles[i].count;
        }
    }

    /// <summary>
    /// select a new tile to place
    /// </summary>
    public void SelectTile(DisplayTile newTile)
    {
        DisplayTile.SelectedTile?.DeselectTile();
        newTile?.SelectTile();
        UpdateDisplay();
    }

    /// <summary>
    /// disperse a given tile allotment
    /// </summary>
    public void GiveTiles(List<TileAllotment> tiles)
    {
        availableTiles = tiles;
        // BadUpdateDisplay(tiles);
    }

    /// <summary>
    /// update display to show current tile allotment
    /// </summary>
    private void UpdateDisplay()
    {
        if(null == DisplayTile.SelectedTile)
        {
            display.selectButton.image.color = Color.white;
            display.selectButton.image.sprite = null;
            display.quantityText.text = "x 0";
            display.buildCostText.text = "0";
        }
        else
        {
            display.selectButton.image.color = DisplayTile.SelectedTile.defaultColor;
            display.selectButton.image.sprite = DisplayTile.SelectedTile.Data.displayImage;
            display.quantityText.text = DisplayTile.SelectedTile.quantityText.text;
            display.buildCostText.text = DisplayTile.SelectedTile.buildCostText.text;
        }
    }

    /// <summary>
    /// click (and select) a map tile
    /// </summary>
    public void ClickTile(MapTile clickedTile)
    {
        Debug.Assert(null != clickedTile);

        if (null == DisplayTile.SelectedTile)
        {
            Debug.Log("no tile selected");
            // other stuff maybe
            // eventually reset to default, right now not so important
            return;
        }

        if(!Available(DisplayTile.SelectedTile.Data))
        {
            Debug.Log("tile not available");
            return;
        }

        if (!clickedTile.CanBeChanged)
        {
            Debug.LogWarning("tile can't be changed");
            return;
        }       

        if (clickedTile.HasCost) { } // buy             

        // return old tile, then assign new tile
        if (clickedTile.placedByPlayer) { ReturnTile(clickedTile); }

        Debug.Assert(null != DisplayTile.SelectedTile.Tile);
        if (Debugger.Instance.TileMessages) { Debug.Log("setting tile to " + DisplayTile.SelectedTile.Tile.name); }

        // assign new tile if different, reset to default  if same
        if(clickedTile.Data == DisplayTile.SelectedTile.Tile)
        {
            clickedTile.AssignData(defaultTile);
            clickedTile.placedByPlayer = false;
        }
        else
        {
            // make new tile
            availableTiles.Find(tile => tile.tile == DisplayTile.SelectedTile.Data).Decr();
            DisplayTile.SelectedTile.TakeTile();

            clickedTile.AssignData(DisplayTile.SelectedTile.Tile);
            clickedTile.placedByPlayer = true;
        }

        if (!DisplayTile.SelectedTile.Available) { DisplayTile.ClearSelection(); } // inelegant but whatever

        UpdateDisplay();

        // don't destroy, data is not transferred instead
        // clickedTile.Destroy();
    }

    /// <summary>
    /// return a given map tile to allotment
    /// </summary>
    private void ReturnTile(MapTile returningTile)
    {
        if (Debugger.Instance.TileMessages) { Debug.Log("returning " + returningTile.Data.tileType.ToString()); }

        TileAllotment returnedAllot = 
            availableTiles.Find(tile => tile.tile == returningTile.Data);

        returnedAllot.count++;
        Debug.Log(returningTile.Data.tileType.ToString() + ": " + returnedAllot.count);

        DisplayTile availableSlot = tileSlots.Find(slot => slot.Tile.tileType == returningTile.Data.tileType);  // gross
        availableSlot.Allotment = returnedAllot.count; // not the best design
        availableSlot.SetAvailable();
        availableSlot.UpdateDisplay();

        UpdateDisplay();

        // return money
    }

    /// <summary>
    /// check (by tile so) if tile is available
    /// </summary>
    private bool Available(TileSO tileSO)
    {
        return availableTiles.Find(tile => tile.tile == tileSO).count > 0;
    }

    /// <summary>
    /// visualize unused tiles
    /// </summary>
    public void IndicateUnused() 
    { 
        foreach(DisplayTile tile in tileSlots)
        {
            if (Available(tile.Data)) { tile.IndicateUnused(); }
        }
    }
}
