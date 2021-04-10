using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// ToDo: there has got to be a better way to change states

/// <summary>
/// manages transitioning through game states
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public delegate void ExitGameplayEvent();
   // invoked upon returning to menu from gameplay state
    public event ExitGameplayEvent cleanupGameplay;

    // singleton instance
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        if(null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
    }

    private void Start()
    {        
        player = new Player(); // this should happen somewhere else

        currentState = OpenState.Instance;
        Transition(MainMenuState.Instance);

        PauseState.Instance.PauseGame(true);
    }

    private GameState previousState; // previous game state
    public GameState currentState; // current game state
    Player player; // player object

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

    // ToDo: take out
    /// <summary>
    /// transition back to previous state
    /// </summary>
    /// <returns>returns whether transition was possible</returns>
    public bool GoBack() { return Transition(previousState); }

    /// <summary>
    /// transition to next logical state
    /// </summary>
    /// <param name="oldState">"current" state</param>
    /// <returns>whether transition was successful</returns>
    public bool TransitionToNextState(GameState oldState)
    {
        if(null == oldState || null == oldState.NextLogicalState || !oldState.CanTransition())
        {
            Debug.LogWarning("cannot transition to next logical state");
            if (!oldState.CanTransition() && Debugger.Instance.StateTransitionWarnings) { Debug.Log("State: " + oldState.GetType().ToString() + " cannot transition"); }
            return false;
        }
        else
        {
            Transition(oldState.NextLogicalState);
            return true;
        }
    }

    /// <summary>
    /// return to menu state
    /// </summary>
    /// <returns>whether transition was successful</returns>
    public bool ReturnToMenu() 
    {
        // grosssss
        // fix this
        if (currentState.GetType().IsSubclassOf(typeof(GamePlayState))) { Debug.Log("cleanup gameplay"); cleanupGameplay?.Invoke(); }

        return Transition(MainMenuState.Instance); 
    }

    // ToDo: there's probably a better way and this isn't more efficient than before

    /// <summary>
    /// transition to start state
    /// </summary>
    /// <returns>whather transition was successful</returns>
    public bool StartGame() { return Transition(StartState.Instance); }
    /// <summary>
    /// transition to tutorial state
    /// </summary>
    /// <returns>whather transition was successful</returns>
    public bool StartTutorial() { return Transition(TutorialState.Instance); }
    /// <summary>
    /// transition to settings state
    /// </summary>
    /// <returns>whather transition was successful</returns>
    public bool OpenSettings() { return Transition(SettingsState.Instance); }
    /// <summary>
    /// transition to gameover state
    /// </summary>
    /// <param name="gameWin">the game was won</param>
    /// <returns>whather transition was successful</returns>
    public bool GameOver(bool gameWin) { return Transition(GameOverState.Instance); }
}

// ToDo: make all state instaces private
//  this will better encapsulate transitions

/// <summary>
/// a game state (kind of like a scene)
/// </summary>
public abstract class GameState : MonoBehaviour
{
    [SerializeField] [Tooltip("state-specific UI object")] private GameObject UIPanel;

    protected static PlayerSO player;


    protected GameState nextLogicalState; // the state it makes most sense to go to next (null allowed)
    public GameState NextLogicalState { get => nextLogicalState; } // get next logical state

    /// <returns>whether the state can transition out</returns>
    public abstract bool CanTransition();
    /// <summary>
    /// setup on state enter
    /// </summary>
    public virtual void OnEnter() { UIPanel.SetActive(true); }
    /// <summary>
    /// teardown on state exit
    /// </summary>
    public virtual void OnExit() { UIPanel.SetActive(false); }

    // public abstract void GoToNextState();
}

/// <summary>
/// game state dependent on game time
/// </summary>
public abstract class GamePlayState : GameState
{
    public delegate void EnterStateEvent(); // enter state event

    protected static Level currentLevel; // current game level
    public static Level CurrentLevel { get => currentLevel; } // get current level
    [SerializeField] [Tooltip("just here for viewing")] private Level showLevel;

    /// <summary>
    /// setup gameplay state
    /// </summary>
    protected void Initialize()
    {
        SetLevel(new Level(Level.Load_s(LevelBuilder.saveDir + "1" + Level.ext))); // load initial level
        GameStateManager.Instance.cleanupGameplay += Recycle;
    }

    /// <summary>
    /// clears the current level
    /// </summary>
    public void Recycle()
    {
        currentLevel.Recycle();
        currentLevel = null;

        GameStateManager.Instance.cleanupGameplay -= Recycle; // ?
    }

    // just here so i can view level in inspector
    protected void SetLevel(Level newLevel)
    {
        currentLevel = newLevel;
        showLevel = currentLevel;
        if (Debugger.Instance.LevelNumber) { Debug.Log("Level " + currentLevel.Number + " loaded"); }
    }


    // pause?
    // stats?
}

