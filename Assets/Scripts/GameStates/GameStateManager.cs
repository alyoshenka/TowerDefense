using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// there has got to be a better way to change states

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance { get => instance; }

    private void Awake()
    {
        if(null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        currentState = OpenState.Instance;
        Transition(MainMenuState.Instance);

        player = new Player();
    }

    private GameState previousState; // private
    public GameState currentState;
    Player player;

    /// <summary>
    /// transitions from current state to new state
    /// </summary>
    /// <returns> whether transition was possible </returns>
    private bool Transition(GameState next)
    {
        if (currentState.CanTransition() && null != next)
        {
            currentState.OnExit();
            next.OnEnter();
            previousState = currentState;
            currentState = next;
            return true;
        }
        else { Debug.LogWarning("cannot transition"); return false; }
    }

    // take out
    public bool GoBack() { return Transition(previousState); }

    public bool TransitionToNextState(GameState oldState)
    {
        if(null == oldState || null == oldState.NextLogicalState || !oldState.CanTransition())
        {
            Debug.LogError("cannot transition to next logical state");
            return false;
        }
        else
        {
            Transition(oldState.NextLogicalState);
            return true;
        }
    }

    public bool ReturnToMenu() { return Transition(MainMenuState.Instance); }

    // there's probably a better way and this isn't more efficient than before

    public bool StartGame() { return Transition(StartState.Instance); }
    public bool StartTutorial() { return Transition(TutorialState.Instance); }
    public bool OpenSettings() { return Transition(SettingsState.Instance); }
    public bool GameOver(bool gameWin) { return Transition(GameOverState.Instance); }
}

// todo: make all state instaces private
//  this will better encapsulate transitions

public abstract class GameState : MonoBehaviour
{
    [SerializeField] private GameObject UIPanel;

    protected GameState nextLogicalState; // the state it makes most sense to go tonext (null allowed)
    public GameState NextLogicalState { get => nextLogicalState; }

    public abstract bool CanTransition();
    public virtual void OnEnter() { UIPanel.SetActive(true); }
    public virtual void OnExit() { UIPanel.SetActive(false); }

    // public abstract void GoToNextState();
}

public abstract class GamePlayState : GameState
{
    public delegate void EnterStateEvent();

    protected static Level currentLevel;
    public static Level CurrentLevel { get => currentLevel; }
    [SerializeField] private Level showLevel; // just here to view

    protected static Player player;


    protected void Initialize()
    {
        SetLevel(Level.CreateLevel(0));
    }

    // just here so i can view level in inspector
    protected void SetLevel(Level newLevel)
    {
        currentLevel = newLevel;
        showLevel = currentLevel;
        Debug.Log("Level set to " + currentLevel.Number);
    }


    // pause?
    // stats?
}

