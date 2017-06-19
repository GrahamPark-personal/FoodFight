using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GamePartical
{
    public string mKey;
    [Space(10)]
    public GameObject mPartical;
    public string mSoundID;
    public bool mDelayed;
    [Space(10)]
    public AudioClip[] mAttackSounds;
    [HideInInspector]
    public ParticleControl mControl;
}

[System.Serializable]
public struct PlayerSpecificSounds
{

    public string mName;
    public AudioClip[] mDamageSounds;

    [Header("Only use for minions, or npc's")]
    public AudioClip[] mAttackSounds;

}


[System.Serializable]
public struct PlayerPartical
{
    public string mCharacter;
    public string mBasicPartical;
    public string[] mDuoPartical;
}


public class ParticleManager : MonoBehaviour
{

    static public ParticleManager sInstance = null;

    [Header("Cursors")]
    public Texture2D mHover;
    public Texture2D mClick;

    [Header("Player Sounds(NOT implemented yet)")]
    public PlayerSpecificSounds[] mPlayerSounds;

    [Header("Minion Sounds(NOT implemented yet)")]
    public PlayerSpecificSounds[] mMinions;

    [Header("Character Particals")]
    public GameObject[] mCharacterParticals;

    [Space(10)]
    [Header("Extra Stuff")]
    public GameObject mExclaim;
    public GamePartical mDeathPartical;

    [Space(10)]
    [Header("Effects")]
    public GameObject mStunnedObject;
    public GameObject mSlowedObject;
    public GameObject mStormCloak;

    [Space(10)]
    [Header("Partical Blocks")]
    public GameObject ElectricHailStorm;
    public GameObject ElectricAvenue;

    [Space(10)]
    [Header("Talking Sounds")]
    public AudioClip[] mYellow;
    public AudioClip[] mBlue;
    public AudioClip[] mBrown;
    public AudioClip[] mRed;
    public AudioClip[] mGreen;
    public AudioClip[] mBlack;
    public AudioClip[] mWhite;

    [Space(10)]
    [Header("Attack Particals")]
    public GamePartical[] mParticals;

    //public PlayerPartical[] mPlayerParticals;

    Dictionary<string, GamePartical> mParticalContainer;


    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            Debug.Assert(false, "[ParticalManager]Cannot have two particle managers");
            Destroy(this);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(mClick, new Vector2(100, 84), CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(mHover, new Vector2(100, 84), CursorMode.Auto);
        }
    }

    void Start()
    {

        mParticalContainer = new Dictionary<string, GamePartical>(mParticals.Length);

        Cursor.SetCursor(mHover, new Vector2(0, 0), CursorMode.Auto);

        for (int i = 0; i < mParticals.Length; i++)
        {
            Debug.Log("times through");
            if (mParticals[i].mPartical != null)
            {
                mParticals[i].mControl = mParticals[i].mPartical.gameObject.GetComponent<ParticleControl>();
            }
            mParticalContainer.Add(mParticals[i].mKey, mParticals[i]);
        }
        mParticalContainer.Add("DEATH", mDeathPartical);

    }



    public void SpawnPartical(string key, Transform mStart, Transform mEnd, bool playSound)
    {
        if (!mParticalContainer.ContainsKey(key))
        {
            Debug.Log("[ParticalManager] Couldn't find key: " + key);
            return;
        }
        StartCoroutine(SpawnParticalAfterTime(key, mStart, mEnd, true));

    }

    IEnumerator SpawnParticalAfterTime(string key, Transform mStart, Transform mEnd, bool playSound)
    {
        GamePartical gamePart = mParticalContainer[key];
        Debug.Log("Partical: " + key);
        if (gamePart.mDelayed)
        {
            yield return new WaitForSeconds(1.6f);
        }
        else
        {
            yield return new WaitForSeconds(0.0f);
        }
        GameSounds.sInstance.PlayAudio(gamePart.mSoundID);


        GameObject obj;

        if (gamePart.mPartical != null)
        {
            obj = Instantiate(gamePart.mPartical, mEnd.position + new Vector3(0, 1, 0), gamePart.mPartical.gameObject.transform.rotation);
            ParticleControl pCon = obj.GetComponent<ParticleControl>();


            if (pCon.mAction == mParticleAction.Stationary_DestroyAfterTime || pCon.mAction == mParticleAction.Stationary_DestroyOnCall)
            {
                obj.transform.position = mEnd.position + new Vector3(0, 1, 0);
                pCon.Init(mEnd);
            }
            else if (pCon.mAction == mParticleAction.StartAtEnemyMoveTowardsCharacter)
            {
                obj.transform.position = mEnd.position + new Vector3(0, 1, 0);
                pCon.Init(mStart);
            }
            else if (pCon.mAction == mParticleAction.StartOnCharacter)
            {
                obj.transform.position = mStart.position + new Vector3(0, 1, 0);
                pCon.Init(mStart);
            }
            else
            {
                obj.transform.position = mStart.position + new Vector3(0, 1, 0);
                pCon.Init(mEnd);
            }
        }

        AudioClip clip;

        int audioLength = gamePart.mAttackSounds.Length;

        if (playSound && audioLength > 0)
        {
            if (audioLength > 1)
            {
                int rnd = Random.Range(0, (audioLength - 1));
                clip = gamePart.mAttackSounds[rnd];

            }
            else
            {
                clip = gamePart.mAttackSounds[0];
            }

            AudioManager.sInstance.CreateAudioAtPosition(clip, this.transform);
        }

    }


}
