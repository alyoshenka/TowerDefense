using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileManagerSO", menuName = "ScriptableObjects/TileManagerSO")]
public class TileManagerSO : ScriptableObject
{
    public List<TileSO> allTiles;

    public int Count { get => allTiles.Count; }
}
