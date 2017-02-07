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
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];



        print("Its the right one");


        



    }
}
