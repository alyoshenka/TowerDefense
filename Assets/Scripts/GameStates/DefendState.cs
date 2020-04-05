using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefendState : GamePlayState
{
    public event EnterStateEvent openDefend;

    private static DefendState instance;
    public static DefendState Instance { get { return instance; } private set { } }

    public TMP_Text castleHealth;
    public Button pauseButton;
    public int startDelay;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }
    private void Start()
    {
        pauseButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, PauseState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        openDefend?.Invoke();
        currentLevel.Board.goalTile.CastleHealth = castleHealth;

        StartCoroutine("StartDelay");
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter defend"); }
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        PauseState.Instance.OnExit(); // change
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit defend"); }

        // save state
    }

    public override bool CanTransition()
    {
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("defend transition condition"); }
        return true;
    }
}
