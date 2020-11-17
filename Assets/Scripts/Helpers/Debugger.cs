using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// debug display
/// </summary>
public class Debugger : MonoBehaviour
{
    public static Debugger Instance; // singleton instance

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

    [Tooltip("bottom left debug")] public TMPro.TMP_Text BL;
    [Tooltip("bottom right debug")] public TMPro.TMP_Text BR;

    void Awake() { Instance = this; }
}
