using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// debug node connections
/// </summary>
public class ConnectionShower : MonoBehaviour
{
    public static ConnectionShower Instance { get; private set; } // singleton instance

    [Tooltip("current board")] public GameBoard board;
    [Tooltip("show board connections")] public bool showConnections;

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    private void OnDrawGizmos()
    {
        if(null == board || !showConnections || !Application.isPlaying) { return; }

        Gizmos.color = Color.green;

        /*
        foreach(PathNode node in board.nodeMap.Nodes)
        {
            foreach(PathNode neighbor in node.connections)
            {
                // im sorry

                Gizmos.DrawLine(
                    board.FindAssociatedTile(node).transform.position, 
                    board.FindAssociatedTile(neighbor).transform.position);
            }
        }
        */

        if(null == MapTile.currentHover) { return; }

        foreach(PathNode neighbor in board.FindAssociatedNode(MapTile.currentHover).connections)
        {
            Gizmos.DrawLine(
                    MapTile.currentHover.transform.position, 
                    board.FindAssociatedTile(neighbor).transform.position);
        }
    }
}
