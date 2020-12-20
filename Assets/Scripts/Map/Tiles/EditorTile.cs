using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// map tile used for board editing
/// </summary>
public class EditorTile : MapTile
{
    public static EditorTile CurrentHover { get => (EditorTile)currentHover; } // current hovered tile
    [Tooltip("color to display this tile")] public Color displayColor;

    protected override void Awake()
    {
        tileEnter += HoverEnter;
        tileExit += HoverExit;
        tileClick += TileSelected;
    }

    private void Start()
    {
        displayColor = BoardEditor.Instance.CurrentTile.displayColor;
    }

    protected override void OnDestroy()
    {
        tileEnter -= HoverEnter;
        tileExit -= HoverExit;
        tileClick -= TileSelected;
    }

    protected override void HoverEnter()
    {
        showConnections = true;
        currentHover = this;
    }
    protected override void HoverExit()
    {
        showConnections = false;
        if (this == currentHover) { currentHover = null; }
    }
    protected override void TileSelected()
    {
        BoardEditor.Instance.EditorTileClick(this);
        displayColor = BoardEditor.Instance.CurrentTile.displayColor;
    }

    protected override void OnDrawGizmos()
    { 
        Gizmos.color = this == currentHover ? BoardEditor.Instance.color : displayColor;
        Gizmos.DrawCube(transform.position, this == currentHover
            ? Vector3.one * 1.1f : Vector3.one * 0.95f);

        if (showConnections)
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position;
            foreach(MapTile t in connections)
            {
                Vector3 end = t.transform.position;
                Gizmos.DrawLine(origin - Vector3.forward, end - Vector3.forward);
            }
        }
    }

    public override void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
