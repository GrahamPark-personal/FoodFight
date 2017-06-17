using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBlackDuoAttack : Attack
{

    public override void Init()
    {

        SetDamage(15);
        SetRange(1);        
        SetRadius(1);
        SetEffectDuration(3);        
        SetDamageDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.OnCell;
        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {
        // put a time bomb on the casting character
        // bomb follows casting character when they move
        // enemies in a cross around the caster take damage at the end of each turn
        // when the duration runs out, enemies AND caster take MORE damage (lets say 6?)

        // option 1:
        // make an ailment
        // check ailment at the end of the turn. (apply damage to surrounding enemies)
        // if the ailment's duration == 0 apply the larger damage

        Character mCharOnCell = GameManager.sInstance.mCharacterObj;
               
        mCharOnCell.AddAilment(AilmentID.TimeBomb, GetEffectDuration(), GetDamage());

    }
}


