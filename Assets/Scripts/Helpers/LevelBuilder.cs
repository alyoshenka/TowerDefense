using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// allows contruction and saving of new levels
/// </summary>
[RequireComponent(typeof(BoardEditor))]
public class LevelBuilder : MonoBehaviour
{
    public static readonly string saveDir = "Assets/Levels/";

    public BoardEditor boardEditor;
    [SerializeField]
    public Level level;

    private void OnValidate()
    {
        if (boardEditor) { boardEditor.board = level.Board; }
        // if(level.Board.Size != oldBoardSize) { level.Board.Resize(); }
    }

    public void SaveLevel()
    {
        string path = saveDir + level.Name + Level.ext;
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("save level: " + path); }
        level.Save(path, level.ToSave());
        SaveBoard();
    }

    public void SaveBoard()
    {
        string path = saveDir + level.Board.name + GameBoard.ext;
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("save board: " + path); }
        level.Board.Save(path, level.Board.ToSave());
    }

    public void LoadLevel()
    {
        if (level.Name == string.Empty) { Debug.LogWarning("no level specified"); return; }

        string fn = LevelBuilder.saveDir + level.Name + Level.ext;
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("load level: " + fn); }
        level.FromLoad(level.Load(fn));
        // board is loaded within level
    }

    public void LoadBoard()
    {
        if (level.Board.name == string.Empty) { Debug.LogWarning("no board specified"); return; }

        string fn = LevelBuilder.saveDir + level.Board.name + GameBoard.ext;
        if (Debugger.Instance && Debugger.Instance.IOMessages) { Debug.Log("load board: " + fn); }
        level.Board.FromLoad(level.Board.Load(fn));
    }
}

/// <summary>
/// data needed to store and regenerate a level
/// </summary>
public struct Level_Save
{
    public LevelData data;
    public string boardName;
}

/// <summary>
/// data needed to store and regenerate a gameboard
/// </summary>
public struct GameBoard_Save
{
    public string name;
    public Vector2 size;
    public TileType[] tiles;
    public Vector3[] tilePositions;
    public List<int> constantTiles; // tiles that cannot be changed
}

/// <summary>
/// an object that can be saved
/// </summary>
public interface ISaveable<T> : System.Collections.Generic.IEnumerable<T>
{
    /// <returns>the saveable conversion of the object</returns>
    T ToSave();

    /// <summary>
    /// saves the saveable data to the given file
    /// </summary>
    /// <returns>if save was successful</returns>
    bool Save(string fileName, T data);

    /// <summary>
    /// loads the object from the given save data
    /// </summary>
    void FromLoad(T data);

    /// <summary>
    /// loads the saveable data from the given file
    /// </summary>
    T Load(string fileName);
}

// https://stackoverflow.com/questions/980766/how-do-i-declare-a-nested-enum

