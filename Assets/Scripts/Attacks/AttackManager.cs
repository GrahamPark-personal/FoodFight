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
            int character = (int)GameManager.sInstance.mCharacterObj.mCharacterType;
            if (GameManager.sInstance.mCharacterObj.mMoved)
            {
                finishCharacter = true;

            }
            if(finishCharacter)
            {
                SelectionBar.sInstance.Attacked();
                SelectionBar.sInstance.AttackReset();
                GameManager.sInstance.mMouseMode = MouseMode.None;
            }
            else
            {
                SelectionBar.sInstance.Attacked();
                SelectionBar.sInstance.AttackReset();
                StartCoroutine(WaitForReset(character));
            }
            //TODO:: instead of a whole reset, only revert back to character 1 being selected
            //GameManager.sInstance.mUIManager.RevertHover(finishCharacter);
        }
        else
        {
            mRemoveAttack = true;
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

        foreach (GameObject go in GameManager.sInstance.mPreviewBlocks)
        {
            Destroy(go);
        }

        GameManager.sInstance.mPreviewBlocks.Clear();


    }

    IEnumerator WaitForReset(int character)
    {
        yield return new WaitForSeconds(0.1f);
        SelectionBar.sInstance.SelectCharacter(character);
        GameManager.sInstance.SetSelected(GameManager.sInstance.mCharacters[character].mCellPos, TypeOnCell.character, GameManager.sInstance.mCharacters[character]);
    }


}
