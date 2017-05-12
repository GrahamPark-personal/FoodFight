using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



class RedGreenDuoAttack : Attack
{

    Cell mCell;

    public override void Init()
    {

        //TODO Set real values
        CreateID();
        SetDamage(8);
        SetHealth(6);//still have to add functionality
        SetRange(4);
        SetRadius(4);
        SetEffectDuration(1);
        SetDamageDuration(1);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Cross;

        GameManager.sInstance.mCurrentRange = GetRange();

        GameManager.sInstance.mPreviewShape = HoverShape.Row;


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

       EffectParameters effectParm = new EffectParameters();
        effectParm.Effect = cellEffect.Ice;
        effectParm.CellAction = CellActionType.StartOfTurn;
        effectParm.Damage = GetDamage();
        effectParm.Slow = GetSlow();
        effectParm.Health = GetHealth();
        effectParm.Poison = GetPoison();
        effectParm.Taunt = GetTaunt();
        effectParm.EffectDuration = GetEffectDuration();
        effectParm.DamageDuration = GetDamageDuration();
        effectParm.Stun = GetStun();
        effectParm.ID = GetID();



        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj = tempCharacter;

        GameManager.sInstance.CreateRowEffect(GetStartPos(), pos, CellTag.Enchanted, GetDamage());

        tempCharacter.mCellPos = pos;

    }


}