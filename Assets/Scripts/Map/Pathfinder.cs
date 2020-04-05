using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{

    // weight paths that are closer to as-the-crow-flies paths


    public static FoundPath DjikstrasPath(PathNode _start, PathNode _goal, NodeMap nodeMap)
    {
        return RunDjikstras(_start, _goal, nodeMap);
    }

    // make with board/tilemap
    static FoundPath RunDjikstras(PathNode _start, PathNode _goal, NodeMap nodeMap)
    {
        // setup
        PathNode currentNode = _start;
        currentNode.calculatedCost = 0;
        List<PathNode> unvisitedNodes = new List<PathNode>(nodeMap.Nodes);

        while (unvisitedNodes.Count > 0 && currentNode != _goal)
        {
            unvisitedNodes.Remove(currentNode);

            Debug.Assert(currentNode.connections.Count <= 4); // take out later

            // all neighbors
            foreach (PathNode neighbor in currentNode.connections)
            {
                if(neighbor.calculatedCost > 1000)
                {
                    float newCost = currentNode.calculatedCost + neighbor.TraversalCost;
                    if (newCost < neighbor.calculatedCost)
                    {
                        neighbor.calculatedCost = newCost;
                        neighbor.previousNode = currentNode;
                    }
                }               
            }

            // next current
            currentNode = unvisitedNodes[0];
            foreach (PathNode currentCheck in unvisitedNodes)
            {
                if (currentCheck.calculatedCost < currentNode.calculatedCost)
                {
                    currentNode = currentCheck;
                }
            }
        }

        List<PathNode> _path = GeneratePath(_start, _goal, false);
        FoundPath toReturn = new FoundPath
        {
            start = _start,
            goal = _goal,
            path = _path
        };

        // cleanup
        nodeMap.ResetNodes();

        return toReturn;
    }

    static List<PathNode> GeneratePath(PathNode _start, PathNode _goal, bool addStart = true)
    {
        PathNode currentNode = _goal; // check

        List<PathNode> path = new List<PathNode>();
        do
        {
            path.Insert(0, currentNode);
            Debug.Assert(currentNode.previousNode != null);
            currentNode = currentNode.previousNode;
        } while (currentNode != _start);
        if (addStart) { path.Add(currentNode); }

        Debug.Assert(currentNode == _start); // assert path (change later)
        return path;
    } 
}
