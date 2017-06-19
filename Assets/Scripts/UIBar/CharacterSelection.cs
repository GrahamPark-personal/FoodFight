using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectionState
{
    NotSelected = 0, //no glow
    FirstSelection = 1, //one glow
    SecondSelection = 2 //two glows
}

public enum ImageState
{
    Idle = 0, //image is idle
    Hover = 1, //image is hover
    Selected = 2, //image is selected
    Unavailable = 3, //image is unavailable
    Locked = 4 //image is locked
}

public enum CharMode
{
    None = 0, //character is fully accessible
    Attacked = 1, //character can be selected, but not for attack
    Used = 2, //character cannot be selected (is unavailable)
    Locked = 3, //character cannot be seen, or activated/accessed in any way
    TemporaryUsed = 4
}

[System.Serializable]
public struct StandaloneSound
{
    public string mName;
    public AudioClip mAudio;
    [Space(10)]
    public bool mUseDefaultSettings;
    public AudioSetting mSettings;
}


public class CharacterSelection : MonoBehaviour
{


    public string mName;

    CharMode mCharacterMode = CharMode.None; //for edge cases

    SelectionState mSelectionState = SelectionState.NotSelected; //state of this selection(for what it should show)

    ImageState mImageState = ImageState.Idle; //(state of image)

    public Texture[] mPortraits; //The textures for idle,Hover, and selected , at pos 3 is the unavailable, and at 4 is the locked image

    public RawImage mBasePortrait; //the rawImage that this is connected to

    public RawImage mBaseGlow; //smaller glow
    public RawImage mSecondaryGlow; //larger glow

    public RawImage mAttackingImage; //to show they are apart of the attack

    [Space(10)]
    public StandaloneSound[] mTalkingSounds;
    public StandaloneSound[] mSelectedSounds;

    [Space(10)]
    public StandaloneSound[] mAttackingSounds;
    public StandaloneSound[] mGettingHitSounds;


    bool mHovering = false; // makes glow one up from current selection state
    bool mAttacking = false; // makes it have an attack image

    CharMode mStoredMode;

    GameObject mHoverObject;

    GameObject mCurrentSoundObject;
    int mLastSound = -1;

    void Start()
    {

    }

