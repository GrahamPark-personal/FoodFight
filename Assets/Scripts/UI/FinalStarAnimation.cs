using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalStarAnimation : MonoBehaviour
{
    Animator mAnim;

    public GameObject mStarObj;

    public RawImage mBronzeStar;
    public RawImage mSilverStar;
    public RawImage mGoldStar;

    void Start()
    {
        mAnim = GetComponent<Animator>();
        mStarObj.SetActive(false);
    }

    void Update()
    {
        if (GameManager.sInstance.mFinishedLastCutScene)
        {
            //make them active
            mAnim.SetBool("Shown", true);
            //play animation where the go from small to large
            //set the hue to 100 for each color on the ones they got
            StartCoroutine(WaitAndShowStars());
            //ex. if they ony got bronze light that one up
            //ex. if they got gold, have bronze light up, wait a second, light silver up, then light gold up
        }
    }

    IEnumerator WaitAndShowStars()
    {




        yield return new WaitForSeconds(2f);

        switch (GameManager.sInstance.mUIManager.mCurrentStar)
        {
            case StarLevel.Gold:
                GameSounds.sInstance.PlayAudio("STAR_THREE");
                break;
            case StarLevel.Silver:
                GameSounds.sInstance.PlayAudio("STAR_TWO");
                break;
            case StarLevel.Bronze:
                GameSounds.sInstance.PlayAudio("STAR_ONE");
                break;
            default:
                break;
        }

        mStarObj.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        //maybe interpolate
        mBronzeStar.color = new Color(100, 100, 100);
        StarLevel mCurrentLevel = GameManager.sInstance.mUIManager.mCurrentStar;
        if (mCurrentLevel == StarLevel.Silver || mCurrentLevel == StarLevel.Gold)
        {
            yield return new WaitForSeconds(0.7f);
            mSilverStar.color = new Color(100, 100, 100);
        }
        if (mCurrentLevel == StarLevel.Gold)
        {
            yield return new WaitForSeconds(0.7f);
            mGoldStar.color = new Color(100, 100, 100);
        }
    }
}
