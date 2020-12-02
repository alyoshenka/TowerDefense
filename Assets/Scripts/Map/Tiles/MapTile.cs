using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// tile "geography"
/// </summary>
public enum TileType
{
    basic,
    dirt,
    grass,
    water,
    wall,
    goal,
    enemy,
    turret
}

/// <summary>
/// a node with a type and connections
/// </summary>
public class PathNode
{
    private TileData uniqueData; // tile type data
    public TileData Data { get => uniqueData; } // get unique data
    public TileType Type { get => uniqueData.Type; } // get tile type
    public int Index { get => uniqueData.index; set => uniqueData.index = value; } // get tile index
    
    [HideInInspector] public float calculatedCost; // cost to get to this node

    public int TraversalCost { get => uniqueData.traversalCost; } // get traversal cost

    public PathNode previousNode; // determined when finding path
    public List<PathNode> connections; // all connected nodes

    /// <summary>
    /// remove node connection
    /// </summary>
    public void RemoveConnection(PathNode oldNode) { connections.Remove(oldNode); }

    /// <summary>
    /// add new node connection, asserting no duplicates
    /// </summary>
    public void AddConnection(PathNode newNode)
    {
        Debug.Assert(!connections.Contains(newNode));
        connections.Add(newNode);
    }

    private PathNode() { }

    /// <param name="setData">unique data</param>
    public PathNode(TileData setData)
    {
        connections = new List<PathNode>();
        calculatedCost = Mathf.Infinity;

        uniqueData = setData;

        Debug.Assert(TraversalCost == setData.traversalCost);
    }

    /// <summary>
    /// assign new unique data
    /// </summary>
    public void AssignData(TileData newData)
    {
        newData.index = uniqueData.index;
        uniqueData = newData;
    }

    /// <summary>
    /// replace this node's connections with the old node's connections
    /// remove old node's connections
    /// </summary>
    public void ReplaceConnections(PathNode oldNode) 
    {
        for(int i = 0; i < oldNode.connections.Count; i++)
        {
            PathNode neighbor = oldNode.connections[i];
            neighbor.RemoveConnection(oldNode);
            neighbor.AddConnection(this);
        }

        connections = new List<PathNode>(oldNode.connections);
        oldNode.ClearConnections();

        Debug.Assert(oldNode.connections.Count == 0);
    }
     
    /// <summary>
    /// make this tile not the goal tile
    /// </summary>
    public void RemoveFromGoal() { AssignData(TileData.Basic); }

    /// <summary>
    /// make this tile the goal tile
    /// </summary>
    public void AssignAsGoal() { AssignData(TileData.Goal); }

    /// <summary>
    /// remove all connetions to this node
    /// </summary>
    public void ClearConnections()
    {
        foreach (PathNode connection in connections) { connection.RemoveConnection(this); }
        connections.Clear();
    }
}

/// <summary>
/// a tile in the map, gameobject
/// </summary>
[System.Serializable]
public class MapTile : MonoBehaviour
{
    [SerializeField] [Tooltip("tile type data")] protected TileData uniqueData;
    public TileData Data { get => uniqueData; } // get unique data
    public TileType Type { get => uniqueData.Type; } // get tile type
    public Color DisplayColor { get => uniqueData.displayColor.ToColor(); } // get display color
    public int TraversalCost { get => uniqueData.traversalCost; } // get traversal cost
    public int BuildCost { get { return uniqueData.buildCost; } private set { } } // get build cost
    public int Index { get => uniqueData.index; set => uniqueData.index = value; } // get/set tile index
    

    public delegate void TileEnteredEvent();
    public event TileEnteredEvent tileEnter; // invoke on mouse hover enter
    public delegate void TileExitedEvent();
    public event TileExitedEvent tileExit; // invoke on mouse hover exit
    public delegate void TileClickedEvent();
    public event TileClickedEvent tileClick; // invoke on tile click
    public static MapTile currentHover; // the current tile being hovered over

    public bool GoalTile { get => uniqueData.Type == TileType.goal; } // get whether this is the goal tile

    // ToDo: not quite optimal but ok
    [SerializeField] [Tooltip("this tile can be edited")] protected bool canBeChanged = true;
    // ToDo: find better place
    [SerializeField] [Tooltip("tile UI display image")] public Sprite displayImage;
    [Tooltip("show tile/node connetions")] public bool showConnections;
    [Tooltip("this tile has been placed by the player")] public bool placedByPlayer;
    public bool CanBeChanged { get => canBeChanged || placedByPlayer;  set => canBeChanged = value; } // get if tile can be change
    public bool HasCost { get => BuildCost > 0; } // get if tile has a build cost    

    [HideInInspector] public TileStatus tileStatus = TileStatus.none; // pathfinding status   

