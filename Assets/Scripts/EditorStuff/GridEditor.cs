using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    public Grid mGrid;

    public GameObject mRangedEnemy;
    public GameObject mMeleeEnemy;
    public GameObject mBoss;

    [HideInInspector]
    public bool GridCreated;

    public Cell GetCell(int x, int y)
    {
        return mGrid.rows[y].cols[x];
    }

}
