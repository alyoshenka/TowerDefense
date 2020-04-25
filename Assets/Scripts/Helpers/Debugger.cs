using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public static Debugger Instance;

    public bool StateChangeMessages;
    public bool StateTransitionWarnings;
    public bool TileMessages;
    public bool AgentMessages;
    public bool EnemyMessages;
    public bool AgentBrainMessages;
    public bool DeveloperHaltMessages;
    public bool UIAssigned;
    public bool AggroTriggers;
    public bool ShowAggroRanges;

    public TMPro.TMP_Text BL;
    public TMPro.TMP_Text BR;

    void Awake() { Instance = this; }
}
