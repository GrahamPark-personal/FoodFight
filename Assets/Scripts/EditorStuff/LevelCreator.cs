using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    //it is a cube
    public GameObject mCellObject;

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
                GameObject temp = Instantiate(mCellObject, new Vector3(0 + (-i), 0, 90 + (-j)), new Quaternion(0, 0, 0, 1), mParent.transform);
                mGrid.rows[i].cols[j] = temp.GetComponent<Cell>();
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
