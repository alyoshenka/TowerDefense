using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public float waitTime = 0.1f;

    // weight paths that are closer to as-the-crow-flies paths

    public static FoundPath DjikstrasPath(MapTile _start, MapTile _goal, List<MapTile> allTiles)
    {
        // something should happen here???

        return RunDjikstras(_start, _goal, allTiles); 
    }

    // make with board/tilemap
    static FoundPath RunDjikstras(MapTile _start, MapTile _goal, List<MapTile> allTiles)
    {
        // setup
        MapTile currentNode = _start;
        currentNode.calculatedCost = 0;
        List<MapTile> unvisitedNodes = new List<MapTile>(allTiles);

        while (unvisitedNodes.Count > 0 && currentNode != _goal)
        {
            unvisitedNodes.Remove(currentNode);

            Debug.Assert(currentNode.Connections.Count <= 4); // take out later

            // all neighbors
            foreach (MapTile neighbor in currentNode.Connections)
            {
                if(neighbor.calculatedCost > 1000000)
                {
                    int newCost = currentNode.calculatedCost + neighbor.Data.traversalCost;
                    if (newCost < neighbor.calculatedCost)
                    {
                        neighbor.calculatedCost = newCost;
                        neighbor.previousTile = currentNode;
                    }
                }               
            }

            // next current
            currentNode = unvisitedNodes[0];
            foreach (MapTile currentCheck in unvisitedNodes)
            {
                if (currentCheck.calculatedCost < currentNode.calculatedCost)
                {
                    currentNode = currentCheck;
                }
            }
        }

        List<MapTile> _path = GeneratePath(_start, _goal, false);
        FoundPath toReturn = new FoundPath
        {
            start = _start,
            goal = _goal,
            path = _path
        };

        // cleanup
        foreach(MapTile tile in allTiles) { tile.ResetPathfinding(); }

        return toReturn;
    } 

    public static List<MapTile> GeneratePath(MapTile _start, MapTile _goal, bool addStart = true)
    {
        MapTile currentNode = _goal; // check

        List<MapTile> path = new List<MapTile>();
        do
        {
            path.Insert(0, currentNode);
            Debug.Assert(currentNode.previousTile != null);
            currentNode = currentNode.previousTile;
        } while (currentNode != _start);
        if (addStart) { path.Add(currentNode); }

        Debug.Assert(currentNode == _start); // assert path (change later)
        return path;
    } 
}


