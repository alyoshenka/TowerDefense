using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// map tile used for board editing
/// </summary>
public class EditorTile : MapTile
{
    public static EditorTile CurrentHover { get => (EditorTile)currentHover; } // current hovered tile
    [Tooltip("data this tile will represent")] public TileSO setData;

    public Color DisplayColor { get => null == setData ? Color.gray : setData.displayColor; }

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
        if (Debugger.Instance.TileHover) { Debug.Log("hover enter: " + name); }
    }
    protected override void HoverExit()
    {
        showConnections = false;
        if (this == currentHover) { currentHover = null; }
        if (Debugger.Instance.TileHover) { Debug.Log("hover exit: " + name); }

    }
    protected override void TileSelected()
    {
        BoardEditor.Instance.EditorTileClick(this);
        setData = BoardEditor.Instance.CurrentTile;
        gameObject.name = gameObject.name.Substring(0, gameObject.name.IndexOf("-") + 1) + setData.tileType.ToString();
        if (Debugger.Instance.TileSelect) { Debug.Log("select: " + name); }

    }

    protected override void OnDrawGizmos()
    { 
        Gizmos.color = this == currentHover ? BoardEditor.Instance.color : DisplayColor;
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
