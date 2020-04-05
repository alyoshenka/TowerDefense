using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    basic,
    dirt,
    grass,
    water,
    wall,
    goal,
    enemy
}

public class PathNode
{
    private TileData uniqueData;
    public TileData Data { get => uniqueData; }
    public TileType Type { get => uniqueData.Type; }
    public int Index { get => uniqueData.index; }
    
    [HideInInspector] public float calculatedCost; // cost to get to this node

    public int TraversalCost { get => uniqueData.traversalCost; }

    public PathNode previousNode; // determined when finding path
    public List<PathNode> connections;

    public void RemoveConnection(PathNode oldNode) { connections.Remove(oldNode); }
    public void AddConnection(PathNode newNode)
    {
        Debug.Assert(!connections.Contains(newNode));
        connections.Add(newNode);
    }

    private PathNode() { }

    public PathNode(TileData setData)
    {
        connections = new List<PathNode>();
        calculatedCost = Mathf.Infinity;

        uniqueData = setData;

        Debug.Assert(TraversalCost == setData.traversalCost);
    }

    public void AssignData(TileData newData)
    {
        newData.index = uniqueData.index;
        uniqueData = newData;
    }

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
     
    public void RemoveFromGoal() { AssignData(TileData.Basic); }

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

[System.Serializable]
public class MapTile : MonoBehaviour
{
    [SerializeField] protected TileData uniqueData;
    public TileData Data { get => uniqueData; }
    public TileType Type { get => uniqueData.Type; }
    public Color DisplayColor { get => uniqueData.DisplayColor; }
    public int TraversalCost { get => uniqueData.traversalCost; }
    public int BuildCost { get { return uniqueData.BuildCost; } private set { } }
    public int Index { get => uniqueData.index; set => uniqueData.index = value; }
    

    public delegate void TileEnteredEvent();
    public event TileEnteredEvent tileEnter;
    public delegate void TileExitedEvent();
    public event TileExitedEvent tileExit;
    public delegate void TileClickedEvent();
    public event TileClickedEvent tileClick;
    public static MapTile currentHover;

    public bool GoalTile { get => uniqueData.Type == TileType.goal; }

    [SerializeField] protected bool canBeChanged = true; // not quite optimal but ok
    public bool CanBeChanged { get { return canBeChanged || placedByPlayer; } private set { } }
    
    public bool HasCost { get => BuildCost > 0; }

    
    [SerializeField] private Sprite displayImage; // find better place

    public bool showConnections;

    public TileStatus tileStatus = TileStatus.none;

    public bool placedByPlayer;

    protected virtual void Awake()
    {
        // ok this might be kinda bad
        if (canBeChanged) 
        { 
            PlaceState.Instance.openPlace += AddPlaceIndicators;
            DefendState.Instance.openDefend += RemovePlaceIndicators;
        }
    }

    protected virtual void OnDestroy()
    {
        PlaceState.Instance.openPlace -= AddPlaceIndicators;
        DefendState.Instance.openDefend -= RemovePlaceIndicators;
    }

    #region Placement
    public void AddPlaceIndicators()
    {
        tileEnter += HoverEnter;
        tileExit += HoverExit;
        tileClick += TileSelected;
    }
    public void RemovePlaceIndicators()
    {
        tileEnter -= HoverEnter;
        tileExit -= HoverExit;
        tileClick -= TileSelected;
    }
    protected virtual void HoverEnter()
    {
        transform.localScale *= 1.4f;
        currentHover = this;
    }
    protected virtual void HoverExit()
    {
        transform.localScale /= 1.4f;
        if(this == currentHover) { currentHover = null; }
    }
    protected virtual void TileSelected()
    {
        TilePlacement.Instance.ClickTile(this);
    }
    #endregion

    public MapTile InstantiateInPlace(MapTile tileModel)
    {
        GameObject copy = Instantiate(
            tileModel.gameObject, transform.position, transform.rotation, transform.parent);
        MapTile newTile = copy.GetComponent<MapTile>();
        newTile.AssignData(uniqueData, false); // use my data

        return newTile;
    }

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

    // just pathdfinding stuff
    public void IndicateCurrent() { tileStatus = TileStatus.current; }
    public void IndicateNext() { tileStatus = TileStatus.next; }
    public void IndicateCleared() { tileStatus = TileStatus.clear; }
    public void IndicateNone() { tileStatus = TileStatus.none; }

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

    // is this the best way to be doign thie?

    private void OnMouseEnter() { tileEnter?.Invoke(); }

    private void OnMouseExit() { tileExit?.Invoke(); }

    private void OnMouseDown() { tileClick?.Invoke(); }


    public void AssignData(TileData newData, bool preserveIndex)
    {
        if (preserveIndex) { newData.index = uniqueData.index; }
        uniqueData = newData;

        name = uniqueData.index + "-" + uniqueData.Type;

        if(Type != TileType.basic) { canBeChanged = false; } // maybe??
    }

    public void AssignDataTile(MapTile otherTile)
    {
        uniqueData = otherTile.uniqueData;
    }
}

public enum TileStatus
{
    none,
    current,
    next,
    clear
}



