using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// main menu, first interactable state
/// </summary>
public class MainMenuState : GameState
{
    public static MainMenuState Instance { get; private set; } // sinhleton instance

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    [Tooltip("push to start game")]     public Button playButton;
    [Tooltip("push to start tutorial")] public Button tutorialButton;
    [Tooltip("push to open settings")]  public Button settingsButton;

    private void Start()
    {
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.StartGame()); // start
        tutorialButton.onClick.AddListener(
            () => GameStateManager.Instance.StartTutorial());
        settingsButton.onClick.AddListener(
            () => GameStateManager.Instance.OpenSettings());

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
