using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverState : GamePlayState
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
        returnToMenu.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, MainMenuState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        PauseState.Instance.isPaused = true;
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();

        currentLevel.Destroy();
        currentLevel = null;
        player.SaveAndDestroy();
        player = null;
    }

    public override bool CanTransition()
    {
        return true;
    }
}
