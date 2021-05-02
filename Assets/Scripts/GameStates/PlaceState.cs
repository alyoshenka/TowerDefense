using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceState : GamePlayState
{
    public event EnterStateEvent openPlace; // invoked upon entering state

    public static PlaceState Instance { get; private set; } // singleton instance

    public bool GoalPlaced { get => currentLevel.Board.GoalAssigned; } // get whether goal is assigned
    // ToDo: bad design!
    public Board Board { get => currentLevel.Board; } // get current board

    [Tooltip("parent transform for tiles")] public Transform tileParent;
    [Tooltip("push to 'start' game")] public Button playButton;
    [Tooltip("tile placement manager")] public TilePlacement tilePlacement;

    private void Awake()
    {
        if (null == Instance) { Instance = this; }
        else if (this != Instance) { Destroy(this); }
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

        // GenerateNextMap();

        openPlace?.Invoke();
    }

    public override void OnExit()
    {
        base.OnExit();
        DisplayTile.ClearSelection();
        if (Debugger.Instance.StateChangeMessages) { Debug.Log("exit place"); }
    }

    public override bool CanTransition()
    {
        if (!tilePlacement.AllUsed) { tilePlacement.IndicateUnused(); }
        if (!currentLevel.Board.GoalAssigned && Debugger.Instance.StateTransitionWarnings) { Debug.Log("goal not assigned"); }
        if (Debugger.Instance.StateTransitionWarnings) { Debug.LogWarning("place transition condition"); }
        return currentLevel.Board.GoalAssigned;
    }
    /*
    /// <summary>
    /// increment level, generate and assign new board
    /// </summary>
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
    */
}
