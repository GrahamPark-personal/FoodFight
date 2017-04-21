using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{

    public GameObject mRangedEnemy;
    public GameObject mMeleeEnemy;
    public GameObject mBoss;
    [Space(10)]
    [Header("Place Grid Here")]
    public Grid mGrid;

    [HideInInspector]
    public bool GridCreated;

    public Cell GetCell(int x, int y)
    {
        return mGrid.rows[y].cols[x];
    }

}
