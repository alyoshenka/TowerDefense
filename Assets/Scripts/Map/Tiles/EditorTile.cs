using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTile : MapTile
{
    public static EditorTile CurrentHover { get => (EditorTile)currentHover; }

    protected override void Start()
    {
        // base.Start();

        tileEnter += (() => 
        {
            showConnections = true;
            currentHover = this;
        });
        tileExit += (() => 
        {
            showConnections = false;
            if (this == currentHover) { currentHover = null; }
        });
        tileClick += (() =>
        {
            MapEditor.Instance.EditorTileClick(this);
        });
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = this == currentHover ? MapEditor.color : DisplayColor;
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
