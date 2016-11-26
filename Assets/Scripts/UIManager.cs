using UnityEngine;
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
    
    [HideInInspector]
    public int mActiveCharacters;


    IntVector2 mPos;

    TypeOnCell mTypeOnCell;


    void Start ()
    {
        mActiveCharacters = GameManager.sInstance.mCharacters.Length;

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

    }

    public void OnEndTurnDown()
    {
        //end turn
        print("Pressed end turn");

    }

    public void OnCharacter1Down()
    {
        //character 1
        print("Pressed character1");
        if(mActiveCharacters >= 1)
        {
            mPos = GameManager.sInstance.mCharacters[0].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[0]);
            
        }

    }

    public void OnCharacter2Down()
    {
        //character 2
        print("Pressed character2");
        if (mActiveCharacters >= 2)
        {
            mPos = GameManager.sInstance.mCharacters[1].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[1]);

        }
    }

    public void OnCharacter3Down()
    {
        //character 3
        print("Pressed character3");
        if (mActiveCharacters >= 3)
        {
            mPos = GameManager.sInstance.mCharacters[2].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[2]);

        }
    }

    public void OnCharacter4Down()
    {
        //character 4
        print("Pressed character4");
        if (mActiveCharacters >= 4)
        {
            mPos = GameManager.sInstance.mCharacters[3].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[3]);

        }
    }

    public void OnCharacter5Down()
    {
        //character 5
        print("Pressed character5");
        if (mActiveCharacters >= 5)
        {
            mPos = GameManager.sInstance.mCharacters[4].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[4]);

        }
    }

    public void OnCharacter6Down()
    {
        //character 6
        print("Pressed character6");
        if (mActiveCharacters >= 6)
        {
            mPos = GameManager.sInstance.mCharacters[5].mCellPos;
            mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[5]);

        }
    }
}
