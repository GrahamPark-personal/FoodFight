using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//[System.Serializable]
//public struct CharacterAttackImages
//{
//    public Texture2D[] images;
//}

public enum StarLevel
{
    Gold = 2,
    Silver = 1,
    Bronze = 0
}


public class UIManager : MonoBehaviour
{

    [HideInInspector]
    public bool mEnemyPopUpBarShown = false;

    public Button mMenuB;
    public Button mBasicAttack;
    public Button mEndTurn;

    [Space(10)]

    public GameObject[] mCharFrame;

    [Space(10)]

    public RawImage[] mCharImage;

    //[Space(10)]

    //public RawImage[] mCharPopUpImage;

    [Space(10)]

    public Slider[] mCharHealth;

    [Space(10)]

    //[Range(0, 1)]
    //public float[] mCharSlideAmount;

    [Space(10)]

    public Texture2D[] mCharTexture;

    [Space(10)]

    public Texture2D[] mCharHiddenTexture;

    [Space(10)]

    public Texture2D[] mCharHoverTexture;

    [Space(10)]

    public Texture2D[] mCharSelectedTexture;

    [HideInInspector]
    public int mActiveCharacters;

    public Character[] mCharacters;

    public RawImage[] mAttackImages;

    //public CharacterAttackImages[] mTexturesForAttacks;

    public GameObject[] mBubbles;

    public BubbleTextObjects mTextBubble;

    IntVector2 mPos;

    TypeOnCell mTypeOnCell;

    int mCurrentCharacter = 0;

    Texture2D[] mSavedCharImage;

    public RawImage[] mChararcterGlow;
    public RawImage[] mCharacterBigGlow;
    public RawImage[] mCharacterAttack;

    [HideInInspector]
    public int mCurrentSlotSelected = -1;
    [HideInInspector]
    public int mSecondarySlotSelected = -1;

    [HideInInspector]
    public StarLevel mCurrentStar = StarLevel.Gold;

    int mCurrentTurns;

    [Space(10)]
    [Header("Star System")]
    public RawImage mStarImageSlot;

    public Texture2D mGoldStarImage;
    public Texture2D mSilverStar;
    public Texture2D mBronzeStar;

    public int mGoldTurns;
    public int mSilverTurns;

    public Text mStarText;


    int mSavedHover1 = -1;
    int mSavedHover2 = -1;

    int mCurrentHover1 = -1;
    int mCurrentHover2 = -1;

    int mAttackHover1 = -1;
    int mAttackHover2 = -1;
    bool mAttackShown = false;

    //public GameObject mElectricHailstorm;


    void Start()
    {

        ResetAttackHovers();
        ResetCurrentHover1();
        ResetCurrentHover2();
        //ResetAttackHover();
        //SetHoverGlowToInvisible();
        mCurrentTurns = mGoldTurns;

        ResetBubbles();

        mActiveCharacters = GameManager.sInstance.mCharacters.Length;

        mSavedCharImage = new Texture2D[mCharTexture.Length];

        for (int i = 0; i < mCharacters.Length; i++)
        {
            mSavedCharImage[i] = mCharTexture[i];
        }

        for (int i = 0; i < mCharacters.Length; i++)
        {
            mCharImage[i].texture = mCharTexture[i];
        }

        for (int i = 0; i < mCharacters.Length; i++)
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

    }

    //No One Selected

    //Hover
    //reset CurrentHover1, so there is none selected
    //save CurrentHover1 location
    //Move mouse away
    //reset CurrentHover1
    //Click
    //save CurrentHover1 location, now move on to character selected

    //if you revert back:
    //reset currentHover1

    //Character Selected

    //Hover
    //if slot is also currentHover make it double, else then only show the SingleHover
    //save CurrentHover2 location
    //Move mouse away
    //reset CurrentHover2
    //Click
    //reset attackHover
    //set attackHover to CurrentHover1, and CurrentHover2
    //Set AttackSelected to true
    //save currentHover2 location, and move on to Selected Attack

