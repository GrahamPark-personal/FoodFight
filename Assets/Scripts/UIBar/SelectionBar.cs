using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    None = 0,
    FirstCharacterSelected = 1,
    Attacking = 2
}


public class SelectionBar : MonoBehaviour
{
    public static SelectionBar sInstance = null;

    public CharacterSelection[] mPortraits;

    UIState mUIState = UIState.None;

    bool mAttacking;

    int mCharacter1;
    int mCharacter2;


    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        int characterLength = GameManager.sInstance.mCharacters.Length;

        for (int i = 0; i < mPortraits.Length; i++)
        {
            if (i < characterLength)
            {
                mPortraits[i].SetLocked(false);
            }
            else
            {
                mPortraits[i].SetLocked(true);
            }
        }

    }


    public UIState GetState()
    {
        return mUIState;
    }

    public bool IsAttacking()
    {
        return mAttacking;
    }

    public bool SelectCharacter(int character)
    {
        if (GameManager.sInstance.mCharacters[character].mMoved && GameManager.sInstance.mCharacters[character].mAttacked)
        {
            mPortraits[character].SetCharacterUsed();
            return false;
        }

        //if none set to mCharacter1
        if (mUIState == UIState.None)
        {
            mPortraits[character].SelectCharacter();
            mCharacter1 = character;

            GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

            IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

            GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0,0.5f,0), partical.transform.rotation);

            mPortraits[character].SetHoverPartical(obj);

            if (mPortraits[character].GetCharacterMode() == CharMode.Attacked)
            {
                TemporaryHideExcept(character);
            }

            mUIState = UIState.FirstCharacterSelected;
            return true;
        }
        else if (mUIState == UIState.FirstCharacterSelected)
        {
            if (mPortraits[character].GetCharacterMode() == CharMode.None)
            {
                mPortraits[character].SelectCharacter();
                mCharacter2 = character;
                mUIState = UIState.Attacking;
                SetAttackUp();

                GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

                IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

                GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 0.5f, 0), partical.transform.rotation);

                mPortraits[character].SetHoverPartical(obj);

                return true;
            }
        }

        return false;
    }

    public void PlayAttackSound(int character)
    {
        if (character >= 0 && character < mPortraits.Length)
        {
            mPortraits[character].PlayAttackSounds();
        }
    }

    public void PlayHitSound(int character)
    {
        if (character >= 0 && character < mPortraits.Length)
        {
            mPortraits[character].PlayHitSounds();
        }
    }

    public bool SelectCharacter(int character, bool playSound, bool onlyFirst)
    {
        if (GameManager.sInstance.mCharacters[character].mMoved && GameManager.sInstance.mCharacters[character].mAttacked)
        {
            mPortraits[character].SetCharacterUsed();
            return false;
        }

        //if none set to mCharacter1
        if (mUIState == UIState.None)
        {
            mPortraits[character].SelectCharacter(playSound, onlyFirst);
            mCharacter1 = character;

            GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

            IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

            GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 0.5f, 0), partical.transform.rotation);

            mPortraits[character].SetHoverPartical(obj);

            if (mPortraits[character].GetCharacterMode() == CharMode.Attacked)
            {
                TemporaryHideExcept(character);
            }

            mUIState = UIState.FirstCharacterSelected;
            return true;
        }
        else if (mUIState == UIState.FirstCharacterSelected)
        {
            if (mPortraits[character].GetCharacterMode() == CharMode.None)
            {
                mPortraits[character].SelectCharacter(playSound, onlyFirst);
                mCharacter2 = character;
                mUIState = UIState.Attacking;
                SetAttackUp();

                GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

                IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

                GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 0.5f, 0), partical.transform.rotation);

                mPortraits[character].SetHoverPartical(obj);

                return true;
            }
        }

        return false;
    }

    public void SetPlayerAttacked(int character)
    {
        mPortraits[character].SetCharacterAttacked();
    }

    public void SetPlayerUsed(int character)
    {
        mPortraits[character].SetCharacterUsed();
    }

    public void TemporaryHideExcept(int character)
    {
        for (int i = 0; i < mPortraits.Length; i++)
        {
            if (i != character)
            {
                mPortraits[i].SetTemporaryUsed();
            }

        }
    }

    public void SetHover(int character, bool hovering)
    {
        if (character < GameManager.sInstance.mCharacters.Length)
        {

            if (hovering && mPortraits[character].GetCharacterMode() != CharMode.Used && mPortraits[character].GetCharacterMode() != CharMode.TemporaryUsed && mPortraits[character].GetCharacterMode() != CharMode.Locked)
            {
                if (mPortraits[character].GetState() == SelectionState.NotSelected)
                {

                    GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

                    IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

                    GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 0.5f, 0), partical.transform.rotation);

                    mPortraits[character].SetHoverPartical(obj);
                }

                mPortraits[character].SetHover(hovering);

            }
            else
            {
                mPortraits[character].SetHover(false);
            }


        }

    }


    public void SetHover(int character, bool hovering, bool onlyFirstHover, bool playSound)
    {
        if (character < GameManager.sInstance.mCharacters.Length)
        {

            if (hovering && mPortraits[character].GetCharacterMode() != CharMode.Used && mPortraits[character].GetCharacterMode() != CharMode.TemporaryUsed && mPortraits[character].GetCharacterMode() != CharMode.Locked)
            {
                if (mPortraits[character].GetState() == SelectionState.NotSelected)
                {

                    GameObject partical = ParticleManager.sInstance.mCharacterParticals[character];

                    IntVector2 pos = GameManager.sInstance.mCharacters[character].mCellPos;

                    GameObject obj = Instantiate(partical, GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].transform.position + new Vector3(0, 0.5f, 0), partical.transform.rotation);

                    mPortraits[character].SetHoverPartical(obj);
                }

                if (onlyFirstHover)
                {
                    if (mPortraits[character].GetState() == SelectionState.NotSelected)
                    {
                        mPortraits[character].SetHover(hovering, playSound);
                    }
                }
                else
                {
                    mPortraits[character].SetHover(hovering, playSound);
                }
            }
            else
            {
                mPortraits[character].SetHover(false, false);
            }


        }

    }

    void SetAttackUp()
    {
        mAttacking = true;
        mPortraits[mCharacter1].SetAttacking(true);
        mPortraits[mCharacter2].SetAttacking(true);
        //set attack up
    }

    public void AttackReset()
    {
        mUIState = UIState.None;
        mAttacking = false;
        foreach (CharacterSelection item in mPortraits)
        {
            item.ResetCharacter();
        }
    }

    public void Attacked()
    {
        AttackReset();
        mPortraits[mCharacter1].SetCharacterAttacked();
    }

    public void RoundCleanUp()
    {
        mUIState = UIState.None;
        mAttacking = false;
        foreach (CharacterSelection item in mPortraits)
        {
            item.NewRoundReset();
        }
    }

    public void ResetToMove()
    {
        AttackReset();
    }


    void Update()
    {
        for (int i = 0; i < GameManager.sInstance.mCharacters.Length; i++)
        {
            if (GameManager.sInstance.mCharacters[i].mMoved && GameManager.sInstance.mCharacters[i].mAttacked)
            {
                mPortraits[i].SetCharacterUsed();
            }
        }
    }

}
