using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// preset tile allotments
/// </summary>
public class TileAllotmentPresets : MonoBehaviour
{
    public static TileAllotmentPresets Instance { get; private set; }
    public List<DisplayTileAllotment> levelPresets;

    private void Awake()
    {
        if(null == Instance) { Instance = this; }
    }
}

/// <summary>
/// tile allotment, but with more data
/// </summary>
[System.Serializable]
public class DisplayTileAllotment
{
    public int levelNumber = 1;
    public List<TileAllotment> tileAllotments;
    public List<Sprite> tileImages;
}
