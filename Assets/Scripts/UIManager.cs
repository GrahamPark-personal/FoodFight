using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct CharacterAttackImages
{
    public Texture2D[] images;
}

public class UIManager : MonoBehaviour
{

    [HideInInspector]
    public bool mEnemyPopUpBarShown = false;

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

    //[Range(0, 1)]
    //public float[] mCharSlideAmount;

    [Space(10)]

    public Texture2D[] mCharTexture;

    [Space(10)]

    public Texture2D[] mCharHiddenTexture;

    [HideInInspector]
    public int mActiveCharacters;

    public Character[] mCharacters;

    public RawImage[] mAttackImages;

    public CharacterAttackImages[] mTexturesForAttacks;


    IntVector2 mPos;

    TypeOnCell mTypeOnCell;

    int mCurrentCharacter = 0;

    Texture2D[] mSavedCharImage;


    void Start()
    {
        mActiveCharacters = GameManager.sInstance.mCharacters.Length;

        mSavedCharImage = new Texture2D[mCharTexture.Length];

        for (int i = 0; i < mCharTexture.Length; i++)
        {
            mSavedCharImage[i] = mCharTexture[i];
        }
    }


    void Update()
    {

        //Debug.Log("UI Update Called");
        for (int i = 0; i < mCharHealth.Length; i++)
        {
            //Debug.Log("Health Updated");
            mCharHealth[i].value = mCharacters[i].mHealth;
        }

        for (int i = 0; i < mCharFrame.Length; i++)
        {
            mCharImage[i].texture = mCharTexture[i];
        }

        for (int i = 0; i < mCharFrame.Length; i++)
        {
            if (i < GameManager.sInstance.mCharacters.Length)
            {
                if (GameManager.sInstance.mCharacters[i].mAttacked && GameManager.sInstance.mCharacters[i].mMoved)
                {
                    mCharImage[i].texture = mCharHiddenTexture[i];
                }
                else if (mCharImage[i].texture == mCharHiddenTexture[i])
                {
                    mCharImage[i].texture = mCharTexture[i];
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnCharacter1Down(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnCharacter2Down(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnCharacter3Down(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnCharacter4Down(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OnCharacter5Down(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            OnCharacter6Down(true);
        }
    }

    public void ResetPopUp(bool open)
    {
        if (open)
        {
            mEnemyPopUpBarShown = false;
            //change images

            for (int i = 0; i < mAttackImages.Length; i++)
            {
                mAttackImages[i].texture = mTexturesForAttacks[mCurrentCharacter].images[i];
            }

            StartCoroutine(WaitForReset());
        }
        else
        {
            mEnemyPopUpBarShown = false;
        }
    }

    IEnumerator WaitForReset()
    {
        yield return new WaitForSeconds(0.3f);
        mEnemyPopUpBarShown = true;
    }

    public void OnMenuDown()
    {
        //menu
        SceneManager.LoadScene(0);

    }

    public void OnBasicAttackDown()
    {
        //basic attack
        GameManager.sInstance.mMouseMode = MouseMode.Attack;
        GameManager.sInstance.ResetSelected();
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);

    }

    public void OnEndTurnDown()
    {
        //end turn
        if (GameManager.sInstance.mGameTurn == GameTurn.Player)
        {
            GameManager.sInstance.FinishPlayerTurn();
        }
        else
        {
            Debug.Log("Cannot finish player turn if it is the enemies");
        }

    }

    public void OnCharacter1Down(bool moveCam)
    {
        //character 1
        if (mActiveCharacters >= 1)
        {
            SelectCharacter(0, moveCam);
        }

    }

    public void OnCharacter2Down(bool moveCam)
    {
        //character 2

        if (mActiveCharacters >= 2)
        {
            SelectCharacter(1, moveCam);
        }
    }

    public void OnCharacter3Down(bool moveCam)
    {
        //character 3
        if (mActiveCharacters >= 3)
        {
            SelectCharacter(2, moveCam);
        }
    }

    public void OnCharacter4Down(bool moveCam)
    {
        //character 4
        if (mActiveCharacters >= 4)
        {
            SelectCharacter(3, moveCam);
        }
    }

    public void OnCharacter5Down(bool moveCam)
    {
        //character 5
        if (mActiveCharacters >= 5)
        {
            SelectCharacter(4, moveCam);
        }
    }

    public void OnCharacter6Down(bool moveCam)
    {
        //character 6
        if (mActiveCharacters >= 6)
        {
            SelectCharacter(5, moveCam);
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


    public void OnBasicAbilityDown()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }

    public void OnDuoAbility1Down()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mDualAbilities.mDuoAbility1;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }
    public void OnDuoAbility2Down()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mDualAbilities.mDuoAbility2;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }
    public void OnDuoAbility3Down()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mDualAbilities.mDuoAbility3;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }
    public void OnDuoAbility4Down()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mDualAbilities.mDuoAbility4;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }

    public void OnDuoAbility5Down()
    {
        GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
        Attack temp = GameManager.sInstance.mCharacterObj.mDualAbilities.mDuoAbility5;
        AttackManager.sInstance.SetAttack(temp);
        GameManager.sInstance.ResetSelected();
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
        }
        else
        {
            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
        }
    }

    public void SelectCharacter(int character, bool moveCam)
    {

        mCurrentCharacter = character;
        mPos = GameManager.sInstance.mCharacters[character].mCellPos;

        if (moveCam)
        {
            Vector3 camMovePos = GameManager.sInstance.mCharacters[character].mPosition.position;
            GameManager.sInstance.mCamControl.MoveToPosition(camMovePos);
        }
        mTypeOnCell = TypeOnCell.character;
        GameManager.sInstance.mMouseMode = MouseMode.Move;
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[character]);
    }

    public void SelectCharacter(IntVector2 mNewPos)
    {

        Character temp = null;

        if (GameManager.sInstance.mGameTurn == GameTurn.Player)
        {
            temp = GameManager.sInstance.mCurrGrid.rows[mNewPos.y].cols[mNewPos.x].mCharacterObj;
        }
        else if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            temp = GameManager.sInstance.mBoss;
        }

        if (temp == null)
        {
            Debug.Log("Selecting a null character");
            return;
        }


        for (int i = 0; i < GameManager.sInstance.mCharacters.Length; i++)
        {
            if (temp == GameManager.sInstance.mCharacters[i])
            {
                mCurrentCharacter = i;
                break;
            }
        }

        int number = temp.mCharNumber;
        //int number = GameManager.sInstance.mCurrGrid.rows[mNewPos.y].cols[mNewPos.x].mCharacterObj.mCharNumber;

        switch (number)
        {
            case 0:
                OnCharacter1Down(false);
                break;
            case 1:
                OnCharacter2Down(false);
                break;
            case 2:
                OnCharacter3Down(false);
                break;
            case 3:
                OnCharacter4Down(false);
                break;
            case 4:
                OnCharacter5Down(false);
                break;
            case 5:
                OnCharacter6Down(false);
                break;

        }
    }

    void MoveCharacterHover(int character)
    {
        if (character < GameManager.sInstance.mCharacters.Length)
        {
            IntVector2 tempPos = GameManager.sInstance.mCharacters[character].mCellPos;
            GameManager.sInstance.MoveCharacterHover(tempPos);
        }
    }

    public void HideCharacterHover()
    {
        GameManager.sInstance.HideCharacterHover(false);
    }

    public void Rotate(string direction)
    {

        if (direction == "left")
        {
            GameManager.sInstance.mCamRotation.RotateLeft();
        }
        else if (direction == "right")
        {
            GameManager.sInstance.mCamRotation.RotateRight();
        }

    }

    //debug area

    public void EndEnemyTurn()
    {
        GameManager.sInstance.FinishEnemyTurn();
    }

    public void ChangeEnemyToMove()
    {
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.ChangeEnemyToMove();
        }
    }

    public void ChangeEnemyToBasicAttack()
    {
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.ChangeEnemyToBasicAttack();
        }
    }

    public void ChangeEnemyToAbilityAttack()
    {
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            GameManager.sInstance.ChangeEnemyToBasicAbility();

            GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
            Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
            AttackManager.sInstance.SetAttack(temp);


            GameManager.sInstance.ResetSelected();
        }

    }

}