    //if you revert back:
    //reset currentHover2
    //reset attackHover


    //Attack Selected
    //do nothing

    public void SetCurrentHover1(int slot)
    {
        ResetCurrentHover1();

        if (mCharacters[slot].mAttacked && mCharacters[slot].mMoved)
        {
            return;
        }

        mCharImage[slot].texture = mCharHoverTexture[slot];

        mCurrentHover1 = slot;
        mSavedHover1 = slot;
        mChararcterGlow[slot].color = new Color(mChararcterGlow[slot].color.r, mChararcterGlow[slot].color.g, mChararcterGlow[slot].color.b, 100);
    }

    public void SaveCurrentHover1()
    {
        if(mSavedHover1 == -1)
        {
            return;
        }
        if (mCharacters[mSavedHover1].mAttacked && mCharacters[mSavedHover1].mMoved)
        {
            for (int i = 0; i < mCharFrame.Length; i++)
            {
                mCharFrame[i].SetActive(false);
                if (i != mSavedHover1)
                {
                    mAttackImages[i].texture = mCharHiddenTexture[i];
                }
            }
        }
        StartCoroutine(ShowMainGlow());
    }

    public void ResetCurrentHover1()
    {
        for (int i = 0; i < mChararcterGlow.Length; i++)
        {
            mChararcterGlow[i].color = new Color(mChararcterGlow[i].color.r, mChararcterGlow[i].color.g, mChararcterGlow[i].color.b, 0);
        }

        for (int i = 0; i < mCharacters.Length; i++)
        {
            if(mCharacters[i].mAttacked && mCharacters[i].mMoved)
            {
                mCharImage[i].texture = mCharHiddenTexture[i];
            }
            else
            {
                mCharImage[i].texture = mCharTexture[i];
            }
        }

    }

    public void SetCurrentHover2(int slot)
    {
        if (!mAttackShown && !mCharacters[mSavedHover1].mAttacked)
        {
            ResetCurrentHover2();
            if (slot == mSavedHover1)
            {
                mCharacterBigGlow[slot].color = new Color(mCharacterBigGlow[slot].color.r, mCharacterBigGlow[slot].color.g, mCharacterBigGlow[slot].color.b, 100);
            }
            mChararcterGlow[slot].color = new Color(mChararcterGlow[slot].color.r, mChararcterGlow[slot].color.g, mChararcterGlow[slot].color.b, 100);

            //mCharPopUpImage

            if (slot != mSavedHover1)
            {
                mAttackImages[slot].texture = mCharHoverTexture[slot];
            }

            mCurrentHover2 = slot;
            mSavedHover2 = slot;
        }
    }

    public void ResetCurrentHover2()
    {
        if (!mAttackShown)
        {
            if (mSavedHover1 != mSavedHover2 && mSavedHover2 != -1)
            {
                mChararcterGlow[mSavedHover2].color = new Color(mChararcterGlow[mSavedHover2].color.r, mChararcterGlow[mSavedHover2].color.g, mChararcterGlow[mSavedHover2].color.b, 0);
            }
            for (int i = 0; i < mCharacterBigGlow.Length; i++)
            {
                mCharacterBigGlow[i].color = new Color(mCharacterBigGlow[i].color.r, mCharacterBigGlow[i].color.g, mCharacterBigGlow[i].color.b, 0);
            }
            for (int i = 0; i < mCharacters.Length; i++)
            {
                if(i != mSavedHover1)
                {
                    if(mCharacters[i].mAttacked && mCharacters[i].mMoved)
                    {
                        continue;
                    }
                    mAttackImages[i].texture = mCharTexture[i];
                }
            }
        }
    }

