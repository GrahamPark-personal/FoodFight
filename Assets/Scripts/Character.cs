using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AttackType
{
    Melee = 0,
    Ranged
}

public enum AilmentID
{
    Stun = 0,
    Poison,
    Slow
}

public class Character : MonoBehaviour {

    struct StatusAilment
    {

        public AilmentID ID;
        public int duration;
        public int turnsPassed;
        public int extra;
    }


    [HideInInspector]
    public Transform mPosition;

    public IntVector2 mCellPos;

    public int mCharNumber;

    public int mHealth;

    public int mDamage;

    public int mMoveDistance;

    public int mDamageDistance;

    public AttackType mAttackType;

    public Direction mDirection;

    public float mRotationSpeed;

    int mCurrDirection;

    int mTotalMove;

    [HideInInspector]
    public Queue<Transform> mPath = new Queue<Transform>();
    public Queue<IntVector2> mPosPath = new Queue<IntVector2>();

    [HideInInspector]
    public Transform mFinalPosition;

    [HideInInspector]
    public bool mRunPath = false;

    Vector3 tempV;
    float speed;

    IntVector2 nextBlock;
    IntVector2 lastBlock;

    [HideInInspector]
    public bool mMoved = false;
    [HideInInspector]
    public bool mAttacked = false;

    bool mMoving = false;

    List<StatusAilment> statusAilments = new List<StatusAilment>();

    public void AddAilment(AilmentID ID, int duration, int extra)
    {
        StatusAilment ailment;
        ailment.ID = ID;
        ailment.duration = duration;
        ailment.turnsPassed = 0;
        ailment.extra = extra;

        for(int i = 0; i < statusAilments.Count; i++)
        {
            if(statusAilments[i].ID == ailment.ID )
            {
                statusAilments.Remove(statusAilments[i]);                
            }
        }

        statusAilments.Add(ailment);
        print("AddedAilement to " + mCharNumber + ", with attack " + ID + ", total ailments are: " + statusAilments.Count);
    }

    public void clearAilments()
    {

        for(int i = 0; i < statusAilments.Count; i++)
        {
            StatusAilment temp = statusAilments[i];
            temp.turnsPassed++;
            statusAilments[i] = temp;
            if(statusAilments[i].duration < statusAilments[i].turnsPassed)
            {            
                statusAilments.Remove(statusAilments[i]);
            }
            
        }


    }

    public void checkAilments()
    {
        for(int i = 0; i < statusAilments.Count; i++)
        {
            if(statusAilments[i].ID == AilmentID.Stun)
            {
                print("did ailment");
                mAttacked = true;
                mMoved = true;
                mMoveDistance = 0;
            }
            if (statusAilments[i].ID == AilmentID.Poison)
            {
                mHealth -= statusAilments[i].extra;
            }
            if (statusAilments[i].ID == AilmentID.Slow)
            {
                mTotalMove -= statusAilments[i].extra;
            }
        }
    }


    void Start ()
    {
        mPosition = transform;
        mFinalPosition = transform;

        mTotalMove = mMoveDistance;

    }

    public void ResetTurn()
    {
        mMoveDistance = mTotalMove;
        mMoved = false;
        mAttacked = false;

        clearAilments();
        checkAilments();

    }

    public void EndCharacterTurn()
    {
        mMoveDistance = 0;
        mMoved = true;
        mAttacked = true;


    }

    public void RemoveMoves(int amount)
    {
        mMoveDistance -= amount;
        if(mMoveDistance <= 0)
        {
            mMoved = true;
        }
    }

	void Update ()
    {
        if (mRunPath)
        {
            Transform tempT;

            speed = GameManager.sInstance.mEntityMoveSpeed;

            if (mPath.Count > 0 && !mMoving)
            {
                lastBlock = nextBlock;
                tempT = mPath.Dequeue();
                nextBlock = mPosPath.Dequeue();
                tempV = tempT.position + new Vector3(0, 1, 0);
                //print(nextBlock.x + "," + nextBlock.y + "|" + lastBlock.x + "," + lastBlock.y + "|||" + mCellPos.x + "," + mCellPos.y);
                mMoving = true;
            }
            if (mPath.Count == 0 && !mMoving)
            {
                mRunPath = false;
            }

        }
        
        if (nextBlock.x > lastBlock.x)
        {
            mDirection = Direction.pos2;
        }
        else if (nextBlock.x < lastBlock.x)
        {
            mDirection = Direction.pos4;
        }
        else if (nextBlock.y > lastBlock.y)
        {
            mDirection = Direction.pos3;
        }
        else if (nextBlock.y < lastBlock.y)
        {
            mDirection = Direction.pos1;
        }
        

        if (transform.position == tempV)
        {
            mMoving = false;
        }

        if (mHealth <= 0)
        {
            //Die
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
        }


        if (mDirection == Direction.pos1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(0, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos2)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(90, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos3)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(180, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos4)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(270, Vector3.up), mRotationSpeed * Time.deltaTime);
        }

    }


    void FixedUpdate()
    {
        if (mMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tempV, speed * Time.deltaTime);
        }
    }
}
