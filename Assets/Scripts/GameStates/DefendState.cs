using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefendState : GamePlayState
{
    private static DefendState instance;
    public static DefendState Instance { get { return instance; } private set { } }

    public Button pauseButton;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }
    private void Start()
    {
        pauseButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, PauseState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter defend"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit base"); }

        // save state
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("defend transition condition"); }
        return true;
    }
}
