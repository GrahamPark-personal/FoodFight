﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBaseAttack : Attack {

    Cell mCell;

    public override void Init()
    {
        CreateID();

        SetDamage(6);
        SetRange(5);
        SetStun(1);
        SetRadius(1);
        SetAOE(1);
        SetEffectDuration(2);
        SetDamageDuration(1);
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.x].cols[pos.y];

        if (mCell.mTypeOnCell == TypeOnCell.character)
        {
            mCell.mCharacterObj.AddAilment(AilmentID.Stun, GetEffectDuration(), 0);
            mCell.mCharacterObj.mHealth -= GetDamage();
        }
        else if (mCell.mTypeOnCell == TypeOnCell.enemy)
        {
            print(GetEffectDuration() + "," + GetStun());
            mCell.mEnemyObj.AddAilment(AilmentID.Stun, GetEffectDuration(), 0);
            mCell.mEnemyObj.mHealth -= GetDamage();
        }

    }
}