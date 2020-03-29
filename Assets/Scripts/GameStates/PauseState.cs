using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseState : GameState
{
    private static PauseState instance;
    public static PauseState Instance { get => instance; }

    public Button resumeButton;
    public bool isPaused; // private

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, DefendState.Instance));

        isPaused = true;
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
        isPaused = true;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter pause"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        isPaused = false;
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit pause"); }
    }
}
