using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// in-game pause
/// </summary>
public class PauseState : GameState
{
    public static PauseState Instance { get; private set; } // singleton instance

    [Tooltip("push to resume play")] public Button resumeButton;
    private bool loudPaused; // ui is displayed
    public bool Paused { get; private set; } // get silent paused

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    private void Start()
    {
        nextLogicalState = this; // kinda weird
        resumeButton.onClick.AddListener(
            () => PauseState.Instance.TogglePauseGame());

        Paused = false;
        loudPaused = false;
        gameObject.SetActive(false);
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("pause transition condition"); }
        return true;
    }

    public override void OnEnter()
    {
        if (loudPaused) { base.OnEnter(); } // show ui
        Paused = true;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log(GamePlayState.CurrentLevel.WinCon ? "game win" : GoalTile.LoseCon ? "game lose" : "enter pause"); } // sorry
    }

    public override void OnExit()
    {
        base.OnExit();
        Paused = loudPaused = false;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit pause"); }
    }

    /// <summary>
    /// pause gameplay
    /// </summary>
    /// <param name="silent">silent = game time stops, !silent = pause menu</param>
    public void PauseGame(bool silent = false)
    {       
        if (Paused && Debugger.Instance.DeveloperHaltMessages) { Debug.LogWarning("game silent paused"); }
        if (Paused && loudPaused) { Debug.LogWarning("game already paused"); }

        Paused = true;
        if (!silent) 
        { 
            loudPaused = true; 
            GameStateManager.Instance.TransitionToNextState(this); // gross?? or will it work?
        }

        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("pause: " + (silent ? "silent" : "loud")); }
    }

    /// <summary>
    /// go back to game state, resume play
    /// </summary>
    public void UnpauseGame()
    {
        if (!Paused) { Debug.LogError("game not paused"); }
        Paused = false;
        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("unpause"); }

        GameStateManager.Instance.GoBack();
    }

    /// <summary>
    /// pause if unpaused, unpause if paused
    /// </summary>
    public void TogglePauseGame()
    {
        if (Paused) { UnpauseGame(); }
        else { PauseGame(); }
    }
}
