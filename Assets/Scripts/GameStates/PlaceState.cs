﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceState : GamePlayState
{
    public event EnterStateEvent openPlace;

    private static PlaceState instance;
    public static PlaceState Instance { get => instance; }

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
        nextLogicalState = DefendState.Instance;
        playButton.onClick.AddListener(
            () => GameStateManager.Instance.TransitionToNextState(this));

        gameObject.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("enter place"); }

        GenerateNextMap();

        openPlace?.Invoke();
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit place"); }
    }

    public override bool CanTransition()
    {
        if (!tilePlacement.AllUsed) { tilePlacement.IndicateUnused(); }
        if (!currentLevel.Board.GoalAssigned) { Debug.Log("goal not assigned"); }
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("place transition condition"); }
        return currentLevel.Board.GoalAssigned;
    }

    private void GenerateNextMap()
    {
        // generate map
        int newNumber = currentLevel.Number + 1;
        Level newLevel = Level.CreateLevel(newNumber);
        currentLevel = newLevel; 
        GameBoard newBoard = MapGenerator.GenerateBoard(newNumber, 
            Vector2.one * (newNumber + 5), tileParent);
        currentLevel.AssignBoard(newBoard);

        // take this out later
        ConnectionShower.Instance.board = newBoard;

        SetLevel(CurrentLevel);
    }
}