    public void PlayHitSounds()
    {
        StandaloneSound clip;

        int audioLength = mGettingHitSounds.Length;

        if (audioLength > 0)
        {
            if (audioLength > 1)
            {
                int rnd = mLastSound;


                while (rnd == mLastSound)
                {
                    rnd = Random.Range(0, (audioLength - 1));
                }

                clip = mGettingHitSounds[rnd];
            }
            else
            {
                clip = mGettingHitSounds[0];
            }

            if (clip.mUseDefaultSettings)
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform);
            }
            else
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform, clip.mSettings);
            }
        }
    }

    public void PlayAttackSounds()
    {
        StandaloneSound clip;

        int audioLength = mAttackingSounds.Length;

        if (audioLength > 0)
        {
            if (audioLength > 1)
            {
                int rnd = mLastSound;


                while (rnd == mLastSound)
                {
                    rnd = Random.Range(0, (audioLength - 1));
                }

                clip = mAttackingSounds[rnd];
            }
            else
            {
                clip = mAttackingSounds[0];
            }

            if (clip.mUseDefaultSettings)
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform);
            }
            else
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform, clip.mSettings);
            }
        }
    }

    public void SetHoverPartical(GameObject partical)
    {
        if (mHoverObject != null)
        {
            Destroy(partical);
        }
        else
        {
            mHoverObject = partical;
        }
    }


    public void SetTemporaryUsed()
    {
        if (mCharacterMode != CharMode.Locked)
        {
            mStoredMode = mCharacterMode;
            mCharacterMode = CharMode.TemporaryUsed;
        }
    }

    public void SetCharacterAttacked()
    {
        mCharacterMode = CharMode.Attacked;
    }

    public void SetCharacterUsed()
    {
        mCharacterMode = CharMode.Used;
    }

    public CharMode GetCharacterMode()
    {
        return mCharacterMode;
    }

    public void SetHover(bool hovering)
    {
        mHovering = hovering;
        if (hovering)
        {
            if (mCharacterMode != CharMode.Locked)
            {
                PlaySounds();
            }
        }
        else
        {
            //Destroy(mCurrentSoundObject);
            if (mSelectionState == SelectionState.NotSelected)
            {
                if (mHoverObject != null)
                {
                    Destroy(mHoverObject.gameObject);
                }
            }
        }
    }

    public void SetHover(bool hovering, bool mPlaySounds)
    {
        mHovering = hovering;
        if (hovering)
        {
            if (mCharacterMode != CharMode.Locked)
            {
                if (mPlaySounds)
                {
                    PlaySounds();
                }
            }
        }
        else
        {
            //Destroy(mCurrentSoundObject);
            if (mSelectionState == SelectionState.NotSelected)
            {

                if (mHoverObject != null)
                {
                    Destroy(mHoverObject.gameObject);
                }
            }
        }
    }

    public void DestroyHoverPartical()
    {
        if (mHoverObject != null)
        {
            Destroy(mHoverObject.gameObject);
        }
    }

    void PlaySounds()
    {
        StandaloneSound clip;

        int audioLength = mTalkingSounds.Length;

        if (audioLength > 0)
        {
            if (audioLength > 1)
            {
                int rnd = mLastSound;


                while (rnd == mLastSound)
                {
                    rnd = Random.Range(0, (audioLength - 1));
                }

                clip = mTalkingSounds[rnd];
            }
            else
            {
                clip = mTalkingSounds[0];
            }

            if (clip.mUseDefaultSettings)
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform);
            }
            else
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform, clip.mSettings);
            }
        }
    }

    void PlaySelectionSounds()
    {
        StandaloneSound clip;

        int audioLength = mSelectedSounds.Length;

        if (audioLength > 0)
        {
            if (audioLength > 1)
            {
                int rnd = mLastSound;


                while (rnd == mLastSound)
                {
                    rnd = Random.Range(0, (audioLength - 1));
                }

                clip = mSelectedSounds[rnd];
            }
            else
            {
                clip = mSelectedSounds[0];
            }

            if (clip.mUseDefaultSettings)
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform);
            }
            else
            {
                mCurrentSoundObject = AudioManager.sInstance.CreateAudioAtPosition(clip.mAudio, this.transform, clip.mSettings);
            }
        }
    }

    public void SetLocked(bool locked)
    {
        CharMode mode = locked ? CharMode.Locked : CharMode.None;

        mCharacterMode = mode;
    }

    public void SelectCharacter()
    {
        if (mCharacterMode != CharMode.Locked)
        {

            //based on selection state
            if (mSelectionState == SelectionState.NotSelected && mCharacterMode != CharMode.Used)
            {
                //first selection(give it a glow, change its image to selected)
                mSelectionState = SelectionState.FirstSelection;
                PlaySelectionSounds();
            }
            else if (mSelectionState == SelectionState.FirstSelection)
            {
                //second selection(give it another glow)
                if (mCharacterMode != CharMode.Attacked)
                {
                    mSelectionState = SelectionState.SecondSelection;
                    PlaySelectionSounds();
                }
            }
        }
    }
    public void SelectCharacter(bool playSound, bool onlyFirst)
    {
        if (mCharacterMode != CharMode.Locked)
        {

            //based on selection state
            if (mSelectionState == SelectionState.NotSelected && mCharacterMode != CharMode.Used)
            {
                //first selection(give it a glow, change its image to selected)
                mSelectionState = SelectionState.FirstSelection;
                if (playSound)
                {
                    PlaySelectionSounds();
                }
            }
            else if (mSelectionState == SelectionState.FirstSelection)
            {
                //second selection(give it another glow)
                if (mCharacterMode != CharMode.Attacked && !onlyFirst)
                {
                    mSelectionState = SelectionState.SecondSelection;
                    if (playSound)
                    {
                        PlaySelectionSounds();
                    }
                }
            }
        }
    }

    public SelectionState GetState()
    {
        return mSelectionState;
    }


    public void SetAttacking(bool attackMode)
    {
        //so when in attack mode its known to be one of the attackers
        mAttacking = attackMode;
    }



    public void ResetCharacter()
    {
        //set to idle state


        if (mCharacterMode == CharMode.TemporaryUsed)
        {
            mCharacterMode = mStoredMode;
        }

        //selection state is notselected
        mSelectionState = SelectionState.NotSelected;
        //mAttacking is false
        SetAttacking(false);

        //reset visuals
        HideAllGlows();
        SetAttackImage(false);
        if (mHoverObject != null && !mHovering)
        {
            Destroy(mHoverObject.gameObject);
        }

    }

    public void NewRoundReset()
    {
        //call resetcharacter()
        ResetCharacter();

        //if(!Locked) then set charmode to none
        if (mCharacterMode != CharMode.Locked)
        {
            mCharacterMode = CharMode.None;
        }
    }

    void HideAllGlows()
    {
        SetBaseGlow(false);
        SetSecondaryGlow(false);
    }
    void ShowAllGlows()
    {
        SetBaseGlow(true);
        SetSecondaryGlow(true);
    }

    void SetBaseGlow(bool show)
    {
        int value = show ? 100 : 0;

        mBaseGlow.color = new Color(mBaseGlow.color.r, mBaseGlow.color.g, mBaseGlow.color.b, value);
    }
    void SetSecondaryGlow(bool show)
    {
        int value = show ? 100 : 0;

        mSecondaryGlow.color = new Color(mSecondaryGlow.color.r, mSecondaryGlow.color.g, mSecondaryGlow.color.b, value);
    }


    void SetAttackImage(bool show)
    {
        int value = show ? 100 : 0;

        mAttackingImage.color = new Color(mAttackingImage.color.r, mAttackingImage.color.g, mAttackingImage.color.b, value);
    }

    void UpdatePortrait()
    {
        mBasePortrait.texture = mPortraits[(int)mImageState];
    }

    void SetImageState(ImageState state)
    {
        mImageState = state;
    }

    void Update()
    {
        if (mCharacterMode == CharMode.Locked)
        {
            //show locked(Set imageState)
            SetImageState(ImageState.Locked);
            //hide attack image
            SetAttackImage(false);
            //hide all glows
            HideAllGlows();
        }
        else if (mCharacterMode == CharMode.Used || mCharacterMode == CharMode.TemporaryUsed)
        {
            //show unavailable(Set imageState)
            SetImageState(ImageState.Unavailable);
            //hide attack image
            SetAttackImage(false);
            //hide all glows
            HideAllGlows();
        }
        else if (mCharacterMode == CharMode.Attacked && SelectionBar.sInstance.GetState() != UIState.None)
        {
            if (mSelectionState == SelectionState.FirstSelection)
            {
                SetImageState(ImageState.Selected);
                SetBaseGlow(true);
                SetSecondaryGlow(false);
            }
            else
            {
                //show unavailable(Set imageState)
                SetImageState(ImageState.Unavailable);
                //hide attack image
                SetAttackImage(false);
                //hide all glows
                HideAllGlows();
            }
        }
        else if (SelectionBar.sInstance.IsAttacking())
        {
            if (mAttacking)
            {
                Debug.Assert(mSelectionState != SelectionState.NotSelected, "[CharacterSelection] Cannot attack if not selected");

                //show attack image
                SetAttackImage(true);
                //show selected image(Set imageState)
                SetImageState(ImageState.Selected);

                switch (mSelectionState)
                {
                    case SelectionState.FirstSelection:
                        {
                            //show one glow
                            SetBaseGlow(true);
                            SetSecondaryGlow(false);
                        }
                        break;
                    case SelectionState.SecondSelection:
                        {
                            //show two glow
                            ShowAllGlows();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            //hide attack image
            SetAttackImage(false);
            switch (mSelectionState)
            {
                case SelectionState.NotSelected:
                    {

                        if (mHovering)
                        {
                            //show one glow
                            SetBaseGlow(true);
                            SetSecondaryGlow(false);
                            //show hover image(Set imageState)
                            SetImageState(ImageState.Hover);
                        }
                        else
                        {
                            //show nothing
                            HideAllGlows();
                            //show idle image(Set imageState)
                            SetImageState(ImageState.Idle);
                        }

                    }
                    break;
                case SelectionState.FirstSelection:
                    {

                        if (mHovering)
                        {
                            //show two glow
                            ShowAllGlows();
                            //show Selected image(Set imageState)
                            SetImageState(ImageState.Selected);
                        }
                        else
                        {
                            //show one glow
                            SetBaseGlow(true);
                            SetSecondaryGlow(false);
                            //show Selected image(Set imageState)
                            SetImageState(ImageState.Selected);
                        }

                    }
                    break;
                case SelectionState.SecondSelection:
                    {
                        //show two glows
                        ShowAllGlows();
                        //show Selected image(Set imageState)
                        SetImageState(ImageState.Selected);
                    }
                    break;
                default:
                    break;
            }

        }

        UpdatePortrait(); // making sure its the right image, changes based on if its hovering or not

    }
}

