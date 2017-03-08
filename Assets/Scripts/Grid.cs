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
        mSize.y = rows.Length;
        mSize.x = rows[0].cols.Length;
        print("mSize: " + mSize.x + "," + mSize.y);
    }

    void Start ()
    {

        for (int x = 0; x < mSize.x; x++) 
        {
            for (int y = 0; y < mSize.y; y++)
            {
                rows[y].cols[x].mPos.x = x;
                rows[y].cols[x].mPos.y = y;

                if (!rows[y].cols[x].mCannotMoveHere)
                {
                    Transform pos = rows[y].cols[x].transform;
                    GameObject Spawn = Instantiate(GameManager.sInstance.mGridSquare, pos.position, pos.rotation); // + new Vector3(0,.08f,0)
                    Spawn.transform.localScale = pos.localScale;
                }

            }
        }
	}
	
	
	void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    GameManager.sInstance.mMouseMode = MouseMode.Move;
        //    GameManager.sInstance.ResetSelected();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    GameManager.sInstance.mMouseMode = MouseMode.Attack;
        //    GameManager.sInstance.ResetSelected();
        //}
    }
}
