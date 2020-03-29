using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private static LevelBuilder instance;
    public static LevelBuilder Instance { get { return instance; } private set { } }
    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public List<Level> levels;
}

