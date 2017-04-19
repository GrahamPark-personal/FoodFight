using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBaseAttack : Attack
{

	Cell mCell;
    List<Character> closedList = new List<Character>();

    public override void Init()
    {
        print("INIT");

        CreateID();
        SetHealth(4);
        SetRange(4);
        SetRadius(2);
        SetAOE(9);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Heal;
        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {


        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        if (mCell.mTypeOnCell == TypeOnCell.character)
        {
            Character mCharacter = mCell.mCharacterObj;
            mCharacter.Heal(GetHealth());

            if (mCharacter.mHealth > mCharacter.GetMaxHealth())
            {
                mCharacter.mHealth = mCharacter.GetMaxHealth();
            }
        }
        ChainHeal(pos);


        
    }

    void ChainHeal(IntVector2 pos)
    {
        List<Cell> cellList = new List<Cell>();
        List<Character> openList = new List<Character>();

        cellList = GameManager.sInstance.GetCellsInRange(pos,GetRadius());

        foreach (Cell cell in cellList)
        {
            if(cell.mTypeOnCell == TypeOnCell.character)
            {
                if(!closedList.Contains(cell.mCharacterObj) && !openList.Contains(cell.mCharacterObj))
                {
                    openList.Add(cell.mCharacterObj);
                }
            }
        }

        foreach (Character var in openList)
        {
            if (!closedList.Contains(var))
            {
                closedList.Add(var);
                openList.Remove(var);

                var.Heal(GetHealth());

                if (var.mHealth > var.GetMaxHealth())
                {
                    var.mHealth = var.GetMaxHealth();
                }
                ChainHeal(var.mCellPos);
            }
        }

    }

}
