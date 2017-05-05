using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAnimation : MonoBehaviour
{

    Animator mAnimator;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.sInstance.mFinishedLastCutScene)
        {
            Debug.Log("Animation should play");
            mAnimator.SetBool("WonGame",true);
        }
    }
}
