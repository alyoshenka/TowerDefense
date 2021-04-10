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

    public TileSO Tile { get => Data; } // get associated tile

    [Tooltip("push to select this tile")] public Button selectButton;
    [Tooltip("display image holder")] public Image showImage;
    [Tooltip("tile alloment holder")] public TMP_Text quantityText;
    [Tooltip("build cost holder")] public TMP_Text buildCostText;
    [Tooltip("selection animator")] public Animator animator;

    // ToDo: get rid of later
    [HideInInspector] public Color defaultColor = Color.white; // default tile color
    [HideInInspector] public Color displayColor; // tile display color

    private int allotment;
    public int Allotment
    {
        set
        {
            allotment = value;
            UpdateDisplay(); ;
        }
    }
    public bool Available { get => allotment > 0; } // do we want this?

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

        defaultColor = Color.white;
        displayColor = defaultColor;

        DeselectTile();

        SelectedTile = null;

        UpdateDisplay();
    }

    public override void AssignData(TileSO newData, List<MapTile> newConnections = null)
    {
        base.AssignData(newData, newConnections);

        showImage.sprite = newData.displayImage;
        UpdateDisplay();
    }

    /// <summary>
    /// select a tile for placing
    /// </summary>
    public void SelectTile()
    {
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
            if (null != Debugger.Instance && Debugger.Instance.TileSelect) { Debug.Log("selecting " + Data.name); }
        }

        animator.SetTrigger("ClickDisplayTile");

        UpdateDisplay();
    }

    /// <summary>
    /// deseelct tile for placing
    /// </summary>
    public void DeselectTile(bool grayOut = false)
    {
        displayColor = grayOut ? Color.gray : defaultColor;
        UpdateDisplay();
        if (null != Debugger.Instance && Debugger.Instance.TileSelect) { Debug.Log("deselecting " + Data.name); }
    }

    /// <summary>
    /// take a tile from the allotment
    /// </summary>
    public void TakeTile()
    {
        allotment--;
        if(allotment == 0) { SetUnavailable(); }
        UpdateDisplay();

        if (Debugger.Instance.TileMessages) { Debug.Log(Data.tileType.ToString() + ": " + allotment); }
    }

    /// <summary>
    /// update UI display text
    /// </summary>
    public void UpdateDisplay()
    {
        quantityText.text = "x " + allotment.ToString();
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
