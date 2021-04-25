using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// main game state, defend after tiles have been placed
/// </summary>
public class DefendState : GamePlayState
{
    public event EnterStateEvent openDefend; // invoked upon entering state
    public static DefendState Instance { get; private set; } // singleton instance

    [SerializeField]
    [Tooltip("enemy management object")]
    private EnemyManagerSO enemyManagerSO;

    [Tooltip("player/castle health display")] public TMP_Text castleHealth;
    [Tooltip("push to pause game")] public Button pauseButton;
    [Tooltip("delay start for number to seconds")] public int startDelay;
    [Tooltip("pause (silent) upon entering")] public bool pauseOnEnter;

    [Tooltip("enemy parent")] public Transform enemyParent;
    [Tooltip("available enemy tiles")] public EnemyManagerSO enemyManager;

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }
    private void Start()
    {
        pauseButton.onClick.AddListener(
            () => PauseState.Instance.TogglePauseGame());

        nextLogicalState = GameOverState.Instance; // should this be different?
        GoalTile.goalDeath += (() => { GameStateManager.Instance.TransitionToNextState(this); });

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        openDefend?.Invoke();
        // ToDo: currentLevel.Board.goalTile.CastleHealth = castleHealth;

        StartCoroutine("StartDelay");
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter defend"); }

        if (pauseOnEnter) { PauseState.Instance.PauseGame(); }
    }

    /// <summary>
    /// delay start, play animation?
    /// </summary>
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
