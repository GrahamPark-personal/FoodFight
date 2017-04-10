using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownRedDuoAttack : Attack {

    public override void Init()
    {

        CreateID();
        SetRange(1);
        SetHealth(14);
        SetEffectDuration(2);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.OnCell;
        GameManager.sInstance.mCurrentRange = GetRange();


    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        Character mCharOnCell = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;

        print("Activated");

        mCharOnCell.AddAilment(AilmentID.Heal, GetEffectDuration(), GetHealth());

    }
}
