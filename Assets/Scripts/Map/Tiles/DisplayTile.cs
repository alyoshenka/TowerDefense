using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// display for a tile that can be placed
/// </summary>
public class DisplayTile : MapTile
{
    public static DisplayTile SelectedTile { get; private set; } // currently selected tile

    [SerializeField] [Tooltip("how many of this tile that are allowed")] 
    private TileAllotment allotment;
    public TileSO Tile { get => allotment.tile; } // get associated tile
    public TileAllotment Allottment { get => allotment; } // get tile allotment

    [Tooltip("push to select this tile")] public Button selectButton;
    [Tooltip("display image holder")] public Image showImage;
    [Tooltip("tile alloment holder")] public TMP_Text quantityText;
    [Tooltip("build cost holder")] public TMP_Text buildCostText;
    [Tooltip("selection animator")] public Animator animator;

    // ToDo: get rid of later
    [HideInInspector] public Color defaultColor = Color.white; // default tile color
    [HideInInspector] public Color displayColor; // tile display color

    public bool Available { get => null != allotment && allotment.count > 0; } // get if tile is available to place

    /// <summary>
    /// clear tile selection
    /// </summary>
    public static void ClearSelection()
    {
        SelectedTile?.DeselectTile();
        SelectedTile = null;
    }

    protected override void Awake()
    {
        // base.Start();

        selectButton.onClick.AddListener(() => TilePlacement.Instance.SelectTile(this));

        ColorBlock colorVars = selectButton.colors;
        colorVars.highlightedColor = Color.gray;
        selectButton.colors = colorVars;

        defaultColor = showImage.color;
        displayColor = defaultColor;

        DeselectTile();

        SelectedTile = null;

        UpdateDisplay();
    }

    /// <summary>
    /// select a tile for placing
    /// </summary>
    public void SelectTile()
    {
        if (!Available)
        {
            SelectedTile = null;
            return;
        } // something else??

        // check for bad logic
        // I think it's okay

        if (this == SelectedTile)
        {
            SelectedTile = null;
            DeselectTile();
        }
        else
        {
            SelectedTile = this;
            displayColor = Color.yellow;
        }

        animator.SetTrigger("ClickDisplayTile");

        UpdateDisplay();
    }

    /// <summary>
    /// deseelct tile for placing
    /// </summary>
    public void DeselectTile()
    {
        displayColor = Available ? defaultColor : Color.gray;
        UpdateDisplay();
    }

    /// <summary>
    /// set tile allotmet for associated tile
    /// </summary>
    public void SetAssociatedTile(TileAllotment allot)
    {
        allotment = allot;

        defaultColor = allot.tile.displayColor;
        displayColor = defaultColor;
        quantityText.text = "x " + allot.count.ToString();
        buildCostText.text = allot.tile.buildCost.ToString();
    }

    /// <summary>
    /// take a tile from the allotment
    /// </summary>
    public void TakeTile()
    {
        allotment.count--;
        if(!Available) { SetUnavailable(); }
        UpdateDisplay();
    }

    /// <summary>
    /// update UI display text
    /// </summary>
    public void UpdateDisplay()
    {
        quantityText.text = "x " + allotment?.count.ToString();
        showImage.color = displayColor;
        // other things
    }

    /// <summary>
    /// show tile available to place
    /// </summary>
    public void SetAvailable()
    {
        displayColor = defaultColor;
        DeselectTile();
    }

    /// <summary>
    /// show tile unavailable to place
    /// </summary>
    public void SetUnavailable()
    {       
        DeselectTile(); // is this really necessary
    }

    /// <summary>
    /// visual indication that there are unused tiles left
    /// </summary>
    public void IndicateUnused() { animator.SetTrigger("TileUnused"); }  

    public override void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
