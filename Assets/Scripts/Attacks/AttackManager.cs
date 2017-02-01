using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackManager : MonoBehaviour {

    /*
    
        attackList
        RunAllAttacks
        AddAttack
        RemoveAttack

         
    */

    public static AttackManager sInstance = null;


    public Attack mCurrentAttack;
	

    void Awake()
    {
        if(sInstance == null)
        {
            sInstance = this;
        }
    }
    
    public void RunAttack(IntVector2 pos)
    {
        if(mCurrentAttack != null)
        {
            mCurrentAttack.Execute(pos);
            RemoveAttack();
        }
    }

    public void SetAttack(Attack attack)
    {
        mCurrentAttack = attack;

        mCurrentAttack.Init();
    }
    
    public void RemoveAttack()
    {
        mCurrentAttack.Exit();

        mCurrentAttack = null;
    }


}
