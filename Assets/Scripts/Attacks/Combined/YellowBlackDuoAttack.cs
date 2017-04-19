using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlackDuoAttack : Attack {

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values
        CreateID();
        SetDamage(3);
        SetEffectDuration(3);
        SetDamageDuration(3);
        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.OnCell;

        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }


    public override void Exit()
    {
        GameManager.sInstance.mAttackShape = AttackShape.Area;
    }



    public override void Execute(IntVector2 pos)
    {
        Character tempCharacter = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;

        tempCharacter.AddAilment(AilmentID.Virus, GetEffectDuration(), GetDamage());
        tempCharacter.patientZero = true;

    }


}
