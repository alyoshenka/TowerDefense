using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    private static int buttonWidth = 100;

    private string levelName = "level_name", boardName = "board_name";

    public override void OnInspectorGUI()
    {
        LevelBuilder lb = (LevelBuilder)target;

        levelName = EditorGUILayout.TextField("Level Name", levelName);
        boardName = EditorGUILayout.TextField("Board Name", boardName);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Level", GUILayout.Width(buttonWidth))) { lb.LoadLevel(); }
        if (GUILayout.Button("Save Level", GUILayout.Width(buttonWidth))) { lb.SaveLevel(); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Board", GUILayout.Width(buttonWidth))) { lb.LoadBoard(); }
        if (GUILayout.Button("Save Board", GUILayout.Width(buttonWidth))) { lb.SaveBoard(); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Grid Connections", GUILayout.Width(buttonWidth*2))) { lb.level.Board.AddGridConnections(); }
        if (GUILayout.Button("Clear All Connections", GUILayout.Width(buttonWidth * 2))) { lb.level.Board.ClearAllConnections(); }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Tiles", GUILayout.Width(buttonWidth * 2))) { lb.level.Board.GenerateNewTiles(); }

        EditorGUILayout.Space();

        if (EditorGUILayout.Toggle("Vis 1-way connections", lb.connectionShower.showOneWays)) { lb.connectionShower.showOneWays = true; }
        else { lb.connectionShower.showOneWays = false; }

        DrawDefaultInspector();

        if (GUI.changed) { OnValidate(); }
    }

    private void OnValidate()
    {
        LevelBuilder lb = (LevelBuilder)target;

        lb.level.Name = levelName;
        lb.level.Board.name = boardName;
    }
}
