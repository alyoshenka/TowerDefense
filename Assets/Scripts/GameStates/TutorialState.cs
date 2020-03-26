using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialState : GameState
{
    private static TutorialState instance;
    public static TutorialState Instance { get { return instance; } private set { } }
    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, MainMenuState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter tutorial"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit tutorial"); }
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("tutorial exit condition"); }
        return true;
    }
}
