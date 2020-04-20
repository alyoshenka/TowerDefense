using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseState : GameState
{
    private static PauseState instance;
    public static PauseState Instance { get => instance; }

    public Button resumeButton;
    private bool silentPaused; // game events stop
    private bool loudPaused; // ui is displayed
    public bool Paused { get => silentPaused; }

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        nextLogicalState = this; // kinda weird
        resumeButton.onClick.AddListener(
            () => PauseState.Instance.TogglePauseGame());

        silentPaused = false;
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
        silentPaused = true;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log(GamePlayState.CurrentLevel.WinCon ? "game win" : GoalTile.LoseCon ? "game lose" : "enter pause"); } // sorry
    }

    public override void OnExit()
    {
        base.OnExit();
        silentPaused = loudPaused = false;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit pause"); }
    }

    public void PauseGame(bool silent = false)
    {       
        if (Paused) { Debug.LogWarning("game silent paused"); }
        if (Paused && loudPaused) { Debug.LogWarning("game already paused"); }

        silentPaused = true;
        if (!silent) 
        { 
            loudPaused = true; 
            GameStateManager.Instance.TransitionToNextState(this); // gross?? or will it work?
        }

        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("pause: " + (silent ? "silent" : "loud")); }
    }

    public void UnpauseGame()
    {
        if (!silentPaused) { Debug.LogError("game not paused"); }
        silentPaused = false;
        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("unpause"); }

        GameStateManager.Instance.GoBack();
    }

    public void TogglePauseGame()
    {
        if (silentPaused) { UnpauseGame(); }
        else { PauseGame(); }
    }
}
