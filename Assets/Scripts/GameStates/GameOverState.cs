using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// game is over, either won or lost
/// </summary>
public class GameOverState : GamePlayState
{
    private static GameOverState instance; // singleton instance
    public static GameOverState Instance // get singleton instance
        { get { return instance; } private set { } }

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    [Tooltip("push to return to menu")] public Button returnToMenu;

    private void Start()
    {
       
        returnToMenu.onClick.AddListener(
            () => GameStateManager.Instance.ReturnToMenu());

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        PauseState.Instance.PauseGame(true);
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