    protected virtual void Awake()
    {
        // ok this might be kinda bad
        if (PlaceState.Instance && canBeChanged) 
        { 
            PlaceState.Instance.openPlace += AddPlaceIndicators;
            DefendState.Instance.openDefend += RemovePlaceIndicators;
        }
    }

    protected virtual void OnDestroy()
    {
        if(null == PlaceState.Instance) { return; }

        PlaceState.Instance.openPlace -= AddPlaceIndicators;
        DefendState.Instance.openDefend -= RemovePlaceIndicators;
    }

    #region Placement
    /// <summary>
    /// add interaction indicators for tile placement
    /// </summary>
    public void AddPlaceIndicators()
    {
        tileEnter += HoverEnter;
        tileExit += HoverExit;
        tileClick += TileSelected;
    }
    /// <summary>
    /// remove interaction indicators for tile placement
    /// </summary>
    public void RemovePlaceIndicators()
    {
        tileEnter -= HoverEnter;
        tileExit -= HoverExit;
        tileClick -= TileSelected;
    }
    /// <summary>
    /// action upon mouse hover enter
    /// </summary>
    protected virtual void HoverEnter()
    {
        transform.localScale *= 1.4f;
        currentHover = this;
    }
    /// <summary>
    /// action upon mouse hover exiy
    /// </summary>
    protected virtual void HoverExit()
    {
        transform.localScale /= 1.4f;
        if(this == currentHover) { currentHover = null; }
    }
    /// <summary>
    /// action upon tile selected
    /// </summary>
    protected virtual void TileSelected()
    {
        TilePlacement.Instance.ClickTile(this);
    }
    #endregion

    /// <summary>
    /// instantiate tile in place, assign/move data accordingly
    /// </summary>
    /// <param name="tileModel">tile model to copy</param>
    /// <returns>newly created map tile</returns>
    public MapTile InstantiateInPlace(MapTile tileModel)
    {
        GameObject copy = Instantiate(
            tileModel.gameObject, transform.position, transform.rotation, transform.parent);
        MapTile newTile = copy.GetComponent<MapTile>();
        newTile.AssignData(uniqueData, false); // use my data

        return newTile;
    }

    /// <summary>
    /// properly dispose of tile
    /// </summary>
    public void Destroy()
    {
        currentHover = null;
        if (Debugger.Instance.TileMessages) { Debug.Log("destroy " + name); }
        if (null != gameObject && null != this) { Destroy(gameObject); }
        else { Debug.LogError("something probably bad"); }
        // Destroy(this);
    }

    /// <summary>
    /// how the tile affects agents
    /// </summary>
    /// <param name="agent"> the agent on the tile </param>
    public virtual void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }

    #region Pathfinding Display Indicators
    /// <summary>
    /// mark as current tile (pathfinding display)
    /// </summary>
    public void IndicateCurrent() { tileStatus = TileStatus.current; }
    /// <summary>
    /// mark as next tile (pathfinding display)
    /// </summary>
    public void IndicateNext() { tileStatus = TileStatus.next; }
    /// <summary>
    /// mark tile as cleared (pathfinding stuff)
    /// </summary>
    public void IndicateCleared() { tileStatus = TileStatus.clear; }
    /// <summary>
    /// clear pathfinding stuff
    /// </summary>
    public void IndicateNone() { tileStatus = TileStatus.none; }
    #endregion

    protected virtual void OnDrawGizmos()
    { 
        Vector3 size = new Vector3(1, 1, 1);

        switch (tileStatus)
        {
            case TileStatus.current:
                Gizmos.color = Color.green;
                Gizmos.DrawCube(transform.position, size);
                break;
            case TileStatus.next:
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.position, size);
                break;
            case TileStatus.clear:
                Gizmos.color = Color.white;
                Gizmos.DrawCube(transform.position, size);
                break;
            default:
                break;
        }

        if (placedByPlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(1,1,1));
        }
    }

    // ToDo: is this the best way to be doign thie?

    private void OnMouseEnter() { tileEnter?.Invoke(); }

    private void OnMouseExit() { tileExit?.Invoke(); }

    private void OnMouseDown() { tileClick?.Invoke(); }

    /// <summary>
    /// assign new tile data
    /// </summary>
    /// <param name="preserveIndex">keep current index?</param>
    public void AssignData(TileData newData, bool preserveIndex)
    {
        if (preserveIndex) { newData.index = uniqueData.index; }
        uniqueData = newData;

        name = uniqueData.index + "-" + uniqueData.Type;

        if(Type != TileType.basic) { canBeChanged = false; } // maybe??
    }

    /// <summary>
    /// copy data from another tile
    /// </summary>
    public void AssignDataTile(MapTile otherTile)
    {
        uniqueData = otherTile.uniqueData;
    }
}

/// <summary>
/// pathfinding display status
/// </summary>
public enum TileStatus
{
    none, // no status
    current, // current pathfinding node
    next, // next pathfinding node
    clear // cleared pathfinding node
}



