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
        SetEffectDuration(2);

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

        mCharOnCell.SpawnedMinion = tempChar;
        mCharOnCell.mHasSpawnedMinion = true;
        mCharOnCell.AddAilment(AilmentID.SpawnMinion, GetEffectDuration(), 0);

        myCell.mCharacterObj = tempChar;
        myCell.mTypeOnCell = TypeOnCell.playerMinion;




    }
}
