using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



class BlueGreenDuoAttack : Attack
{

    Cell mCell;
    
    public override void Init()
    {

        //TODO Set real values
        CreateID();
        SetHealth(3);
        SetRange(5);
        SetRadius(2);
        SetEffectDuration(2);


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
       
        tempCharacter.speed = tempCharacter.speed * 4;

        tempCharacter.mPath.Enqueue(GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform);
        tempCharacter.mPosPath.Enqueue(pos);

        tempCharacter.mRunPath = true;

        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj = tempCharacter;

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

        GameManager.sInstance.CreateRowEffect(GetStartPos(), pos, effectParm);
        GameManager.sInstance.CreateBananas(GetStartPos(), pos);

        tempCharacter.mCellPos = pos;
        
        
    }




}