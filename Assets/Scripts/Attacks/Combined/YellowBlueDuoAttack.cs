using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlueDuoAttack : Attack {

    Cell mCell;

    public override void Init()
    {
        CreateID();
        SetHealth(0);
        SetDamage(5);
        SetRange(5);
        SetSlow(1);
        SetRadius(2);
        SetAOE(9);
        SetEffectDuration(3);
        SetDamageDuration(3);
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        EffectParameters effectParm = new EffectParameters();

        effectParm.Damage = GetDamage();
        effectParm.Slow = GetSlow();
        effectParm.Health = GetHealth();
        effectParm.Poison = GetPoison();
        effectParm.EffectDuration = GetEffectDuration();
        effectParm.DamageDuration = GetDamageDuration();
        effectParm.Stun = GetStun();
        effectParm.ID = GetID();

        GameManager.sInstance.CreateAttackSquare(pos, GetRadius(), effectParm);



    }
}
