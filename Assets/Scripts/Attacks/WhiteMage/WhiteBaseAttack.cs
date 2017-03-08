using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBaseAttack : Attack
{

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values
        CreateID();
        SetSlow(5);
        SetEffectDuration(1);
        SetRange(5);
        SetRadius(3);



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
        mCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        IntVector2 finalPos = GetStartPos();

        string dir = "";

        if (GetStartPos().x > pos.x)
        {
            dir = "Up";
            finalPos.x -= GetRange();
        }
        else if (GetStartPos().x < pos.x)
        {
            dir = "Down";
            finalPos.x += GetRange();
        }
        else if (GetStartPos().y > pos.y)
        {
            dir = "Right";
            finalPos.y -= GetRange();
        }
        else if (GetStartPos().y < pos.y)
        {
            dir = "Left";
            finalPos.y += GetRange();
        }



        List<IntVector2> locations = GameManager.sInstance.GetRowLocations(GetStartPos(), finalPos);

        print("StartPos: " + GetStartPos().x + "," + GetStartPos().y);

        print("FinalPos: " + pos.x + "," + pos.y);

        print("Direction: " + dir);

        print("Size: " + locations.Count);

        List<Character> charactersInZone = new List<Character>();


        foreach (IntVector2 item in locations)
        {
            if(GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x].mTypeOnCell == TypeOnCell.character)
            {
                charactersInZone.Add(GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x].mCharacterObj);
            }
        }

        foreach (Character ch in charactersInZone)
        {
            IntVector2 tempPos = ch.mCellPos;

            if (GetStartPos().x > pos.x)
            {
                tempPos.x -= 3;
                if(GameManager.sInstance.IsMovableBlock(tempPos))
                {
                    ch.mPosPath.Enqueue(tempPos);
                    ch.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].transform);
                    ch.mRunPath = true;
                    GameManager.sInstance.MoveCharacterSlot(tempPos, ch);
                }
            }
            else if (GetStartPos().x < pos.x)
            {
                tempPos.x += 3;
                if (GameManager.sInstance.IsMovableBlock(tempPos))
                {
                    ch.mPosPath.Enqueue(tempPos);
                    ch.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].transform);
                    ch.mRunPath = true;
                    GameManager.sInstance.MoveCharacterSlot(tempPos, ch);

                }
            }
            else if (GetStartPos().y > pos.y)
            {
                tempPos.y -= 3;
                if (GameManager.sInstance.IsMovableBlock(tempPos))
                {
                    ch.mPosPath.Enqueue(tempPos);
                    ch.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].transform);
                    ch.mRunPath = true;
                    GameManager.sInstance.MoveCharacterSlot(tempPos, ch);

                }
            }
            else if (GetStartPos().y < pos.y)
            {
                tempPos.y += 3;
                if (GameManager.sInstance.IsMovableBlock(tempPos))
                {
                    ch.mPosPath.Enqueue(tempPos);
                    ch.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].transform);
                    ch.mRunPath = true;
                    GameManager.sInstance.MoveCharacterSlot(tempPos, ch);

                }
            }
        }


    }

}
