using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// debug node connections
/// </summary>
public class ConnectionShower : MonoBehaviour
{
    private static float offset = 0.1f;

    [Tooltip("current board")] public Board board;
    [Tooltip("show board connections")] public bool showConnections = true;
    [Tooltip("show one way connections")] public bool showOneWays = false;

    private void OnDrawGizmos()
    {
        if(null == board || !showConnections || !Application.isPlaying) { return; }

        Gizmos.color = Color.red;
        foreach(MapTile tile in board.tiles)
        {
            foreach(MapTile connection in tile.Connections)
            {
                if(null == connection) { continue; } // not sure why this fixes it
                Gizmos.DrawLine(tile.transform.position, connection.transform.position);
            }
        }

        if(showOneWays)
        {
            Gizmos.color = Color.blue;
            foreach(MapTile tile in board.tiles)
            {
                foreach(MapTile connection in tile.Connections)
                {
                    if (!connection.Connections.Contains(tile))
                    {
                        Gizmos.DrawLine(connection.transform.position, tile.transform.position);
                        Gizmos.DrawWireSphere(tile.transform.position, 0.2f); // around tile with connection
                    }
                }
            }
        }

        if(null == MapTile.currentHover) { return; }

        Gizmos.color = Color.green;
        foreach (MapTile neighbor in MapTile.currentHover.Connections)
        {
            Gizmos.DrawLine(
                    MapTile.currentHover.transform.position,
                    neighbor.transform.position);
        }
    }
}
