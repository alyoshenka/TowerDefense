using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseState : GamePlayState
{
    private static PurchaseState instance;
    public static PurchaseState Instance { get { return instance; } private set { } }

    public Button playButton;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        nextLogicalState = PlaceState.Instance;
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.TransitionToNextState(this));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter purchase"); }

        player.ResetHealth();
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit purchase"); }
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("purchase transition condition"); }
        return true;
    }
}
