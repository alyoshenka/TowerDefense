using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartState : GamePlayState
{
    private static StartState instance;
    public static StartState Instance { get { return instance; } private set { } }

    public Button playButton;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        base.Initialize(); // first state

        playButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, PurchaseState.Instance));

        player = new Player();

        gameObject.SetActive(false);
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("start transition condition"); }
        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter start"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit start"); }
    }
}
