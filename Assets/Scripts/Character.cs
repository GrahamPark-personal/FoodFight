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

    public int mHealth;

    public int mDamage;

    public int mMoveDistance;

    public int mDamageDistance;

    public AttackType mAttackType;

    [HideInInspector]
    public Queue<Transform> mPath = new Queue<Transform>();

    [HideInInspector]
    public Transform mFinalPosition;

    [HideInInspector]
    public bool mRunPath = false;

    Vector3 tempV;
    float speed;

    bool mMoving = false;

    void Start ()
    {
        mPosition = transform;
        mFinalPosition = transform;
	}

	void Update ()
    {
        if (mRunPath)
        {
            Transform tempT;

            print(mPath.Count + " start of char movement ");
            speed = GameManager.sInstance.mEntityMoveSpeed;

            if(mPath.Count > 0 && !mMoving)
            {
                tempT = mPath.Dequeue();
                print(tempT.position.x + " , " + tempT.position.y + " , " + tempT.position.z);
                tempV = tempT.position + new Vector3(0, 1, 0);
                mMoving = true;
            }
            if (mPath.Count == 0 && !mMoving)
            {
                mRunPath = false;
            }

        }

        if (transform.position == tempV)
        {
            mMoving = false;
        }

        if (mHealth <= 0)
        {
            //Die
            GameManager.sInstance.mCurrGrid.rows[mCellPos.x].cols[mCellPos.y].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.x].cols[mCellPos.y].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
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
