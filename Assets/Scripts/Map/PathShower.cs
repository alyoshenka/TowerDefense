using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// pathfinder that visualizes the path
/// </summary>
public class PathShower : MonoBehaviour
{
    public float waitTime = 0.1f;

    public FoundPath pathSave;
    MapTile currentTile;
    List<MapTile> allTiles, unvsitedTiles;
    MapTile start, goal;

    public delegate void PathEvent();
    public PathEvent pathFound;

    public void VisualizePathfinding(MapTile _start, MapTile _goal, List<MapTile> _allTiles)
    {
        // setup
        start = _start;
        goal = _goal;

        currentTile = start;
        currentTile.tileStatus = TileStatus.current;
        currentTile.calculatedCost = 0;
        unvsitedTiles = new List<MapTile>(_allTiles);
        allTiles = new List<MapTile>(_allTiles);

        StartCoroutine(PathfindingAnimation());      
    }

    IEnumerator PathfindingAnimation()
    {
        if (null != currentTile.previousTile)
        {
            currentTile.previousTile.tileStatus = TileStatus.clear;
        }


        while (unvsitedTiles.Count > 0 && currentTile != goal)
        {
            unvsitedTiles.Remove(currentTile);

            Debug.Assert(currentTile.Connections.Count <= 4); // take out later

            // all neighbors
            foreach (MapTile neighbor in currentTile.Connections)
            {
                if (neighbor.calculatedCost > 1000000)
                {
                    int newCost = currentTile.calculatedCost + neighbor.Data.traversalCost;
                    if (newCost < neighbor.calculatedCost)
                    {
                        neighbor.calculatedCost = newCost;
                        neighbor.previousTile = currentTile;
                    }
                }
            }

            // currentTile.tileStatus = TileStatus.clear;
            foreach(MapTile tile in currentTile.Connections) { tile.tileStatus = TileStatus.none; }

            // next current
            currentTile = unvsitedTiles[0];
            foreach (MapTile currentCheck in unvsitedTiles)
            {
                if (currentCheck.calculatedCost < currentTile.calculatedCost)
                {
                    currentTile = currentCheck;
                    currentTile.tileStatus = TileStatus.current;
                    foreach(MapTile neighbor in currentTile.Connections) { neighbor.tileStatus = TileStatus.next; }
                }
            }

            yield return new WaitForSeconds(waitTime);
        }

        List<MapTile> path = Pathfinder.GeneratePath(start, goal, false);
        FoundPath toReturn = new FoundPath
        {
            start = start,
            goal = goal,
            path = path
        };

        // cleanup
        foreach (MapTile tile in allTiles) { tile.ResetPathfinding(); }

        pathSave = toReturn;

        pathFound?.Invoke();
    }
}
