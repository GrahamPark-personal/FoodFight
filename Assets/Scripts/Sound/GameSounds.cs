using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MainSounds
{
    public string ID;
    public AudioClip mAudio;
}



public class GameSounds : MonoBehaviour
{
    public static GameSounds sInstance = null;

    public MainSounds[] mSounds;

    //public AudioClip mButtonSelected;
    //public AudioClip mButtonSelection;

    //public AudioClip EndTurnButton;

    //public AudioClip RotatingMap1;
    //public AudioClip RotatingMap2;

    //public AudioClip mBlueStar;
    //public AudioClip mRedStar;

    //public AudioClip mEnemyTurn;
    //public AudioClip mYourTurn;

    //public AudioClip mEnemyZoneCaptured;
    //public AudioClip mPlayerZoneCaptured;
    //public AudioClip mZoneContested;

    //public AudioClip mEnemyHit1;
    //public AudioClip mEnemyHit2;

    //public AudioClip mPlayerHit1;
    //public AudioClip mPlayerHit2;

    //public AudioClip mStar1;
    //public AudioClip mStar2;
    //public AudioClip mStar3;





    public void PlayAudio(string audioID)
    {

        foreach (MainSounds item in mSounds)
        {
            if(item.ID.Equals(audioID))
            {
                AudioManager.sInstance.CreateAudioAtPosition(item.mAudio, this.transform);
                return;
            }
        }

        Debug.Log("Couldn't Find Audio: " + audioID);

    }


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

    }

    void Update()
    {

    }
}
