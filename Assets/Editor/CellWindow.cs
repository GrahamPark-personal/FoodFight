using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum EnemyOnCell
{
    RangedMinion,
    MeleeMinion,
    Boss,
    None
}


public class CellWindow : EditorWindow
{


    public Rect windowRect = new Rect(100, 100, 200, 200);

    string cellMovementInfo;

    bool mCannotMoveHere;
    Vector3 mRotation;
    int mLayerHeight;
    float mVerticalOffset;
    EnemyOnCell mEnemyType;


    void OnGUI()
    {
        cellMovementInfo = true ? "Cannot Move Here" : "Can Move Here";
        if(GUILayout.Button(cellMovementInfo))
        {
            mCannotMoveHere = !mCannotMoveHere;
        }
    }

    void DoWindow(int unusedWindowID)
    {
        GUI.DragWindow();
    }

    static void Init()
    {
        EditorWindow.GetWindow(typeof(CellWindow));
    }
}
