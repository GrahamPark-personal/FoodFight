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
        SetEffectDuration(1);
        SetDamageDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Cross;

        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }

    public override void Exit()
    {
        GameManager.sInstance.mAttackShape = AttackShape.Area;
    }

    public override void Execute(IntVector2 pos)
    {
        GameManager tempGM = GameManager.sInstance;

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        Character tempEnemy = mCell.mEnemyObj;

        IntVector2 newPos = new IntVector2();

        newPos = GetStartPos();

        IntVector2 targetCell;

        do
        {
            targetCell = GetStartPos();
            targetCell.x -= 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = GetStartPos();
            targetCell.x += 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = GetStartPos();
            targetCell.y += 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = GetStartPos();
            targetCell.y -= 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

        }
        while (false);

        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.nothing;
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCannotMoveHere = false;
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj = null;


        tempEnemy.mPosition.position = GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x].transform.position + new Vector3(0, 1, 0);

        tempEnemy.mCellPos = targetCell;
        
        GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x].mTypeOnCell = TypeOnCell.enemy;
        GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x].mEnemyObj = tempEnemy;






        
    }
}
