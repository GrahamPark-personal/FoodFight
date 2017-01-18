using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AttackType
{
    Melee = 0,
    Ranged
}

public class Character : MonoBehaviour {

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
