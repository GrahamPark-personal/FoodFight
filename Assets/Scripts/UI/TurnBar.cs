using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{

    public Texture2D YourturnImage;
    public Texture2D EnemyTurnImage;
    RawImage mImage;
    float mCurrentAlpha;
    float mTargetAlpha;
    float mSpeed = 5;

    void Start()
    {
        mImage = this.GetComponent<RawImage>();
    }

    void Update()
    {
        if (!GameManager.sInstance.mFinishedLastCutScene)
        {
            mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, Time.deltaTime * mSpeed);
            mImage.color = new Color(mImage.color.r, mImage.color.g, mImage.color.b, mCurrentAlpha);
        }
        else
        {
            mImage.enabled = false;
        }
    }

    public void ShowBar(bool enemyTurn)
    {
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            mImage.texture = EnemyTurnImage;
            mTargetAlpha = 1.0f;

            StartCoroutine(ReShowAfter());
        }
        else
        {
            StartCoroutine(WaitToShow(enemyTurn));
        }
    }

    IEnumerator WaitToShow(bool enemyTurn)
    {

        yield return new WaitForSeconds(0.0f);

        if (enemyTurn)
        {
            mImage.texture = EnemyTurnImage;
        }
        else
        {
            mImage.texture = YourturnImage;
        }



        mTargetAlpha = 1.0f;

        StartCoroutine(ReShowAfter());
    }

    IEnumerator ReShowAfter()
    {
        yield return new WaitForSeconds(2.0f);
        mTargetAlpha = 0.0f;
    }

}
