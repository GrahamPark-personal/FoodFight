using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager sInstance = null;
    public GameObject mAudioObj;
    AudioSource mMusic;
    float mCurrentVolume;
    float mMaxVolume;
    float mLowerVolume = 0.2f;

    void Awake()
    {
        if(!sInstance)
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
        mMusic = GetComponent<AudioSource>();
        mMaxVolume = mMusic.volume;
        mCurrentVolume = mMaxVolume;
    }

    public void LowerMusicFor(float time)
    {
        mCurrentVolume = mLowerVolume;
        StartCoroutine(WaitForSound(time));
    }

    IEnumerator WaitForSound(float time)
    {
        yield return new WaitForSeconds(time);
        mCurrentVolume = mMaxVolume;
    }

    void Update()
    {
        mMusic.volume = Mathf.Lerp(mMusic.volume, mCurrentVolume, Time.deltaTime * 10);
    }
    
    public GameObject CreateAudioAtPosition(AudioClip clip, Transform tr)
    {
        GameObject tempObj =  new GameObject();
        if (clip != null)
        {
            tempObj = Instantiate(mAudioObj, tr.position, tr.rotation);
            //Debug.Log("Got here 1");
            AudioObject audObj = tempObj.GetComponent<AudioObject>();
            //Debug.Log("Got here 2");
            audObj.SetAudio(clip);
            //Debug.Log("Got here 3");
            audObj.StartAudio();
        }

        return tempObj;
    }
}
