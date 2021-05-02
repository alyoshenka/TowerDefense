using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// debug display
/// </summary>
[ExecuteInEditMode]
public class Debugger : MonoBehaviour
{
    private static Debugger instance; // singleton instance
    public static Debugger Instance { get => instance; }

    // [Tooltip("toggle all")] public bool All;

    [Tooltip("print state change messages")] public bool StateChangeMessages;
    [Tooltip("print state transition messages")] public bool StateTransitionWarnings;
    [Tooltip("print tile messages")] public bool TileMessages;
    [Tooltip("print agent messages")] public bool AgentMessages;
    [Tooltip("print enemy messages")] public bool EnemyMessages;
    [Tooltip("print agent brain messages")] public bool AgentBrainMessages;
    [Tooltip("print developer halt messages")] public bool DeveloperHaltMessages; // what
    [Tooltip("print ui assigned messages")] public bool UIAssigned; // if ui not fully assigned
    [Tooltip("print saggro trigger messages")] public bool AggroTriggers;
    [Tooltip("show aggro ranges")] public bool ShowAggroRanges;
    [Tooltip("print file io messages")] public bool IOMessages;
    [Tooltip("name tile hover enter/exit")] public bool TileHover;
    [Tooltip("name tile hover select/deselect")] public bool TileSelect;
    [Tooltip("display level number")] public bool LevelNumber;
    [Tooltip("level loading steps")] public bool LevelSteps;
    [Tooltip("pathfinding visualization")] public bool VisualizePathfinding;


    [Tooltip("bottom left debug")] public TMPro.TMP_Text BL;
    [Tooltip("bottom right debug")] public TMPro.TMP_Text BR;

    void Awake() { instance = this; }
}
