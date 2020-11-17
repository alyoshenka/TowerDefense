using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// allows contruction and saving of new levels
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; } // singleton instance
    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    [Tooltip("current saved levels")] public List<Level> levels;
}

