using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{

    LevelCreator mLevelCreator;

    int mIndex;

    int mRow = 0;
    int mCol = 0;

    string mName;

    void OnEnable()
    {
        Load();
    }

    void OnDisable()
    {
        Save();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();

        mCol = EditorGUILayout.IntField("Columns (X)",mCol);
        mRow = EditorGUILayout.IntField("Rows (Y)", mRow);

        GUILayout.EndHorizontal();

        mName = EditorGUILayout.TextField("Name: ", mName);

        //mIndex = GUILayout.SelectionGrid(mIndex,)
        if (GUILayout.Button("Create Grid"))
        {
            mLevelCreator.CreateCells(mCol, mRow, mName);
        }
    }



    void Save()
    {

    }

    void Load()
    {
        mLevelCreator = (LevelCreator)target;
        //mLevelCreator.Init();
    }

}
