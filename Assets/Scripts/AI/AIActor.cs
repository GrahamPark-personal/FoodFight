using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType
{
    AOP,
    Patrol,
    Spawn
}




public class AIActor : MonoBehaviour
{
    #region Variables

    #region public

    public IntVector2 mDesiredDestination;

    public int mActivationRange;

    public AIType mType;

    public IntVector2[] patrolPoints;

    
    [HideInInspector]
    public IntVector2 mCurrentDestination;

    bool activated = false;
    
    #endregion

    #region Private


    IntVector2 mStartPosition;

    AttackType mAttackType;

    IntVector2 mEnemyPos;

    [HideInInspector]
    public Character mCharacter;


    bool mMovingToDestination;

    #endregion

    #endregion

    #region Functions

    void Start()
    {
        mCharacter = GetComponent<Character>();

        mCharacter.mActorComp = this;

        mStartPosition = mCharacter.mCellPos;
        mCurrentDestination = mDesiredDestination;

        mAttackType = mCharacter.mAttackType;

        mMovingToDestination = true;

    }

    public void switchDestination()
    {
        mMovingToDestination = !mMovingToDestination;
        if (mMovingToDestination)
        {
            mCurrentDestination = mDesiredDestination;
        }
        else
        {
            mCurrentDestination = mStartPosition;
        }
    }

    #endregion

}
