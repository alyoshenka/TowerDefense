using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameOverState : GameState
{
    private static GameOverState instance;
    public static GameOverState Instance { get { return instance; } private set { } }

    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    private void Start()
    {
        gameObject.SetActive(false);

    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanTransition()
    {
        throw new System.NotImplementedException();
    }
}
