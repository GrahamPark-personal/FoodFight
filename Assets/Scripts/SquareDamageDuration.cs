using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDamageDuration : GroundDir {

    public int mTurnsLeft;
    public GameObject[] mVisualSquares;
    public Cell[] mCellLocations;
    public int mDamage;
    public int mSlow;


    void Set(int turns, GameObject[] visualSquares, Cell[] cellLocations, int damage, int slow)
    {
        mTurnsLeft = turns;
        mVisualSquares = visualSquares;
        mCellLocations = cellLocations;
        mDamage = damage;
        mSlow = slow;
    }

    public override void Continue()
    {
        foreach (Cell item in mCellLocations)
        {
            if(item.mTypeOnCell == TypeOnCell.character)
            {
                item.mCharacterObj.mMoveDistance -= mSlow;
                item.mCharacterObj.mHealth -= mDamage;
            }

            if (item.mTypeOnCell == TypeOnCell.enemy)
            {
                item.mEnemyObj.mMoveDistance -= mSlow;
                item.mEnemyObj.mHealth -= mDamage;
            }
        }
        mTurnsLeft--;
        if(mTurnsLeft <= 0)
        {
            foreach (GameObject item in mVisualSquares )
            {
                Destroy(item);
            }
            Destroy(this.gameObject);
        }

    }



}
