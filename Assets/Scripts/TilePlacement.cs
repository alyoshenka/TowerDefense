﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// display info of hovered tile??

public class TilePlacement : MonoBehaviour
{
    private static TilePlacement instance;
    public static TilePlacement Instance { get => instance; }
    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public DisplayTile display;
    public List<DisplayTile> tileSlots; // make dynamic later

    [SerializeField] private List<TileAllotment> availableTiles;

    public GameObject tileParent;

    private void Start()
    {
        availableTiles = new List<TileAllotment>();
        for(int i = 0; i < tileSlots.Count; i++)
        {
            availableTiles.Add(tileSlots[i].Allottment);
        }
    }

    public void SelectTile(DisplayTile newTile)
    {
        DisplayTile.SelectedTile?.DeselectTile();
        newTile?.SelectTile();
        UpdateDisplay();
    }

    public void GiveTiles(List<TileAllotment> tiles)
    {
        availableTiles = tiles;
        // BadUpdateDisplay(tiles);
    }

    private void BadUpdateDisplay(List<TileAllotment> tiles)
    {
        Debug.Assert(tiles.Count == availableTiles.Count);

        for(int i = 0; i < tiles.Count; i++)
        {
            tileSlots[i].SetAssociatedTile(tiles[i]);
        }
    }

    private void UpdateDisplay()
    {
        if(null == DisplayTile.SelectedTile)
        {
            display.selectButton.image.color = Color.white;
            display.quantityText.text = "x 0";
            display.buildCostText.text = "0";
        }
        else
        {
            display.selectButton.image.color = DisplayTile.SelectedTile.defaultColor;
            display.quantityText.text = DisplayTile.SelectedTile.quantityText.text;
            display.buildCostText.text = DisplayTile.SelectedTile.buildCostText.text;
        }
    }

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
        newTile.placedByPlayer = true;

        if (clickedTile.placedByPlayer) { ReturnTile(clickedTile); }

        if (!DisplayTile.SelectedTile.Available) { DisplayTile.ClearSelection(); } // inelegant but whatever

        UpdateDisplay();

        clickedTile.Destroy();
    }

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
}