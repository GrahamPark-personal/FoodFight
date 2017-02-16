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
        //tempCharacter.mPosition.position = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 1, 0);

        

        


        //while (temp.mCellPos.x != mCell.mPos.x || temp.mCellPos.y != mCell.mPos.y)
        //{

        //    //While current position is not the same as destination
        //    // move the character towards the destination 1 square (either x++ or y++)

        //    // whichever axis you are traveling along( x or y ) do the following for each cell

        //    // fireCells.pushback(currentPos.x, currentPos.y - 1)
        //    // fireCells.pushback(currentPos.x, currentPos.y + 0)
        //    // fireCells.pushback(currentPos.x, currentPos.y + 1)

        //    // OR

        //    // fireCells.pushback(currentPos.x - 1, currentPos.y)
        //    // fireCells.pushback(currentPos.x + 0, currentPos.y)
        //    // fireCells.pushback(currentPos.x + 1, currentPos.y)

        //    IntVector2 tempCell = temp.mCellPos;
        //    if (temp.mCellPos.x == mCell.mPos.x)
        //    {
        //        tempCell.x--;
        //        fireCells.Add(tempCell);

        //        tempCell.x++;
        //        fireCells.Add(tempCell);

        //        tempCell.x++;
        //        fireCells.Add(tempCell);

        //        temp.mCellPos.y++;
        //        Debug.Log("Temp Cell Pos : " + temp.mCellPos.x + ", " + temp.mCellPos.y);
        //    }
        //    if (temp.mCellPos.y == mCell.mPos.y)
        //    {

        //        tempCell.y--;
        //        fireCells.Add(tempCell);

        //        tempCell.y++;
        //        fireCells.Add(tempCell);

        //        tempCell.y++;
        //        fireCells.Add(tempCell);

        //        temp.mCellPos.x++;
        //    }
        //}

        //foreach (var cell in fireCells)
        //{
        //    int i = 1;
        //    Debug.Log("FireCell #" + i + " : " + cell.x + " , " + cell.y);
        //    i++;
        //}

        //IntVector2 targetCell = temp.mCellPos;

        //GameManager.sInstance.IsMovableBlock(targetCell);
        //do
        //{
        //    targetCell = pos;
        //    targetCell.x -= 1;
        //    if (GameManager.sInstance.IsMovableBlock(targetCell))
        //    {
        //        break;
        //    }

        //    targetCell = pos;
        //    targetCell.x += 1;
        //    if (GameManager.sInstance.IsMovableBlock(targetCell))
        //    {
        //        break;
        //    }

        //    targetCell = pos;
        //    targetCell.y += 1;
        //    if (GameManager.sInstance.IsMovableBlock(targetCell))
        //    {
        //        break;
        //    }

        //    targetCell = pos;
        //    targetCell.y -= 1;
        //    if (GameManager.sInstance.IsMovableBlock(targetCell))
        //    {
        //        break;
        //    }

        //}
        //while (false);

        //temp.mPosition.position = GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x].transform.position + new Vector3(0, 1, 0);

        ////targetcell is now the end position
        //temp.mCellPos = targetCell;


        //GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mTypeOnCell = TypeOnCell.character;
        //GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCannotMoveHere = true;
        //GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCharacterObj = temp;

        //// Move Character to Target Location
        //// Applies Damage

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
                nextEnemy.mHealth -= GetDamage();
                nextEnemy.mMoveDistance -= GetSlow();
            }
        }

    }
}