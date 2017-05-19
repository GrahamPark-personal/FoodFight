using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum mParticleAction
{
    Destination_DestroyAftertime,
    Destination_DestroyOnEnter,
    Stationary_DestroyAfterTime,
    Stationary_DestroyOnCall
}


public class ParticleControl : MonoBehaviour
{
    [Header("Base Information")]
    public mParticleAction mAction;
    public float mSpeed;

    [Space(10)]
    [Header("Only needed if its not stationary")]
    public Transform mFinalPositon;
    
    [Space(10)]
    [Header("Extra Functions")]
    public float mLifetime;
    public bool mPlaceOtherParticleOnArrival;
    public GameObject mSecondaryParticle;


    float mCurrentTime;
    bool initalized = false;


    public void Init(Transform point)
    {
        mFinalPositon = point;
        mCurrentTime = Time.time + mLifetime;
        initalized = true;
    }

    void Update()
    {
        if (initalized)
        {
            if (mAction == mParticleAction.Stationary_DestroyAfterTime)
            {
                if (Time.time >= mCurrentTime)
                {
                    FinishParticle();
                }
            }
            else if(mAction != mParticleAction.Stationary_DestroyOnCall)
            {
                Vector3.Lerp(transform.position, mFinalPositon.position, mSpeed * Time.deltaTime);
                if (mAction == mParticleAction.Destination_DestroyOnEnter)
                {
                    if (Vector3.Distance(transform.position, mFinalPositon.position) <= 1.0f)
                    {
                        FinishParticle();
                    }
                }
                else if (mAction == mParticleAction.Destination_DestroyAftertime)
                {
                    if (Time.time >= mCurrentTime)
                    {
                        FinishParticle();
                    }
                }
            }
        }
    }


    void FinishParticle()
    {
        if (mPlaceOtherParticleOnArrival)
        {
            Instantiate(mSecondaryParticle, transform.position, transform.rotation);
        }
        Destroy(this.gameObject);
    }

    public void DestroyParticle()
    {
        FinishParticle();
    }

}
