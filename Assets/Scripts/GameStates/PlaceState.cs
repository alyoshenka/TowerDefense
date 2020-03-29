using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceState : GamePlayState
{
    private static PlaceState instance;
    public static PlaceState Instance { get { return instance; } private set { } }

    public bool GoalPlaced { get => currentLevel.Board.GoalAssigned; }
    public GameBoard Board { get => currentLevel.Board; } // bas design!!

    public Transform tileParent;
    public Button playButton;
    public TilePlacement tilePlacement;

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.Transition(this, DefendState.Instance));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter place"); }

        // generate map
        int newNumber = currentLevel.Number + 1;
        Level newLevel = Level.CreateLevel(newNumber);
        currentLevel = newLevel; 
        GameBoard newBoard = MapGenerator.GenerateBoard(newNumber, 
            Vector2.one * (newNumber + 5), tileParent);
        currentLevel.AssignBoard(newBoard);

        SetLevel(CurrentLevel);
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit place"); }
    }

    public override bool CanTransition()
    {
        if (!currentLevel.Board.GoalAssigned) { Debug.Log("goal not assigned"); }
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("place transition condition"); }
        return currentLevel.Board.GoalAssigned;
    }
}
