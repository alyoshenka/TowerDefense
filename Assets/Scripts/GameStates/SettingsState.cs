using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manage game and player settings
/// </summary>
public class SettingsState : GameState
{
    public static SettingsState Instance { get; private set; } // singleton instance

    [Tooltip("push to go back (to main menu")] public Button backButton;

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    private void Start()
    {
        backButton.onClick.AddListener(
            () => GameStateManager.Instance.ReturnToMenu());

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter settings"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit settings"); }
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("settings exit condition"); }
        return true;
    }
}
