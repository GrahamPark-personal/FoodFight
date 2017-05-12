using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlueDuoAttack : Attack {

    Cell mCell;

    public override void Init()
    {
        CreateID();
        SetDamage(5);
        SetSlow(1);
        SetRange(5);
        SetRadius(2);
        SetEffectDuration(2);
        SetDamageDuration(2);
        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.Square;
        GameManager.sInstance.mPreviewRadius = GetRadius();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        EffectParameters effectParm = new EffectParameters();

        effectParm.Effect = cellEffect.nothing;
        effectParm.CellAction = CellActionType.EveryStep;
        effectParm.Damage = GetDamage();
        effectParm.Slow = GetSlow();
        effectParm.Health = GetHealth();
        effectParm.Poison = GetPoison();
        effectParm.Taunt = GetTaunt();
        effectParm.EffectDuration = GetEffectDuration();
        effectParm.DamageDuration = GetDamageDuration();
        effectParm.Stun = GetStun();
        effectParm.ID = GetID();

        GameManager.sInstance.CreateAttackSquare(pos, GetRadius(), effectParm, false);



    }
}
