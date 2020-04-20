using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseState : GameState
{
    private static PauseState instance;
    public static PauseState Instance { get => instance; }

    public Button resumeButton;
    private bool paused; 
    public bool Paused { get => paused; }

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

        paused = true;
        gameObject.SetActive(false);
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("pause transition condition"); }
        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        paused = true;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log(GamePlayState.CurrentLevel.WinCon ? "game win" : GoalTile.LoseCon ? "game lose" : "enter pause"); } // sorry
    }

    public override void OnExit()
    {
        base.OnExit();
        paused = false;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit pause"); }
    }

    public void PauseGame()
    {
        if (paused) { Debug.LogError("game already paused"); }
        paused = true;
        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("pause"); }

        GameStateManager.Instance.TransitionToNextState(this); // gross?? or will it work?
    }

    public void UnpauseGame()
    {
        if (!paused) { Debug.LogError("game not paused"); }
        paused = false;
        if (Debugger.Instance.DeveloperHaltMessages) { Debug.Log("unpause"); }

        GameStateManager.Instance.GoBack();
    }

    public void TogglePauseGame()
    {
        if (paused) { UnpauseGame(); }
        else { PauseGame(); }
    }
}
