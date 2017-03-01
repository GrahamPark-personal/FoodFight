using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlackDuoAttack : Attack
{

    public override void Init()
    {

        SetDamage(4);
        SetRange(1);
        SetHealth(4);
        SetRadius(1);
        SetEffectDuration(2);
        SetDamageDuration(2);
        
        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        Character mCharOnCell = GetComponent<Character>();
        Character tempChar = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj;
      
        tempChar.AddAilment(AilmentID.Poison, GetEffectDuration(), GetDamage());

        // if heal is over time:
        mCharOnCell.AddAilment(AilmentID.Heal, GetEffectDuration(), GetDamage());

        // if heal is just once:
        //mCharOnCell.Heal(GetDamage());



    }
}


