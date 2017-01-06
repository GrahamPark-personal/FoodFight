﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Button mMenuB;
    public Button mBasicAttack;
    public Button mEndTurn;

    [Space(10)]

    public Button[] mCharFrame;

    [Space(10)]

    public RawImage[] mCharImage;

    [Space(10)]

    public Slider[] mCharHealth;

    [Space(10)]

    [Range(0,1)]
    public float[] mCharSlideAmount;

    [Space(10)]

    public Texture2D[] mCharTexture;

    [Space(10)]

    public Texture2D[] mCharHiddenTexture;

    [HideInInspector]
    public int mActiveCharacters;




    IntVector2 mPos;

    TypeOnCell mTypeOnCell;

    int mCurrentCharacter = 0;

    Texture2D[] mSavedCharImage;


    void Start ()
    {
        mActiveCharacters = GameManager.sInstance.mCharacters.Length;

        mSavedCharImage = new Texture2D[mCharTexture.Length];

        for (int i = 0; i < mCharTexture.Length; i++)
        {
            mSavedCharImage[i] = mCharTexture[i];
        }
    }
	

	void Update ()
    {
        for (int i = 0; i < mCharHealth.Length; i++)
        {
            mCharHealth[i].value = mCharSlideAmount[i];
        }

        for (int i = 0; i < mCharFrame.Length; i++)
        {
            mCharImage[i].texture = mCharTexture[i];

            
        }

        for (int i = 0; i < mCharFrame.Length; i++)
        {
            if(GameManager.sInstance.mCharacters[i].mAttacked && GameManager.sInstance.mCharacters[i].mMoved)
            {
                mCharImage[i].texture = mCharHiddenTexture[i];
            }
            else if(mCharImage[i].texture == mCharHiddenTexture[i])
            {
                mCharImage[i].texture = mCharTexture[i];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnCharacter1Down();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnCharacter2Down();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnCharacter3Down();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnCharacter4Down();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OnCharacter5Down();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            OnCharacter6Down();
        }
    }

    public void OnMenuDown()
    {
        //menu
        print("Pressed Menu");
        Application.Quit();

    }

    public void OnBasicAttackDown()
    {
        //basic attack
        print("Pressed attack");
        GameManager.sInstance.mMouseMode = MouseMode.Attack;
        GameManager.sInstance.ResetSelected();
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);

    }

    public void OnEndTurnDown()
    {
        //end turn
        print("Pressed end turn");
        if(GameManager.sInstance.mGameTurn == GameTurn.Player)
        {
            GameManager.sInstance.FinishPlayerTurn();
        }
        else
        {
            Debug.Log("Cannot finish player turn if it is the enemies");
        }

    }

    public void OnCharacter1Down()
    {
        //character 1
        print("Pressed character1");
        if(mActiveCharacters >= 1)
        {
            SelectCharacter(0);
        }

    }

    public void OnCharacter2Down()
    {
        //character 2
        print("Pressed character2");
        if (mActiveCharacters >= 2)
        {
            SelectCharacter(1);
        }
    }

    public void OnCharacter3Down()
    {
        //character 3
        print("Pressed character3");
        if (mActiveCharacters >= 3)
        {
            SelectCharacter(2);
        }
    }

    public void OnCharacter4Down()
    {
        //character 4
        print("Pressed character4");
        if (mActiveCharacters >= 4)
        {
            SelectCharacter(3);
        }
    }

    public void OnCharacter5Down()
    {
        //character 5
        print("Pressed character5");
        if (mActiveCharacters >= 5)
        {
            SelectCharacter(4);
        }
    }

    public void OnCharacter6Down()
    {
        //character 6
        print("Pressed character6");
        if (mActiveCharacters >= 6)
        {
            SelectCharacter(5);
        }
    }

    public void OnCharEnter1()
    {
        if (mActiveCharacters >= 0)
        {
            MoveCharacterHover(0);
        }
    }

    public void OnCharEnter2()
    {
        if (mActiveCharacters >= 1)
        {
            MoveCharacterHover(1);
        }
    }

    public void OnCharEnter3()
    {
        if (mActiveCharacters >= 2)
        {
            MoveCharacterHover(2);
        }
    }

    public void OnCharEnter4()
    {
        if (mActiveCharacters >= 3)
        {
            MoveCharacterHover(3);
        }
    }

    public void OnCharEnter5()
    {
        if (mActiveCharacters >= 4)
        {
            MoveCharacterHover(4);
        }
    }

    public void OnCharEnter6()
    {
        if (mActiveCharacters >= 5)
        {
            MoveCharacterHover(5);
        }
    }

    public void SelectCharacter(int character)
    {

        mCurrentCharacter = character;
        mPos = GameManager.sInstance.mCharacters[character].mCellPos;

        Vector3 camMovePos = GameManager.sInstance.mCharacters[character].mPosition.position;
        GameManager.sInstance.mCamControl.MoveToPosition(camMovePos);
        mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mMouseMode = MouseMode.Move;
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[character]);
    }

    public void SelectCharacter(IntVector2 mPos)
    {
        Character temp = GameManager.sInstance.mCurrGrid.rows[mPos.x].cols[mPos.y].mCharacterObj;

        for (int i = 0; i < GameManager.sInstance.mCharacters.Length; i++)
        {
            if(temp == GameManager.sInstance.mCharacters[i])
            {
                mCurrentCharacter = i;
                break;
            }
        }
        this.mPos = mPos;

        Vector3 camMovePos = GameManager.sInstance.mCharacters[mCurrentCharacter].mPosition.position;
        GameManager.sInstance.mCamControl.MoveToPosition(camMovePos);
        mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mMouseMode = MouseMode.Move;
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
    }

    void MoveCharacterHover(int character)
    {
        IntVector2 tempPos = GameManager.sInstance.mCharacters[character].mCellPos;
        GameManager.sInstance.MoveCharacterHover(tempPos);
    }

    public void HideCharacterHover()
    {
        GameManager.sInstance.HideCharacterHover(false);
    }
}
