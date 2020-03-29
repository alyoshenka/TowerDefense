using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefendState : GamePlayState
{
    public delegate void EnterStateEvent();
    public event EnterStateEvent openDefend;

    private static DefendState instance;
    public static DefendState Instance { get { return instance; } private set { } }

    public Button pauseButton;
    public int startDelay;

    private bool openedDefend;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }
    private void Start()
    {
        pauseButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, PauseState.Instance));

        openedDefend = false;
        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (!openedDefend)
        {
            openedDefend = true;
            openDefend?.Invoke();
        }
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
