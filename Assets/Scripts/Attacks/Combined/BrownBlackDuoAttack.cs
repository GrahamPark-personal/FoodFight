using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownBlackDuoAttack : Attack {


    public override void Init()
    {

        CreateID();
        SetRadius(3);
        SetRange(3);
        SetDamage(3);
        SetEffectDuration(2);
        SetDamageDuration(2);

        SetStartPos(GameManager.sInstance.mSelectedCell);

        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();


    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        Character mCharOnCell = GetComponent<Character>();

        Cell myCell = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x];

        List<Cell> AreaEffect = GameManager.sInstance.GetCellsInRange(pos, GetRadius());

        List<IntVector2> walls = new List<IntVector2>();

        IntVector2 tempVector = new IntVector2();

        tempVector = pos;
        tempVector.y += GetRadius();
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.y -= GetRadius();
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.x += GetRadius();
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.x -= GetRadius();
        walls.Add(tempVector);

        tempVector.x += 1;
        tempVector.y -= 1;
        walls.Add(tempVector);

        tempVector.x += 1;
        tempVector.y -= 1;
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.y -= GetRadius();

        tempVector.x += 1;
        tempVector.y += 1;
        walls.Add(tempVector);

        tempVector.x += 1;
        tempVector.y += 1;
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.x += GetRadius();

        tempVector.x -= 1;
        tempVector.y += 1;
        walls.Add(tempVector);

        tempVector.x -= 1;
        tempVector.y += 1;
        walls.Add(tempVector);

        tempVector = pos;
        tempVector.y += GetRadius();

        tempVector.x -= 1;
        tempVector.y -= 1;
        walls.Add(tempVector);

        tempVector.x -= 1;
        tempVector.y -= 1;
        walls.Add(tempVector);


        EffectParameters WallParm = new EffectParameters();
        WallParm.Effect = cellEffect.Wall;
        WallParm.CellAction = CellActionType.Nothing;
        WallParm.EffectDuration = GetEffectDuration();

        foreach (IntVector2 item in walls)
        {
            if (GameManager.sInstance.IsMovableBlock(item))
            {
                GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x].AddEffect(WallParm);
            }
        }

        EffectParameters effectParm = new EffectParameters();

        effectParm.Effect = cellEffect.Poison;
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

        foreach (Cell item in AreaEffect)
        {
            if (!walls.Contains(item.mPos) && GameManager.sInstance.IsOnGrid(item.mPos))
            {
                item.AddEffect(effectParm);
                if(GameManager.sInstance.mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mTypeOnCell != TypeOnCell.nothing || GameManager.sInstance.mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mTypeOnCell != TypeOnCell.playerMinion)
                {
                    GameManager.sInstance.mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].GetCharacterObject().AddAilment(AilmentID.Poison, GetEffectDuration(), GetDamage());
                }
            }
        }



        //calculate walls
        //add walls
        //add effect to objects excluding walls





    }
}