    public void SaveCurrentHover2()
    {
        mAttackShown = true;
        ResetAttackHovers();
        //ResetCurrentHover1();
        //ResetCurrentHover2();

        if (mSavedHover1 == -1)
        {
            AttackData data = GetDuoAttack((CharacterType)mSavedHover2);
            mSavedHover1 = data.char1;
            SetCurrentHover1(mSavedHover1);
            for (int i = 0; i < mCharacterBigGlow.Length; i++)
            {
                mCharacterBigGlow[i].color = new Color(mCharacterBigGlow[i].color.r, mCharacterBigGlow[i].color.g, mCharacterBigGlow[i].color.b, 0);
            }
        }
        else if (mSavedHover2 == -1)
        {
            AttackData data = GetDuoAttack((CharacterType)mSavedHover1);
            mSavedHover2 = data.char2;
            SetCurrentHover2(mSavedHover2);
        }

        for (int i = 0; i < mCharacterBigGlow.Length; i++)
        {
            mCharacterBigGlow[i].color = new Color(mCharacterBigGlow[i].color.r, mCharacterBigGlow[i].color.g, mCharacterBigGlow[i].color.b, 0);
        }

        if (mSavedHover1 == mSavedHover2)
        {
            StartCoroutine(ShowBigGlow());
        }

        for (int i = 0; i < mCharFrame.Length; i++)
        {
            mCharFrame[i].SetActive(false);
        }

        mAttackImages[mSavedHover2].texture = mCharSelectedTexture[mSavedHover2];

        SetAttackHover(mSavedHover1, mSavedHover2);
    }

    public void ResetAttackHovers()
    {
        for (int i = 0; i < mCharacterAttack.Length; i++)
        {
            mCharacterAttack[i].color = new Color(mCharacterAttack[i].color.r, mCharacterAttack[i].color.g, mCharacterAttack[i].color.b, 0);
        }
    }

    public void SetAttackHover(int slot1, int slot2)
    {

        mCharacterAttack[slot1].color = new Color(mCharacterAttack[slot1].color.r, mCharacterAttack[slot1].color.g, mCharacterAttack[slot1].color.b, 100);
        mCharacterAttack[slot2].color = new Color(mCharacterAttack[slot2].color.r, mCharacterAttack[slot2].color.g, mCharacterAttack[slot2].color.b, 100);
    }

    public void RevertHover(bool finishedCharacter)
    {
        StartCoroutine(DelayCleanUp(finishedCharacter));
        //StartCoroutine(DelayCleanUp());
    }

    IEnumerator DelayCleanUp(bool characterFinished)
    {
        yield return new WaitForSeconds(0.1f);

        mCurrentHover2 = -1;
        mSavedHover2 = -1;
        mCurrentHover1 = -1;
        mSavedHover1 = -1;
        mAttackHover1 = -1;
        mAttackHover2 = -1;
        ResetAttackHovers();
        ResetCurrentHover1();
        ResetCurrentHover2();

        ResetPopUp(false);

        mAttackShown = false;

        for (int i = 0; i < mChararcterGlow.Length; i++)
        {
            mChararcterGlow[i].color = new Color(mChararcterGlow[i].color.r, mChararcterGlow[i].color.g, mChararcterGlow[i].color.b, 0);
            mCharacterBigGlow[i].color = new Color(mCharacterBigGlow[i].color.r, mCharacterBigGlow[i].color.g, mCharacterBigGlow[i].color.b, 0);
        }


        for (int i = 0; i < mCharacters.Length; i++)
        {

            if (mCharacters[i].mAttacked && mCharacters[i].mMoved)
            {
                mAttackImages[i].texture = mCharHiddenTexture[i];
                mCharFrame[i].SetActive(false);
            }
            else
            {
                mCharFrame[i].SetActive(true);
            }
        }
        GameManager.sInstance.mMouseMode = MouseMode.None;
        if (characterFinished)
        {
            GameManager.sInstance.mCharacterObj = null;
            GameManager.sInstance.ResetSelected();
        }
        else
        {
            if (GameManager.sInstance.mCharacterObj != null)
            {
                SelectCharacter(GameManager.sInstance.mCharacterObj.mCellPos);
            }
        }

    }



