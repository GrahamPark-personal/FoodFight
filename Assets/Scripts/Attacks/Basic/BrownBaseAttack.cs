using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownBaseAttack : Attack
{

    Cell mCell;

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


    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {
        List<IntVector2> attackPos = GameManager.sInstance.mAttackAreaLocations;

        List<IntVector2> characterList = new List<IntVector2>();

        Character temp = null;

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        Character mCharacter = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;


        foreach (IntVector2 item in attackPos)
        {
            if (item.x != GetStartPos().x && item.y != GetStartPos().y)
            {
                characterList.Add(item);

                Character tempChar = GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x].GetCharacterObject();

                print("Character of temp: " + tempChar);
                if (tempChar != null)
                {
                    tempChar.mTauntCharacter = mCharacter;
                    tempChar.AddAilment(AilmentID.Taunt, GetEffectDuration(), 0);
                }

            }
        }



    }

}
