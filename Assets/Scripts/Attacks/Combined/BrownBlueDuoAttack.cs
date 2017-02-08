using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownBlueDuoAttack : Attack {

    Cell mCell;

    public override void Init()
    {
        CreateID();
        SetRange(6);
        SetRadius(1);
        SetAOE(1);
        SetEffectDuration(1);
        SetDamageDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {
        GameManager tempGM = GameManager.sInstance;

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        Character tempChar = mCell.mEnemyObj;
        tempChar.mPath.Clear();
        tempChar.mPosPath.Clear();

        GameManager.sInstance.mCurrGrid.rows[tempChar.mCellPos.y].cols[tempChar.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
        tempGM.mCurrGrid.rows[tempChar.mCellPos.y].cols[tempChar.mCellPos.x].mCharacterObj = null;
        tempChar.mCellPos = pos;

        tempChar.transform.position = tempGM.mCurrGrid.rows[tempChar.mCellPos.y].cols[tempChar.mCellPos.x].transform.position + new Vector3(0, 1, 0);
        tempGM.mCurrGrid.rows[tempChar.mCellPos.y].cols[tempChar.mCellPos.x].mCharacterObj = tempChar;
        GameManager.sInstance.mCurrGrid.rows[tempChar.mCellPos.y].cols[tempChar.mCellPos.x].mTypeOnCell = TypeOnCell.enemy;

        //Queue<IntVector2> movementPath = GameManager.sInstance.FindPath(pos, GetStartPos());

        //tempChar.RemoveMoves(movementPath.Count - 1);

        //print("count: " + movementPath.Count);


        //while (movementPath.Count > 1)
        //{
        //    IntVector2 intTemp = movementPath.Dequeue();
        //    tempChar.mPosPath.Enqueue(intTemp);
        //    Transform temp = tempGM.mCurrGrid.rows[intTemp.y].cols[intTemp.x].mCellTransform;
        //    tempChar.mPath.Enqueue(temp);
        //}

        //tempChar.mRunPath = true;

    }
}
