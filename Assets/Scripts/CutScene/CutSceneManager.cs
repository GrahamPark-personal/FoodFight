using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager sInstance = null;

    public GameObject mWholeScreen;

    public GameObject mTopOfLeftBubbles;
    public GameObject[] mLeftBubbles;

    public GameObject mTopOfRightBubbles;
    public GameObject[] mRightBubbles;

    public Texture2D[] mCharacterPortraits;


    public RawImage mLeftCharacter;
    public RawImage mRightCharacter;

    public Text mLeftText;
    public Text mRightText;


    bool mActive = false;
    public CutScene mCurrentScene;
    public CutScene mSeconaryScene;
    [HideInInspector]
    public bool mLastPhrase = false;
    [HideInInspector]
    public int mCurrentPhrase = 0;

    [HideInInspector]
    public bool mReplaceLeft = false;

    [HideInInspector]
    public bool mReplaceRight = false;


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
        SetActive(true);
    }

    public void SetActive(bool active)
    {
        mActive = active;
        if (active)
        {
            InitScene();
        }
        else
        {
            mWholeScreen.SetActive(false);
        }
    }

    public void SetScene(CutScene scene)
    {
        mCurrentScene = scene;
    }

    void InitScene()
    {
        if (mCurrentScene)
        {
            mWholeScreen.SetActive(true);
            mTopOfLeftBubbles.SetActive(true);
            mTopOfRightBubbles.SetActive(true);
            SetPortraits();
            ResetBubbles();
            SetLeftBubble();
            SetRightBubble();
            mLeftText.text = "";
            mRightText.text = "";
            mLeftText.enabled = false;
            mRightText.enabled = false;
            mTopOfLeftBubbles.SetActive(false);
            mTopOfRightBubbles.SetActive(false);
            mCurrentPhrase = 0;
            SetScreenUp();

            mReplaceLeft = true;

            mReplaceRight = true;

        }
        else
        {
            Debug.Log("no scene selected");
        }
    }

    void SetScreenUp()
    {
        if (!mCurrentScene.mConvo[mCurrentPhrase].mBlank)
        {
            Phrase mPhrase = mCurrentScene.mConvo[mCurrentPhrase];
            if (mPhrase.mSide == CutSceneSide.Left)
            {
                mTopOfLeftBubbles.SetActive(true);
                mTopOfRightBubbles.SetActive(false);
                ResetBubbles();
                SetLeftBubble();
                SetLeftPortrait();
                //setLeftBubble
                //setLeftPortrait

                mLeftText.enabled = true;
                mRightText.enabled = false;

                mLeftText.text = mPhrase.mSentence;

            }
            else
            {
                mTopOfLeftBubbles.SetActive(false);
                mTopOfRightBubbles.SetActive(true);
                SetRightBubble();
                SetRightPortrait();
                //setRightBubble
                //setRightPortrait

                mLeftText.enabled = false;
                mRightText.enabled = true;

                mRightText.text = mPhrase.mSentence;

            }
            if(mPhrase.mRemoveOtherCharacter)
            {
                if(mPhrase.mSide == CutSceneSide.Left)
                {
                    mRightCharacter.texture = TexturesManager.sInstance.mBlankTexture;
                }
                else
                {
                    mLeftCharacter.texture = TexturesManager.sInstance.mBlankTexture;
                }
            }
            if(mPhrase.mPhraseAudio.Length > 0)
            {
                StartCoroutine(PlaySounds(mPhrase));
            }
        }
        else
        {
            mTopOfLeftBubbles.SetActive(false);
            mLeftText.enabled = false;

            mTopOfRightBubbles.SetActive(false);
            mRightText.enabled = false;
        }
    }

    IEnumerator PlaySounds(Phrase phrase)
    {
        foreach (AudioClip clip in phrase.mPhraseAudio)
        {
            AudioManager.sInstance.CreateAudioAtPosition(clip, transform);
            yield return new WaitForSeconds(clip.length);
        }
    }

    void SetPortraits()
    {
        mLeftCharacter.texture = mCharacterPortraits[(int)mCurrentScene.mLeftCharacter];
        mRightCharacter.texture = mCharacterPortraits[(int)mCurrentScene.mRightCharacter];
    }

    void SetLeftPortrait()
    {
        Texture newText = mCharacterPortraits[(int)mCurrentScene.mConvo[mCurrentPhrase].mCharacter];

        if (newText != mLeftCharacter.texture)
        {
            mReplaceLeft = true;
            ResetBubbles();
            SetLeftBubble();
        }

        mLeftCharacter.texture = newText;
    }

    void SetRightPortrait()
    {
        Texture newText = mCharacterPortraits[(int)mCurrentScene.mConvo[mCurrentPhrase].mCharacter];

        if (newText != mRightCharacter.texture)
        {
            mReplaceRight = true;
            ResetBubbles();
            SetRightBubble();
        }

        mRightCharacter.texture = newText;

    }

    void SetSceneWithCurrentCharacter()
    {

    }

    void ResetBubbles()
    {
        foreach (GameObject item in mLeftBubbles)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in mRightBubbles)
        {
            item.SetActive(false);
        }
    }

    void SetLeftBubble()
    {
        mLeftBubbles[(int)mCurrentScene.mConvo[mCurrentPhrase].mCharacter].SetActive(true);
    }

    void SetRightBubble()
    {
        mRightBubbles[(int)mCurrentScene.mConvo[mCurrentPhrase].mCharacter].SetActive(true);
    }

    void Update()
    {
        if (mActive)
        {
            //do stuff
            if (Input.GetMouseButtonDown(0))
            {
                mCurrentPhrase++;
                if (mCurrentPhrase >= mCurrentScene.mConvo.Length)
                {
                    //end
                    SetActive(false);
                }
                else
                {
                    SetScreenUp();
                }
            }
        }
        else
        {
            if (mLastPhrase)
            {
                GameManager.sInstance.mFinishedLastCutScene = true;
            }
            mWholeScreen.SetActive(false);
        }
    }
}
