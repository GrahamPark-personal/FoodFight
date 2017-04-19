using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownBaseAttack : Attack
{
    public override void Init()
    {

        CreateID();
        SetRange(5);
        SetRadius(3);
        SetTaunt(1);
        SetAOE(25);
        SetEffectDuration(1);
        SetStartPos(GameManager.sInstance.mSelectedCell);
        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        Character mCharOnCell = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;

        List<Character> mCharacters = GameManager.sInstance.GetCharactersInArea();

        foreach (Character item in mCharacters)
        {
            print(item.mCellPos.x + "," + item.mCellPos.y);
            item.mTauntCharacter = mCharOnCell;
            item.AddAilment(AilmentID.Taunt, GetEffectDuration(), 0);
        }



    }

}
