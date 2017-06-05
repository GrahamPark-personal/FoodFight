using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum side
{
    Left,
    Right
}


public class MovePortrait : MonoBehaviour
{

    public side mSide;
    public float mOffsetStart;
    float mSpeed = 10.0f;
    RectTransform mRect;
    Vector3 mStartPos;

    void Start()
    {
        mRect = GetComponent<RectTransform>();
        mStartPos = mRect.position;
    }

    void Update()
    {
        if (mSide == side.Left)
        {
            if (CutSceneManager.sInstance.mReplaceLeft)
            {
                RunMotion();
            }
        }
        else if (mSide == side.Right)
        {
            if (CutSceneManager.sInstance.mReplaceRight)
            {
                RunMotion();
            }
        }
    }

    void RunMotion()
    {
        mRect.position = new Vector3(mRect.position.x + mOffsetStart, mRect.position.y, mRect.position.z);

        StartCoroutine(moveBack());
    }

    IEnumerator moveBack()
    {
        Debug.Log("[MovePortrait] MoveBackCalled");
        while (Vector3.Distance(mRect.position, mStartPos) > 1.0f)
        {
            yield return new WaitForSeconds(0.001f);
            mRect.position = Vector3.Lerp(mRect.position, mStartPos, mSpeed * Time.deltaTime);

        }

        mRect.position = mStartPos;

        switch (mSide)
        {
            case side.Left:
                CutSceneManager.sInstance.mReplaceLeft = false;
                break;
            case side.Right:
                CutSceneManager.sInstance.mReplaceRight = false;
                break;
            default:
                break;
        }

    }

}