    IEnumerator ShowMainGlow()
    {
        yield return new WaitForSeconds(0.1f);
        mChararcterGlow[mSavedHover1].color = new Color(mChararcterGlow[mSavedHover1].color.r, mChararcterGlow[mSavedHover1].color.g, mChararcterGlow[mSavedHover1].color.b, 100);
    }

    IEnumerator ShowBigGlow()
    {
        yield return new WaitForSeconds(0.1f);
        mCharacterBigGlow[mSavedHover2].color = new Color(mCharacterBigGlow[mSavedHover2].color.r, mCharacterBigGlow[mSavedHover2].color.g, mCharacterBigGlow[mSavedHover2].color.b, 100);
    }


    //public void ResetAttackHover()
    //{
    //    for (int i = 0; i < mCharacterAttack.Length; i++)
    //    {
    //        mCharacterAttack[i].color = new Color(mCharacterAttack[i].color.r, mCharacterAttack[i].color.g, mCharacterAttack[i].color.b, 0);
    //    }
    //}

    //public void SetAttackForCharacter(int charSlot)
    //{
    //    mCharacterAttack[charSlot].color = new Color(mChararcterGlow[charSlot].color.r, mChararcterGlow[charSlot].color.g, mChararcterGlow[charSlot].color.b, 100.0f);
    //    if(mCurrentSlotSelected != -1)
    //    {
    //        mCharacterAttack[mCurrentSlotSelected].color = new Color(mChararcterGlow[mCurrentSlotSelected].color.r, mChararcterGlow[mCurrentSlotSelected].color.g, mChararcterGlow[mCurrentSlotSelected].color.b, 100.0f);
    //    }
    //}

    //public void SetHoverGlowToInvisible()
    //{
    //    for (int i = 0; i < mChararcterGlow.Length; i++)
    //    {
    //        if(i != mCurrentSlotSelected)
    //        {
    //            mChararcterGlow[i].color = new Color(mChararcterGlow[i].color.r, mChararcterGlow[i].color.g, mChararcterGlow[i].color.b, 0);
    //        }
    //    }
    //    for (int i = 0; i < mCharacterBigGlow.Length; i++)
    //    {
    //        if (i != mSecondarySlotSelected)
    //        {
    //            mCharacterBigGlow[i].color = new Color(mCharacterBigGlow[i].color.r, mCharacterBigGlow[i].color.g, mCharacterBigGlow[i].color.b, 0);
    //        }
    //    }

    //}

    //public void HoverOver(int charSlot)
    //{
    //    SetHoverGlowToInvisible();
    //    if(charSlot == mCurrentSlotSelected)
    //    {
    //        mCharacterBigGlow[charSlot].color = new Color(mCharacterBigGlow[charSlot].color.r, mCharacterBigGlow[charSlot].color.g, mCharacterBigGlow[charSlot].color.b, 70.0f);
    //    }

    //    mChararcterGlow[charSlot].color = new Color(mChararcterGlow[charSlot].color.r, mChararcterGlow[charSlot].color.g, mChararcterGlow[charSlot].color.b, 70.0f);
    //}

    //public void SelectHoverSlot(int charSlot)
    //{
    //    if(mCurrentSlotSelected == -1)
    //    {
    //        StartCoroutine(WaitToShowGlow(charSlot));
    //    }
    //    else if(mSecondarySlotSelected == -1)
    //    {
    //        StartCoroutine(WaitToShowSecondGlow(charSlot));
    //    }
    //}

    //IEnumerator WaitToShowSecondGlow(int charSlot)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    mSecondarySlotSelected = charSlot;
    //    mCharacterBigGlow[charSlot].color = new Color(mCharacterBigGlow[charSlot].color.r, mCharacterBigGlow[charSlot].color.g, mCharacterBigGlow[charSlot].color.b, 100.0f);
    //}

