using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    //it is a cube
    //public GameObject mCellObject;

    public GameObject mCornerBlock;
    public GameObject mSideBlock;
    public GameObject mMiddleBlock;

    //public GameObject mParent;

    [HideInInspector]
    public Grid mGrid;

    //string[] mGridList;

    [HideInInspector]
    public int row = 0; // y
    [HideInInspector]
    public int col = 0; // x

    [HideInInspector]
    public string mName;


    bool mCreated = false;


    int mIndex = 0;


    List<GameObject> mObjects;
    //Cell GetCell(int col, int row)
    //{

    //}

    GameObject GetObjectToUse(int x, int y)
    {
        if(y == 0 && x == 0)//corner
        {
            return mCornerBlock;

        }
        if(y == 0 && x == col)//corner
        {
            return mCornerBlock;

        }
        if (y == row && x == 0)//corner
        {
            return mCornerBlock;

        }
        if (y == row && x == col)//corner
        {
            return mCornerBlock;

        }
        if (y == 0)//side
        {
            return mSideBlock;

        }
        if (y == row)//side
        {
            return mSideBlock;

        }
        if (x == 0)//side
        {
            return mSideBlock;

        }
        if (x == col)//side
        {
            return mSideBlock;

        }

        return mMiddleBlock;
    }

    public void CreateCells(int col, int row, string name)
    {
        GameObject mParent = new GameObject(name);

        mGrid = mParent.AddComponent<Grid>();

        mGrid.rows = new Cols[row];
        for (int i = 0; i < row; i++)
        {
            mGrid.rows[i].cols = new Cell[col];
            for (int j = 0; j < col; j++)
            {
                GameObject objToUse = GetObjectToUse(j, i);

                GameObject temp = Instantiate(objToUse, new Vector3(0 + (-i), 0, 90 + (-j)), new Quaternion(0, 0, 0, 1), mParent.transform);
                Cell tempCell = temp.GetComponent<Cell>();
                mGrid.rows[i].cols[j] = tempCell;
                tempCell.mPos.x = j;
                tempCell.mPos.y = i;
            }
        }

        mCreated = true;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
