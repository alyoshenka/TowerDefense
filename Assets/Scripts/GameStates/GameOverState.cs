using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverState : GameState
{
    private static GameOverState instance;
    public static GameOverState Instance { get { return instance; } private set { } }

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public Button returnToMenu;

    private void Start()
    {
        gameObject.SetActive(false);

        returnToMenu.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, MainMenuState.Instance));
    }

    public override void OnEnter()
    {
        PauseState.Instance.isPaused = true;
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();

        // destroy old map
    }

    public override bool CanTransition()
    {
        return true;
    }
}