    //IEnumerator WaitToShowGlow(int charSlot)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    mCurrentSlotSelected = charSlot;
    //    mChararcterGlow[charSlot].color = new Color(mChararcterGlow[charSlot].color.r, mChararcterGlow[charSlot].color.g, mChararcterGlow[charSlot].color.b, 100.0f);
    //}

    public void IncrementTurn()
    {
        mCurrentTurns--;
        if (mCurrentTurns <= 0)
        {
            switch (mCurrentStar)
            {
                case StarLevel.Gold:
                    mCurrentStar = StarLevel.Silver;
                    mCurrentTurns = mSilverTurns;
                    mStarImageSlot.texture = mSilverStar;
                    break;
                case StarLevel.Silver:
                    mCurrentStar = StarLevel.Bronze;
                    mStarImageSlot.texture = mBronzeStar;
                    break;
            }
        }
    }

    void ResetBubbles()
    {
        if (mBubbles.Length > 0)
        {
            foreach (GameObject item in mBubbles)
            {
                if (item != null)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    void AcvivateBubble(int pos)
    {
        if (pos >= 0 && pos <= mBubbles.Length)
        {
            if (pos < mCharTexture.Length && mBubbles[pos])
            {
                mBubbles[pos].GetComponentInChildren<Text>().text = mTextBubble.mBubbletext[mCurrentCharacter].mPlayer[pos];
                mBubbles[pos].SetActive(true);
            }
        }
    }

    void Update()
    {
        if (mCurrentStar != StarLevel.Bronze)
        {
            mStarText.text = "" + mCurrentTurns;
        }
        else
        {
            mStarText.text = "";
        }
        //Debug.Log("UI Update Called");
        for (int i = 0; i < mCharacters.Length; i++)
        {
            //Debug.Log("Health Updated");
            mCharHealth[i].value = mCharacters[i].mHealth;
        }

        if(Input.GetKeyDown(KeyCode.End))
        {
            OnEndTurnDown();
        }

        //for (int i = 0; i < mCharacters.Length; i++)
        //{
        //    mCharImage[i].texture = mCharTexture[i];
        //}

        //for (int i = 0; i < mCharacters.Length; i++)
        //{
        //    if (i < GameManager.sInstance.mCharacters.Length)
        //    {
        //        if (GameManager.sInstance.mCharacters[i].mAttacked && GameManager.sInstance.mCharacters[i].mMoved)
        //        {
        //            mCharImage[i].texture = mCharHiddenTexture[i];
        //        }
        //        else if (mCharImage[i].texture == mCharHiddenTexture[i])
        //        {
        //            mCharImage[i].texture = mCharTexture[i];
        //        }
        //    }
        //}

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
            //change images

            for (int i = 0; i < mCharacters.Length; i++)
            {
                if(i != mCurrentHover1)
                {
                    if(!mCharacters[i].mAttacked && !mCharacters[i].mMoved)
                    {
                        mAttackImages[i].texture = mCharTexture[i];
                    }
                    else
                    {
                        mAttackImages[i].texture = mCharHiddenTexture[i];
                    }

                }
                else
                {
                    mAttackImages[i].texture = mCharSelectedTexture[i];
                }
            }


            for (int i = 0; i < mCharacters.Length; i++)
            {
                if (mCharacters[i].mAttacked && mCharacters[i].mAttacked && mSavedHover1 != i)
                {
                    //mAttackImages[i].texture = mCharHiddenTexture[i];
                    mCharFrame[i].SetActive(false);
                }
                else
                {
                    mCharFrame[i].SetActive(true);
                }
            }

            mEnemyPopUpBarShown = true;
            ResetBubbles();
        }
        else
        {
            Debug.Log("Bar not shown");
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
        if (GameManager.sInstance.mCharacterObj != null)
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.Attack;
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);

            }
        }

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

    public void ShowBubble(int pos)
    {
        ResetBubbles();
        AcvivateBubble(pos);
    }

    public void HideBubbles()
    {
        ResetBubbles();
    }

    public void OnCharacter1Down(bool moveCam)
    {
        //character 1
        if (mActiveCharacters >= 1)
        {
            if (mCharacters[0].mAttacked && mCharacters[0].mMoved)
            {
                return;
            }
            SelectCharacter(0, moveCam);
        }

    }

    public void OnCharacter2Down(bool moveCam)
    {
        //character 2

        if (mActiveCharacters >= 2)
        {
            if (mCharacters[1].mAttacked && mCharacters[1].mMoved)
            {
                return;
            }
            SelectCharacter(1, moveCam);
        }
    }

    public void OnCharacter3Down(bool moveCam)
    {
        //character 3
        if (mActiveCharacters >= 3)
        {
            if (mCharacters[2].mAttacked && mCharacters[2].mMoved)
            {
                return;
            }
            SelectCharacter(2, moveCam);
        }
    }

    public void OnCharacter4Down(bool moveCam)
    {
        //character 4
        if (mActiveCharacters >= 4)
        {
            if (mCharacters[3].mAttacked && mCharacters[3].mMoved)
            {
                return;
            }
            SelectCharacter(3, moveCam);
        }
    }

    public void OnCharacter5Down(bool moveCam)
    {
        //character 5
        if (mActiveCharacters >= 5)
        {
            if (mCharacters[4].mAttacked && mCharacters[4].mMoved)
            {
                return;
            }
            SelectCharacter(4, moveCam);
        }
    }

    public void OnCharacter6Down(bool moveCam)
    {
        //character 6
        if (mActiveCharacters >= 6)
        {
            if (mCharacters[5].mAttacked && mCharacters[5].mMoved)
            {
                return;
            }
            SelectCharacter(5, moveCam);
        }
    }

    public void OnCharEnter1()
    {
        if (mActiveCharacters >= 0)
        {
            if (mCharacters[0].mAttacked && mCharacters[0].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(0);
        }
    }

    public void OnCharEnter2()
    {
        if (mActiveCharacters >= 1)
        {
            if (mCharacters[1].mAttacked && mCharacters[1].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(1);
        }
    }

    public void OnCharEnter3()
    {
        if (mActiveCharacters >= 2)
        {
            if (mCharacters[2].mAttacked && mCharacters[2].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(2);
        }
    }

    public void OnCharEnter4()
    {
        if (mActiveCharacters >= 3)
        {
            if (mCharacters[3].mAttacked && mCharacters[3].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(3);
        }
    }

    public void OnCharEnter5()
    {
        if (mActiveCharacters >= 4)
        {
            if (mCharacters[4].mAttacked && mCharacters[4].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(4);
        }
    }

    public void OnCharEnter6()
    {
        if (mActiveCharacters >= 5)
        {
            if (mCharacters[5].mAttacked && mCharacters[5].mMoved)
            {
                mCurrentHover1 = -1;
                mSavedHover1 = -1;
                return;
            }
            MoveCharacterHover(5);
        }
    }

    public struct AttackData
    {
        public Attack atk;
        public int char1;
        public int char2;
    }


    private AttackData GetDuoAttack(CharacterType characterType)
    {
        DualAbilities currAbilities = GameManager.sInstance.mCharacterObj.mDualAbilities;
        AttackData atkData = new AttackData();

        atkData.char1 = (int)characterType;
        atkData.char2 = (int)GameManager.sInstance.mCharacterObj.mCharacterType;

        if (((currAbilities.ability1Character1 == characterType) || (currAbilities.ability1Character1 == GameManager.sInstance.mCharacterObj.mCharacterType))
            && ((currAbilities.ability1Character2 == characterType) || (currAbilities.ability1Character2 == GameManager.sInstance.mCharacterObj.mCharacterType)))
        {
            //ability = 1
            atkData.atk = currAbilities.mDuoAbility1;
            GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[(int)characterType].mDualAbilities.mAttackPartical1;

            return atkData;
        }
        if (((currAbilities.ability2Character1 == characterType) || (currAbilities.ability2Character1 == GameManager.sInstance.mCharacterObj.mCharacterType))
            && ((currAbilities.ability2Character2 == characterType) || (currAbilities.ability2Character2 == GameManager.sInstance.mCharacterObj.mCharacterType)))
        {
            //ability = 2
            atkData.atk = currAbilities.mDuoAbility2;
            GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[(int)characterType].mDualAbilities.mAttackPartical2;
            return atkData;
        }
        if (((currAbilities.ability3Character1 == characterType) || (currAbilities.ability3Character1 == GameManager.sInstance.mCharacterObj.mCharacterType))
            && ((currAbilities.ability3Character2 == characterType) || (currAbilities.ability3Character2 == GameManager.sInstance.mCharacterObj.mCharacterType)))
        {
            //ability = 3
            atkData.atk = currAbilities.mDuoAbility3;
            GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[(int)characterType].mDualAbilities.mAttackPartical3;
            return atkData;
        }
        if (((currAbilities.ability4Character1 == characterType) || (currAbilities.ability4Character1 == GameManager.sInstance.mCharacterObj.mCharacterType))
            && ((currAbilities.ability4Character2 == characterType) || (currAbilities.ability4Character2 == GameManager.sInstance.mCharacterObj.mCharacterType)))
        {
            //ability = 4
            atkData.atk = currAbilities.mDuoAbility4;
            GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[(int)characterType].mDualAbilities.mAttackPartical4;
            return atkData;
        }
        if (((currAbilities.ability5Character1 == characterType) || (currAbilities.ability5Character1 == GameManager.sInstance.mCharacterObj.mCharacterType))
            && ((currAbilities.ability5Character2 == characterType) || (currAbilities.ability5Character2 == GameManager.sInstance.mCharacterObj.mCharacterType)))
        {
            //ability = 5
            atkData.atk = currAbilities.mDuoAbility5;
            GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[(int)characterType].mDualAbilities.mAttackPartical5;
            return atkData;
        }

        return atkData;
    }

    public void OnBasicAbilityDown()
    {
        Debug.Log("ability test if either attack or ability attack mouse" + (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack));

        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Yellow)
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else
        {
            //find the character, and do the duo ability associated with that character
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Yellow);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }


                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }

    public void OnDuoAbility1Down()
    {
        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Blue)
        {
            //do basic ability for blue
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else // do the duo ability associated
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Blue);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }

                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }
    public void OnDuoAbility2Down()
    {
        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Red)
        {
            //do basic ability for brown
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else // do the duo ability associated
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Red);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }

                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }
    public void OnDuoAbility3Down()
    {

        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Brown)
        {
            //do basic ability for red
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else // do the duo ability associated
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Brown);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }

                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }
    public void OnDuoAbility4Down()
    {

        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Green)
        {
            //do basic ability for green
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else // do the duo ability associated
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Green);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }

                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }

    public void OnDuoAbility5Down()
    {

        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }

        if (GameManager.sInstance.mCharacterObj.mCharacterType == CharacterType.Black)
        {
            //do basic ability for black
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {
                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                Attack temp = GameManager.sInstance.mCharacterObj.mBasicAbility;
                AttackManager.sInstance.SetAttack(temp);
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mCurrentPartical = GameManager.sInstance.mCharacters[mCurrentCharacter].mBasicPartical;

                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacterObj);
                }
                else
                {
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[mCurrentCharacter]);
                }
            }
        }
        else // do the duo ability associated
        {
            if (GameManager.sInstance.mCharacterObj.mAttacked == false)
            {

                AttackData nAtkData = GetDuoAttack(CharacterType.Black);

                int char1 = nAtkData.char1;
                int char2 = nAtkData.char2;

                if (GameManager.sInstance.mCharacters[char1].mAttacked || GameManager.sInstance.mCharacters[char2].mAttacked)
                {
                    return;
                }

                GameManager.sInstance.mMouseMode = MouseMode.AbilityAttack;
                //find the ability associated with both of the characters.s
                //in this case it is whatever the character is mixed with yellow.
                //

                AttackManager.sInstance.SetAttack(nAtkData.atk);
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
        }
    }

    public void SelectCharacter(int character, bool moveCam)
    {

        if (mCharacters[character].mAttacked && mCharacters[character].mMoved)
        {
            ResetCurrentHover1();
            return;
        }
        if (GameManager.sInstance.mMouseMode == MouseMode.Attack || GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack)
        {
            return;
        }


        Debug.Log("Character: " + character);

        SetCurrentHover1(character);
        SaveCurrentHover1();

        mCurrentCharacter = character;
        mPos = GameManager.sInstance.mCharacters[character].mCellPos;

        if (moveCam)
        {
            Vector3 camMovePos = GameManager.sInstance.mCharacters[character].mPosition.position;
            GameManager.sInstance.mCamControl.MoveToPosition(camMovePos);
        }

        ResetPopUp(true);

        mTypeOnCell = TypeOnCell.character;

        GameManager.sInstance.mMouseMode = MouseMode.Move;
        GameManager.sInstance.SetSelected(mPos, mTypeOnCell, GameManager.sInstance.mCharacters[character]);
    }

    public void SelectCharacter(IntVector2 mNewPos)
    {

        for (int i = 0; i < GameManager.sInstance.mCharacters.Length; i++)
        {
            if (GameManager.sInstance.mCharacters[i].mCellPos.x == mNewPos.x && GameManager.sInstance.mCharacters[i].mCellPos.y == mNewPos.y)
            {

                //ResetCurrentHover1();
                //MoveCharacterHover(i);
                //SetCurrentHover1(i);
                //SaveCurrentHover1();
                //SelectCharacter(i, false);
                if (mCharacters[i].mAttacked && mCharacters[i].mMoved)
                {
                    return;
                }

                SelectCharacter(i, false);
                SetCurrentHover1(i);
                SaveCurrentHover1();
                return;
            }
        }
        return;
        //Character temp = null;

        //if (GameManager.sInstance.mGameTurn == GameTurn.Player)
        //{
        //    temp = GameManager.sInstance.mCurrGrid.rows[mNewPos.y].cols[mNewPos.x].mCharacterObj;
        //}
        //else if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        //{
        //    temp = GameManager.sInstance.mBoss;
        //}

        //if (temp == null)
        //{
        //    Debug.Log("Selecting a null character");
        //    return;
        //}


        //for (int i = 0; i < GameManager.sInstance.mCharacters.Length; i++)
        //{
        //    if (temp == GameManager.sInstance.mCharacters[i])
        //    {
        //        mCurrentCharacter = i;
        //        break;
        //    }
        //}

        //int number = temp.mCharNumber;
        ////int number = GameManager.sInstance.mCurrGrid.rows[mNewPos.y].cols[mNewPos.x].mCharacterObj.mCharNumber;

        //switch (number)
        //{
        //    case 0:
        //        OnCharacter1Down(false);
        //        break;
        //    case 1:
        //        OnCharacter2Down(false);
        //        break;
        //    case 2:
        //        OnCharacter3Down(false);
        //        break;
        //    case 3:
        //        OnCharacter4Down(false);
        //        break;
        //    case 4:
        //        OnCharacter5Down(false);
        //        break;
        //    case 5:
        //        OnCharacter6Down(false);
        //        break;

        //}
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
