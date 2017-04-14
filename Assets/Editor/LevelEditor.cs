using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridEditor))]
public class LevelEditor : Editor
{

    GridEditor mGridEditor;

    Vector2 scrollPos;

    int xPos, yPos;

    void OnEnable()
    {
        mGridEditor = (GridEditor)target;
    }

    void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();

        xPos = EditorGUILayout.IntField("X:", xPos);
        yPos = EditorGUILayout.IntField("Y:", yPos);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Edit"))
        {
            EditCell(xPos, yPos);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int y = 0; y < yPos; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < xPos; x++)
            {
                if(GUILayout.Button(x + "," + y))
                {
                    EditCell(x, y);
                }
            }

            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.EndScrollView();

    }


    void EditCell(int x, int y)
    {


        CellWindow window = (CellWindow)EditorWindow.GetWindow(typeof(CellWindow));
        if(mGridEditor.GridCreated && mGridEditor.mGrid != null )
        {
            //open editable window
        }
    }
}
