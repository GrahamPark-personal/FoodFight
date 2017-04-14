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

    #endregion

    #region Private

    IntVector2 mCurrentDestination;

    IntVector2 mStartPosition;

    AttackType mAttackType;

    IntVector2 mEnemyPos;

    Character mCharacter;

    bool mMovingToDestination;

    #endregion

    #endregion

    #region Functions

    void Start()
    {
        mCharacter = GetComponent<Character>();

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
