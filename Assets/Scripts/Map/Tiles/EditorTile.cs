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
        MapEditor.Instance.EditorTileClick(this);

        displayColor = TileData.FindByType(Type).DisplayColor;
    }

    protected override void OnDrawGizmos()
    { 
        Gizmos.color = this == currentHover ? MapEditor.color : displayColor;
        Gizmos.DrawCube(transform.position, this == currentHover 
            ? Vector3.one : new Vector3(0.9f, 0.9f, 0.9f));

        if (showConnections)
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position;
            foreach(PathNode node in MapEditor.Instance.Board.FindAssociatedNode(this).connections)
            {
                Vector3 end = MapEditor.Instance.Board.FindAssociatedTile(node).transform.position;
                Gizmos.DrawLine(origin, end);
            }
        }
    }

    public override void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
