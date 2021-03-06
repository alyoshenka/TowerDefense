﻿
using UnityEngine;

// not really sure what this is gonna do

/// <summary>
/// sets everything up for the game, first state
/// </summary>
public class OpenState : GameState
{
    public static OpenState Instance { get; private set; } // singleton instance

    [SerializeField] public PlayerSO playerObject;

    private void Awake()
    {     
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
        if (null == player) { player = playerObject; }
    }

    public override bool CanTransition() { return true; }

    public override void OnEnter() { UnityEngine.Debug.LogWarning("this should never happen"); }
    public override void OnExit() { }
}
