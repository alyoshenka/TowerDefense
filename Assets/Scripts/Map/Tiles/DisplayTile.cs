using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayTile : MapTile
{
    private static DisplayTile selectedTile;
    public static DisplayTile SelectedTile { get => selectedTile; }

    [SerializeField] private TileAllotment allotment;
    public MapTile Tile { get => allotment.tile; }
    public TileAllotment Allottment { get => allotment; } 

    public Button selectButton;
    public Image showImage;
    public TMP_Text quantityText;
    public TMP_Text buildCostText;

    [HideInInspector] public Color defaultColor; // get rid of later
    [HideInInspector] public Color displayColor;

    public bool Available { get => null != allotment && allotment.count > 0; }

    public static void ClearSelection()
    {
        selectedTile?.DeselectTile();
        selectedTile = null;
    }

    protected override void Awake()
    {
        // base.Start();

        selectButton.onClick.AddListener(() => TilePlacement.Instance.SelectTile(this));

        ColorBlock colorVars = selectButton.colors;
        colorVars.highlightedColor = Color.blue;
        selectButton.colors = colorVars;

        defaultColor = showImage.color;
        displayColor = defaultColor;

        DeselectTile();

        selectedTile = null;

        UpdateDisplay();

        uniqueData = allotment.tile.Data;
    }

    public void SelectTile()
    {
        if (!Available)
        {
            selectedTile = null;
            return;
        } // something else??

        // check for bad logic
        // I think it's okay

        if (this == selectedTile)
        {
            selectedTile = null;
            DeselectTile();
        }
        else
        {
            selectedTile = this;
            displayColor = Color.yellow;
        }

        UpdateDisplay();
    }

    public void DeselectTile()
    {
        displayColor = Available ? defaultColor : Color.gray;
        UpdateDisplay();
    }

    public void SetAssociatedTile(TileAllotment allot)
    {
        allotment = allot;

        defaultColor = allot.tile.DisplayColor;
        displayColor = defaultColor;
        quantityText.text = "x " + allot.count.ToString();
        buildCostText.text = allot.tile.BuildCost.ToString();
    }

    public void TakeTile()
    {
        allotment.count--;
        if(!Available) { SetUnavailable(); }
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        quantityText.text = "x " + allotment?.count.ToString();
        showImage.color = displayColor;
        // other things
    }

    public void SetAvailable()
    {
        displayColor = defaultColor;
        DeselectTile();
    }

    public void SetUnavailable()
    {       
        DeselectTile(); // is this really necessary
    }

    public override void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
