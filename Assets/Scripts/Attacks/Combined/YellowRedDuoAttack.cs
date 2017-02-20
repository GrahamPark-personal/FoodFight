using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



class YellowRedDuoAttack : Attack
{

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values
        CreateID();
        SetDamage(4);
        SetRange(6);
        SetRadius(2);
        SetAOE(9);



        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Cross;

        GameManager.sInstance.mCurrentRange = GetRange();

    }


    public override void Exit()
    {
        GameManager.sInstance.mAttackShape = AttackShape.Area;
    }



    public override void Execute(IntVector2 pos)
    {
        Character tempCharacter = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;

        GameManager.sInstance.mCurrGrid.rows[tempCharacter.mCellPos.y].cols[tempCharacter.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
        GameManager.sInstance.mCurrGrid.rows[tempCharacter.mCellPos.y].cols[tempCharacter.mCellPos.x].mCannotMoveHere = false;
        GameManager.sInstance.mCurrGrid.rows[tempCharacter.mCellPos.y].cols[tempCharacter.mCellPos.x].mCharacterObj = null;

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        //List<IntVector2> fireCells = new List<IntVector2>();


        tempCharacter.speed = tempCharacter.speed * 4;

        tempCharacter.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform);
        tempCharacter.mPosPath.Enqueue(pos);

        tempCharacter.mRunPath = true;


        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj = tempCharacter;

        GameManager.sInstance.CreateRowEffect(GetStartPos(), pos, CellTag.Fire, GetDamage());

        tempCharacter.mCellPos = pos;

    }




    public void GetNeighbors(Character character, List<IntVector2> neighbors)
    {
        for (int i = character.mCellPos.x - 1; i < character.mCellPos.x + 1; i++)
        {
            for (int j = character.mCellPos.y - 1; j < character.mCellPos.y + 1; j++)
            {
                if (character.mCellPos.x != i || character.mCellPos.y != j)
                {
                    IntVector2 nextNeighbor = new IntVector2();
                    nextNeighbor.x = character.mCellPos.x + i;
                    nextNeighbor.y = character.mCellPos.y + j;

                    neighbors.Add(nextNeighbor);
                }

            }
        }
    }

    public void CheckNeighbors(List<IntVector2> neighbors)
    {

        foreach (var cellPos in neighbors)
        {
            Character nextEnemy = GameManager.sInstance.mCurrGrid.rows[cellPos.y].cols[cellPos.x].mEnemyObj;

            if (nextEnemy != null)
            {
                nextEnemy.Damage(GetDamage());
                nextEnemy.mMoveDistance -= GetSlow();
            }
        }

    }
}