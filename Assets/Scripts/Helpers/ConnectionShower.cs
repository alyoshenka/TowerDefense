using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionShower : MonoBehaviour
{
    private static ConnectionShower instance;
    public static ConnectionShower Instance { get => instance; }

    public GameBoard board;
    public bool showConnections;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void OnDrawGizmos()
    {
        if(null == board || !showConnections || !Application.isPlaying) { return; }

        Gizmos.color = Color.green;

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
    }
}
