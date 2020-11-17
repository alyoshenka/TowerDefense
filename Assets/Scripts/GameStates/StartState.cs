using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// get ready to start the game
/// </summary>
public class StartState : GamePlayState
{
    public static StartState Instance { get; private set; } // singleton instance

    [Tooltip("push to start game")] public Button playButton;

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    private void Start()
    {
        nextLogicalState = PurchaseState.Instance;
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.TransitionToNextState(this));

        gameObject.SetActive(false);
    }

    /// <summary>
    /// set up a new game, initialize player
    /// </summary>
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
