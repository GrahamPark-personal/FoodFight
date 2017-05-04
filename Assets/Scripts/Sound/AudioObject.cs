using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{

    //[HideInInspector]
    //public float mTimer;

    float minSoundRadius = 8;

    [HideInInspector]
    bool mTimerStarted = false;

    AudioSource mAudio;

    void Start()
    {
        mAudio = GetComponent<AudioSource>();
    }

    public void SetAudio(AudioClip clip)
    {
        mAudio = GetComponent<AudioSource>();
        Debug.Log("Set 1");
        mAudio.playOnAwake = false;
        Debug.Log("Set 2");
        mAudio.minDistance = minSoundRadius;
        Debug.Log("Set 3");
        mAudio.clip = clip;
    }

    public void StartAudio()
    {
        mAudio.Play();
        AudioManager.sInstance.LowerMusicFor(mAudio.clip.length);
        mTimerStarted = true;
    }

    void Update()
    {   
        if(mTimerStarted)
        {
            if(!mAudio.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
