using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioSetting
{
    [Range(0, 256)]
    public int mPriority;
    [Range(0.0f, 1.0f)]
    public float mVolume;
    [Range(-3.0f, 3.0f)]
    public float mPitch;
    [Range(0.0f,1.1f)]
    public float mReverb;

}


public class AudioManager : MonoBehaviour
{
    public AudioSetting mdefaultSetting;
    public static AudioManager sInstance = null;
    public GameObject mAudioObj;

    [HideInInspector]
    public AudioSource mMusic;
    float mCurrentVolume;
    float mMaxVolume;
    float mLowerVolume = 0.3f;

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

    void SetAudioWithSettings(ref AudioSource clip, AudioSetting settings)
    {
        clip.priority = settings.mPriority;
        clip.volume = settings.mVolume;
        clip.pitch = settings.mPitch;
        clip.reverbZoneMix = settings.mReverb;
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

    public GameObject CreateAudioAtPosition(AudioClip clip, Transform tr, AudioSetting settings)
    {
        GameObject tempObj = new GameObject();
        if (clip != null)
        {
            tempObj = Instantiate(mAudioObj, tr.position, tr.rotation);
            //Debug.Log("Got here 1");
            AudioSource clipSource = tempObj.GetComponent<AudioSource>();
            SetAudioWithSettings(ref clipSource, settings);

            AudioObject audObj = tempObj.GetComponent<AudioObject>();
            //Debug.Log("Got here 2");
            audObj.SetAudio(clip);


            Debug.Log("About to play");

            //Debug.Log("Got here 3");
            audObj.StartAudio();
        }

        return tempObj;
    }

}
