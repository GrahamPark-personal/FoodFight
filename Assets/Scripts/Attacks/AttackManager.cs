using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackManager : MonoBehaviour
{

    /*
    
        attackList
        RunAllAttacks
        AddAttack
        RemoveAttack

         
    */

    public static AttackManager sInstance = null;


    public Attack mCurrentAttack;

    [HideInInspector]
    public bool mRemoveAttack = true;

    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
    }

    public void RunAttack(IntVector2 pos)
    {

        if (mCurrentAttack != null)
        {

            mCurrentAttack.Execute(pos);
            if (mRemoveAttack)
            {
                RemoveAttack();
            }
        }
        if (mRemoveAttack)
        {
            GameManager.sInstance.CheckLose();
            GameManager.sInstance.CheckWin();

            GameManager.sInstance.mMouseMode = MouseMode.None;
            bool finishCharacter = false;
            if (GameManager.sInstance.mCharacterObj.mMoved)
            {
                finishCharacter = true;
            }
            GameManager.sInstance.mUIManager.RevertHover(finishCharacter);
        }
        else
        {
            mRemoveAttack = true;
        }
        //GameManager.sInstance.ResetSelected();

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

        foreach (GameObject go in GameManager.sInstance.mPreviewBlocks)
        {
            Destroy(go);
        }

        GameManager.sInstance.mPreviewBlocks.Clear();


    }


}
