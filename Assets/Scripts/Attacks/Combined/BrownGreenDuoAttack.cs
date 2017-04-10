using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownGreenDuoAttack : Attack
{

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values

        CreateID();
        SetAOE(9);
        SetEffectDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.AllCharacters;

        //GameManager.sInstance.mCurrentRange = GetRange();

    }


    public override void Exit()
    {
        GameManager.sInstance.mAttackShape = AttackShape.Area;
    }



    public override void Execute(IntVector2 pos)
    {
        mCell = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x];

        Character casterCharacter = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;
        Character selectedCharacter = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj;

        selectedCharacter.AddAilment(AilmentID.Link, GetEffectDuration(), 0);
        selectedCharacter.CharacterLink = casterCharacter;
        selectedCharacter.Linked = true;

        //set character at pos's connected to true
        //set character at pos linked to startpos() character
        //add ailment to character with ID of Link, have it remove the link after
    }
}
