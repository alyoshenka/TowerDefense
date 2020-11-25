using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    private static int buttonWidth = 100;

    private string levelName, boardName;

    public override void OnInspectorGUI()
    {
        LevelBuilder lb = (LevelBuilder)target;

        levelName = EditorGUILayout.TextField("Level Name", levelName);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Level", GUILayout.Width(buttonWidth))) { lb.LoadLevel(); }
        if (GUILayout.Button("Save Level", GUILayout.Width(buttonWidth))) { lb.SaveLevel(); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        boardName = EditorGUILayout.TextField("Board Name", boardName);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Board", GUILayout.Width(buttonWidth))) { lb.LoadBoard(); }
        if (GUILayout.Button("Save Board", GUILayout.Width(buttonWidth))) { lb.SaveBoard(); }
        EditorGUILayout.EndHorizontal();

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
