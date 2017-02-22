using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBaseAttack : Attack{

    Cell mCell;

    public override void Init()
    {
        CreateID();

        SetDamage(4);
        SetSlow(2);
        SetRange(6);
        SetRadius(1);
        SetAOE(1);
        SetEffectDuration(2);
        SetDamageDuration(1);
        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();
    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {
            mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

            if (mCell.mTypeOnCell == TypeOnCell.character)
            {
                mCell.mCharacterObj.AddAilment(AilmentID.Slow, GetEffectDuration(), GetSlow());
                mCell.mCharacterObj.Damage(gameObject.GetComponent<Character>(), GetDamage());
            }
            else if(mCell.mTypeOnCell == TypeOnCell.enemy)
            {
                mCell.mEnemyObj.AddAilment(AilmentID.Slow, GetEffectDuration(), GetSlow());
                mCell.mEnemyObj.Damage(gameObject.GetComponent<Character>(), GetDamage());
            }
        
    }

}
