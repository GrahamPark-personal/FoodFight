using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Cols
{
    public Cell[] cols;
}

//[System.Serializable]
//public struct Rows
//{
//    public Cell gridPiece;
//}

public class Grid : MonoBehaviour {

    public Cols[] rows;

    [HideInInspector]
    public IntVector2 mSize;

    void Awake()
    {
        mSize.x = rows.Length;
        mSize.y = rows[0].cols.Length;
    }

    void Start ()
    {

        for (int x = 0; x < mSize.x; x++) 
        {
            for (int y = 0; y < mSize.y; y++)
            {
                rows[x].cols[y].mPos.x = x;
                rows[x].cols[y].mPos.y = y;
            }
        }
	}
	
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.ResetSelected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.sInstance.mMouseMode = MouseMode.Attack;
            GameManager.sInstance.ResetSelected();
        }
    }
}
