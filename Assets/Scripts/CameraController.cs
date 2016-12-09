﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float mMoveSpeed;

    public float mCharacterSnapSpeed;

    public Vector3 mMoveOffset;

    Vector2 mMousePosition;

    Rigidbody mRigidBody;

    bool mMovingToCharacter = false;

	void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        mMousePosition = new Vector2(0, 0);
        mRigidBody = GetComponent<Rigidbody>();
    }
	
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        mRigidBody.AddRelativeForce(new Vector3(mMousePosition.y,0, mMousePosition.x) * mMoveSpeed * Time.deltaTime);
	}

    public void RightWall()
    {
        print("mouse in right wall");
        mMousePosition = new Vector2(1,0);
    }

    public void LeftWall()
    {
        print("mouse in left wall");
        mMousePosition = new Vector2(-1, 0);
    }

    public void UpWall()
    {
        print("mouse in up wall");
        mMousePosition = new Vector2(0, -1);
    }

    public void DownWall()
    {
        print("mouse in down wall");
        mMousePosition = new Vector2(0, 1);
    }


    public void TopRight()
    {
        print("mouse in TopRight");
        mMousePosition = new Vector2(1, -1);
    }

    public void TopLeft()
    {
        print("mouse in TopLeft");
        mMousePosition = new Vector2(-1, -1);
    }

    public void BottomRight()
    {
        print("mouse in BottomRight");
        mMousePosition = new Vector2(1, 1);
    }

    public void BottomLeft()
    {
        print("mouse in BottomLeft");
        mMousePosition = new Vector2(-1, 1);
    }


    public void ExitRightWall()
    {
        print("mouse exited right wall");
        mMousePosition = new Vector2(0, 0);
    }
    public void ExitLeftWall()
    {
        print("mouse exited left wall");
        mMousePosition = new Vector2(0, 0);
    }

    public void ExitUpWall()
    {
        print("mouse exited up wall");
        mMousePosition = new Vector2(0, 0);
    }

    public void ExitDownWall()
    {
        print("mouse exited down wall");
        mMousePosition = new Vector2(0, 0);
    }


    public void ExitTopRight()
    {
        print("mouse exited in TopRight");
        mMousePosition = new Vector2(0, 0);
    }

    public void ExitTopLeft()
    {
        print("mouse exited in TopLeft");
        mMousePosition = new Vector2(0, 0);
    }

    public void ExitBottomRight()
    {
        print("mouse exited in BottomRight");
        mMousePosition = new Vector2(0, 0);
    }

    public void ExitBottomLeft()
    {
        print("mouse exited in BottomLeft");
        mMousePosition = new Vector2(0, 0);
    }

    public void MoveToPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x + mMoveOffset.x, pos.y + mMoveOffset.y, pos.z + mMoveOffset.z);
    }
}
