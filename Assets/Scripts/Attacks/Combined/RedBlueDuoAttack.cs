
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



class RedBlueDuoAttack : Attack
{

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values

        CreateID();
        SetRange(7);
        SetDamage(4);
        SetRadius(2);
        SetSlow(2);
        SetEffectDuration(1);
        SetDamageDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mTargetChars[0] = GameManager.sInstance.mCharacters[1];
        GameManager.sInstance.mTargetChars[1] = GameManager.sInstance.mCharacters[2];

        GameManager.sInstance.mAttackShape = AttackShape.OtherCharacter;

        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

    }


    public override void Exit()
    {
        GameManager.sInstance.mAttackShape = AttackShape.Area;
    }



    public override void Execute(IntVector2 pos)
    {

        //TODO:: fix after click bug for this and heal

        mCell = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x];

        Character BlueMage = GameManager.sInstance.mCharacters[1];
        Character RedMage = GameManager.sInstance.mCharacters[2];

        Character StartCharacter = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;
        Character OtherCharacter = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj;

        //BlueMage.GetComponent<MeshRenderer>().enabled = false;
        //RedMage.GetComponent<MeshRenderer>().enabled = false;


        StartCharacter.mCellPos = pos;
        StartCharacter.transform.position = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 1, 0);

        OtherCharacter.mCellPos = GetStartPos();
        OtherCharacter.transform.position = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].transform.position + new Vector3(0, 1, 0);


        //BlueMage.GetComponent<MeshRenderer>().enabled = true;
        //RedMage.GetComponent<MeshRenderer>().enabled = true;



        List<IntVector2> BlueNeighborCells = new List<IntVector2>();
        GetNeighbors(BlueMage, BlueNeighborCells);

        List<IntVector2> RedNeighborCells = new List<IntVector2>();
        GetNeighbors(RedMage, RedNeighborCells);


        CheckNeighbors(BlueNeighborCells, AilmentID.Slow);


        CheckNeighbors(RedNeighborCells, AilmentID.None);

        
            string part = GameManager.sInstance.mCurrentPartical;

        if(!RedMage.mCellPos.Equals(pos))
        {
            //red
            ParticleManager.sInstance.SpawnPartical(part, RedMage.transform, RedMage.transform, false);
        }
        else
        {
            //blue
            GameManager.sInstance.mUIManager.GetDuoAttack(BlueMage.mCharacterType);
            ParticleManager.sInstance.SpawnPartical(part, BlueMage.transform, BlueMage.transform, false);
        }

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

    public void CheckNeighbors(List<IntVector2> neighbors, AilmentID ID)
    {

        foreach (var cellPos in neighbors)
        {
            if (GameManager.sInstance.IsOnGrid(cellPos))
            {

                Character nextEnemy = GameManager.sInstance.mCurrGrid.rows[cellPos.y].cols[cellPos.x].mEnemyObj;

                if (nextEnemy != null)
                {
                    if (ID == AilmentID.Slow)
                    {
                        nextEnemy.mMoveDistance -= GetSlow();
                    }
                    else if (ID == AilmentID.None)
                    {
                        nextEnemy.Damage(gameObject.GetComponent<Character>(), GetDamage());
                    }
                }
            }
        }

    }
}