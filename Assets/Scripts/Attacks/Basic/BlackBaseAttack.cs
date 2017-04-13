using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBaseAttack : Attack
{

    public GameObject ObjectMinion;

    public override void Init()
    {

        CreateID();
        SetRange(3);
        SetEffectDuration(3);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.AreaNoCharacters;
        GameManager.sInstance.mCurrentRange = GetRange();


    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        Character mCharOnCell = GetComponent<Character>();

        Cell myCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        GameObject mMinion = Instantiate(ObjectMinion, myCell.transform.position + new Vector3(0, 1, 0), myCell.transform.rotation);

        Character tempChar = mMinion.GetComponent<Character>();

        mCharOnCell.SpawnedMinion1 = tempChar;
        mCharOnCell.mHasSpawnedMinion = true;
        mCharOnCell.AddAilment(AilmentID.SpawnMinion, GetEffectDuration(), 0);

        myCell.mCharacterObj = tempChar;
        myCell.mTypeOnCell = TypeOnCell.playerMinion;

        IntVector2 targetCell = pos;


        do
        {
            targetCell = pos;
            targetCell.x -= 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = pos;
            targetCell.x += 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = pos;
            targetCell.y += 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

            targetCell = pos;
            targetCell.y -= 1;
            if (GameManager.sInstance.IsMovableBlock(targetCell))
            {
                break;
            }

        }
        while (false);


        Cell myCell2 = GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x];

        GameObject mMinion2 = Instantiate(ObjectMinion, myCell2.transform.position + new Vector3(0, 1, 0), myCell2.transform.rotation);

        Character tempChar2 = mMinion2.GetComponent<Character>();

        mCharOnCell.SpawnedMinion2 = tempChar2;

        myCell2.mCharacterObj = tempChar2;
        myCell2.mTypeOnCell = TypeOnCell.playerMinion;

    }
}
