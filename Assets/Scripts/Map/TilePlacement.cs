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
    }

    public bool AllUsed { get => tileSlots.FindAll(t => t.Available).Count == 0; } // all the tiles have been used

    [Tooltip("currently selected tile display")] public DisplayTile display;
    // ToDo: make dynamic later
    [Tooltip("display tile slots")] public List<DisplayTile> tileSlots; 

    [SerializeField] [Tooltip("tiles available for placing")] private List<TileAllotment> availableTiles;

    [Tooltip("object to parent new tiles to")] public GameObject tileParent;
    // ToDo(?): set old goal
    [Tooltip("honestly not sure")] public MapTile defaultTile; 

    private void Start()
    {
        availableTiles = new List<TileAllotment>();
        for(int i = 0; i < tileSlots.Count; i++)
        {
            availableTiles.Add(tileSlots[i].Allottment); // this needs to be changed to reset on game over
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
            display.selectButton.image.sprite = DisplayTile.SelectedTile.displayImage;
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
            return;
        }

        if(!DisplayTile.SelectedTile.Available)
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

        // make new tile
        DisplayTile.SelectedTile.TakeTile();
        MapTile newTile = PlaceState.Instance.Board.AssignNewTile(clickedTile, DisplayTile.SelectedTile.Tile);
        newTile.AddPlaceIndicators();
        newTile.placedByPlayer = true;

        if (clickedTile.placedByPlayer) { ReturnTile(clickedTile); }

        if (!DisplayTile.SelectedTile.Available) { DisplayTile.ClearSelection(); } // inelegant but whatever

        UpdateDisplay();

        clickedTile.Destroy();
    }

    /// <summary>
    /// return a given map tile to allotment
    /// </summary>
    private void ReturnTile(MapTile returningTile)
    {
        TileAllotment returnedAllot = 
            availableTiles.Find(allot => allot.tile.Type == returningTile.Type);

        returnedAllot.count++;

        DisplayTile availableSlot = tileSlots.Find(slot => slot.Allottment.tile.Type == returningTile.Type);  // gross
        availableSlot.SetAvailable();
        availableSlot.UpdateDisplay();

        UpdateDisplay();

        // return money
    }

    /// <summary>
    /// visualize unused tiles
    /// </summary>
    public void IndicateUnused() { foreach(DisplayTile tile in tileSlots.FindAll(t => t.Available)) { tile.IndicateUnused(); } }
}
