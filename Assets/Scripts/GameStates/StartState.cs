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
        nextLogicalState = PurchaseState.Instance;
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.TransitionToNextState(this));

        gameObject.SetActive(false);
    }

    private void SetupGame()
    {
        base.Initialize(); // first state
        player = new Player(); // here too?
    }

    public override bool CanTransition()
    {
        return currentLevel.Number == 0;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        SetupGame();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter start"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit start"); }
    }
}
