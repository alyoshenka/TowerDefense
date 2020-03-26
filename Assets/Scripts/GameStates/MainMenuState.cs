using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuState : GameState
{
    private static MainMenuState instance;
    public static MainMenuState Instance { get { return instance; } private set { } }
    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public Button playButton;
    public Button tutorialButton;
    public Button settingsButton;

    private void Start()
    {
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, StartState.Instance)); // start
        tutorialButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, TutorialState.Instance));
        settingsButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, SettingsState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter main menu"); }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit main menu"); }
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("main menu exit condition"); }
        return true;
    }
}
