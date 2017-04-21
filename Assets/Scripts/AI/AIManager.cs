using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIManager : MonoBehaviour
{
    List<IntVector2> GetArea(IntVector2 pos, int range)
    {
        List<IntVector2> tempList = new List<IntVector2>();

        int colRadius;
        IntVector2 tempVector = new IntVector2();

        for (int i = (range + 1); i > (-range - 1); i--)
        {

            colRadius = range - Mathf.Abs(i);

            for (int j = -colRadius; j < (colRadius + 1); j++)
            {

                tempVector.x = pos.x + j;
                tempVector.y = pos.y + i;

                tempList.Add(tempVector);
            }
        }



        return tempList;
    }
    List<Character> GetAllEnemiesAround(IntVector2 pos, int range)
    {
        List<IntVector2> area = GetArea(pos, range);

        List<Character> tempCharArea = new List<Character>();

        foreach (IntVector2 item in area)
        {
            Character tempChar = GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x].mCharacterObj;
            if (tempChar != null)
            {
                tempCharArea.Add(tempChar);
            }
        }

        return (tempCharArea);

    }

    int Distance(IntVector2 pos, IntVector2 target)
    {
        return Mathf.Abs((target.x - pos.x) + (target.y - pos.y));
    }

    IntVector2 FindClosestEnemy(Character character, IntVector2 pos, int range)
    {
        //mTauntCharacter
        List<Character> mCharacterList = GetAllEnemiesAround(pos, range);



        if (character.mTauntCharacter != null)
        {
            if (mCharacterList.Contains(character.mTauntCharacter))
            {
                return (character.mTauntCharacter.mCellPos);
            }
        }

        Character mClosest = null;

        foreach (Character item in mCharacterList)
        {
            if (mClosest == null)
            {
                mClosest = item;
            }
            else
            {
                if (Distance(pos, item.mCellPos) < Distance(pos, mClosest.mCellPos))
                {
                    mClosest = item;
                }
            }
        }

        return mClosest.mCellPos;

    }


    bool HasEnemyInArea(IntVector2 pos, int range)
    {
        if(GetAllEnemiesAround(pos,range).Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    void AStar(Character character, IntVector2 start, IntVector2 end, int movementPoints)
    {

    }

    void Attack(Character character, IntVector2 pos)
    {

    }

    void Calculate(Character character, IntVector2 pos, int range)
    {
        //calculate
    }

}
