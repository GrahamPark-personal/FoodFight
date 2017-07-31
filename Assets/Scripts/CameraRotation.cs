﻿using UnityEngine;
using System.Collections;

public enum Direction
{
    pos1 = 0,
    pos2 = 1,
    pos3 = 2,
    pos4 = 3
}

public class CameraRotation : MonoBehaviour {

    public float mRotationSpeed;

    public Direction mCamDirection;

    int mCurrentDirection = 0;

	void Start ()
    {
        mCurrentDirection = (int)mCamDirection;
    }

    public void RotateLeft()
    {
        if (mCurrentDirection < 3)
        {
            mCurrentDirection++;
        }
        else
        {
            mCurrentDirection = 0;
        }

        mCamDirection = (Direction)mCurrentDirection;
    }

    public void RotateRight()
    {
        if (mCurrentDirection > 0)
        {
            mCurrentDirection--;
        }
        else
        {
            mCurrentDirection = 3;
        }
        mCamDirection = (Direction)mCurrentDirection;
    }

    void Update ()
    {

        //left
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(mCurrentDirection < 3)
            {
                mCurrentDirection++;
            }else
            {
                mCurrentDirection = 0;
            }

            mCamDirection = (Direction)mCurrentDirection;
        }

        //right
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (mCurrentDirection > 0)
            {
                mCurrentDirection--;
            }
            else
            {
                mCurrentDirection = 3;
            }
            mCamDirection = (Direction)mCurrentDirection;
        }

        if (mCamDirection == Direction.pos1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(45, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if(mCamDirection == Direction.pos2)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(135, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mCamDirection == Direction.pos3)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(225, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mCamDirection == Direction.pos4)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(315, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
    }
}
