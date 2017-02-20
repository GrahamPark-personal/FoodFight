using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowGreenDuoAttack : Attack {

    Cell mCell;

    public override void Init()
    {
        CreateID();
        SetStun(2);
        SetHealth(5);
        SetRange(5);
        SetRadius(2);
        SetAOE(9);
        SetEffectDuration(3);
        SetDamageDuration(3);
        GameManager.sInstance.mAttackShape = AttackShape.AreaNoCharacters;
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        EffectParameters WallParm = new EffectParameters();
        WallParm.Effect = cellEffect.Wall;
        WallParm.CellAction = CellActionType.Nothing;
        WallParm.EffectDuration = GetEffectDuration();
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].AddEffect(WallParm);

        EffectParameters effectParm = new EffectParameters();

        effectParm.Effect = cellEffect.LightningRod;
        effectParm.CellAction = CellActionType.StartOfTurn;
        effectParm.Damage = GetDamage();
        effectParm.Slow = GetSlow();
        effectParm.Health = GetHealth();
        effectParm.Poison = GetPoison();
        effectParm.Taunt = GetTaunt();
        effectParm.EffectDuration = GetEffectDuration();
        effectParm.DamageDuration = GetDamageDuration();
        effectParm.Stun = GetStun();
        effectParm.ID = GetID();

        print("Fine before here?");

        GameManager.sInstance.CreateAttackSquare(pos, GetRadius(), effectParm, true);


    }
}
