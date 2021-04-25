using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileManagerSO", menuName = "ScriptableObjects/TileManagerSO")]
public class TileManagerSO : ScriptableObject, ICollectionManager<TileType>
{
    [SerializeField]
    private List<TileSO> allTiles;

    public int Count { get => allTiles.Count; }

    public TileSO At(int idx) { return allTiles[idx]; }

    ICollectionObject ICollectionManager<TileType>.Get(TileType type)
    {
        return allTiles.Find(t => t.tileType == type);
    }
}
