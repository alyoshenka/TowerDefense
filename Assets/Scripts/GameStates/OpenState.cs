
// not really sure what this is gonna do

/// <summary>
/// sets everything up for the game
/// </summary>
public class OpenState : GameState
{
    private static OpenState instance;
    public static OpenState Instance { get => instance; }
    private void Awake()
    {
        if (null == instance) { instance = this; }
        else if (this != instance) { Destroy(this); }
    }

    public override bool CanTransition()
    {
        return true;
    }

    public override void OnEnter() { UnityEngine.Debug.LogWarning("this should never happen"); }
    public override void OnExit() { }
}
