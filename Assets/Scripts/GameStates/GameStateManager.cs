using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance { get { return instance; } private set { } }

    private void Awake()
    {
        if(null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        MainMenuState.Instance.OnEnter();
        currentState = MainMenuState.Instance;

        player = new Player();
    }

    GameState currentState;
    Player player;

    public void Transition(GameState prev, GameState next)
    {
        if (prev.CanTransition() && null != next)
        {
            prev.OnExit();
            next.OnEnter();
            currentState = MainMenuState.Instance;
        }
        else { Debug.LogError("cannot transition"); }
    }
}

public abstract class GameState : MonoBehaviour
{
    [SerializeField] private GameObject UIPanel = null;

    public abstract bool CanTransition();
    public virtual void OnEnter()
    {
        UIPanel.SetActive(true);
    }
    public virtual void OnExit()
    {
        UIPanel.SetActive(false);
    }
}

public abstract class GamePlayState : GameState
{
    protected static Level currentLevel;
    public static Level CurrentLevel { get { return currentLevel; } private set { } }
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

