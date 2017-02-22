﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBrownDuoAttack : Attack
{


    Cell mCell;

    public override void Init()
    {
        CreateID();
        SetDamage(5);
        SetHealth(0);
        SetRange(5);
        SetSlow(1);
        SetRadius(2);
        SetAOE(9);
        SetEffectDuration(3);
        SetDamageDuration(3);
        GameManager.sInstance.mAttackShape = AttackShape.Area;
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {
        Character tempCharacter = GameManager.sInstance.mCharacters[3];
        
        //What does this skill do?
        // Give Earth a buff that does the following   
        tempCharacter.AddBuff(BuffID.ThunderCloak, 3, 6, 3);
        
        //The following happens when the player is attacked

        //Returns damage to attacker.
        // If the attacker is melee it gets rooted.     



    }
}