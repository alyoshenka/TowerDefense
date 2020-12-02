using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TileSO", menuName ="ScriptableObjects/TileSO")]
public class TileSO : ScriptableObject
{
    public TileType tileType;
    public GameObject prefab;
    public Color displayColor;
    public Sprite displayImage;
    public int traversalCost;
    public int buildCost;

    public TileData GenerateTileData()
    {
        TileData td = new TileData
        {
            displayColor = new Color_S(displayColor),
            type = tileType,
            buildCost = buildCost,
            traversalCost = traversalCost
        };

        return td;
    }
}
