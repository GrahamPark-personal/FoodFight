using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum EnemyOnCell
{
    None,
    RangedMinion,
    MeleeMinion,
    Boss
}


public class CellWindow : EditorWindow
{

    public LevelEditor mEditor;

    public Rect windowRect = new Rect(100, 100, 200, 200);

    string cellMovementInfo;

    bool mCannotMoveHere;
    Vector3 mRotation;
    int mLayerHeight;
    float mVerticalOffset;

    public int X = 0;
    public int Y = 0;


    void OnGUI()
    {
        cellMovementInfo = mCannotMoveHere ? "Cannot Move Here" : "Can Move Here";

        if(GUILayout.Button(cellMovementInfo))
        {
            mCannotMoveHere = !mCannotMoveHere;
        }

    }

    void OnLostFocus()
    {
        Save();
    }

    void OnDestroy()
    {
        Save();
    }

    void OnFocus()
    {
        Load();
    }

    void Save()
    {
        Cell tempCell = mEditor.mGridEditor.GetCell(X, Y);
        tempCell.mCannotMoveHere = mCannotMoveHere;
    }

    void Load()
    {
        Cell tempCell = mEditor.mGridEditor.GetCell(X, Y);
        mCannotMoveHere = tempCell.mCannotMoveHere;
    }

    void DoWindow(int unusedWindowID)
    {
        GUI.DragWindow();
    }

    static void Init()
    {
        GetWindow(typeof(CellWindow));
    }

}
