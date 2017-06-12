using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GamePartical
{
    public string mKey;
    public GameObject mPartical;

    [HideInInspector]
    public ParticleControl mControl;
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

    [Header("Character Particals")]
    public GameObject[] mCharacterParticals;

    [Space(10)]
    [Header("Effects")]
    public GameObject mStunnedObject;
    public GameObject mSlowedObject;
    public GameObject mStormCloak;

    [Space(10)]
    [Header("Attack Particals")]
    public GamePartical[] mParticals;

    public PlayerPartical[] mPlayerParticals;


    Dictionary<string, GamePartical> mParticalContainer = new Dictionary<string, GamePartical>();


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


    void Start()
    {
        for (int i = 0; i < mParticals.Length; i++)
        {
            mParticalContainer[mParticals[i].mKey] = mParticals[i];
            mParticals[i].mControl = mParticals[i].mPartical.gameObject.GetComponent<ParticleControl>();
        }
    }


    public void SpawnPartical(string key, Transform mStart, Transform mEnd)
    {
        if (!mParticalContainer.ContainsKey(key))
        {
            Debug.Log("[ParticalManager] Couldn't find key: " + key);
            return;
        }
        
        StartCoroutine(SpawnParticalAfterTime(key, mStart, mEnd));

    }

    IEnumerator SpawnParticalAfterTime(string key, Transform mStart, Transform mEnd)
    {
        GamePartical gamePart = mParticalContainer[key];

        yield return new WaitForSeconds(1.6f);


        GameObject obj;

        obj = Instantiate(gamePart.mPartical, mEnd.position + new Vector3(0, 1, 0), gamePart.mPartical.gameObject.transform.rotation);
        ParticleControl pCon = obj.GetComponent<ParticleControl>();


        if (pCon.mAction == mParticleAction.Stationary_DestroyAfterTime || pCon.mAction == mParticleAction.Stationary_DestroyOnCall)
        {
            obj.transform.position = mEnd.position + new Vector3(0, 1, 0);
            pCon.Init(mEnd);
        }
        else if(pCon.mAction == mParticleAction.StartAtEnemyMoveTowardsCharacter)
        {
            obj.transform.position = mEnd.position + new Vector3(0, 1, 0);
            pCon.Init(mStart);
        }
        else if(pCon.mAction == mParticleAction.StartOnCharacter)
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


}
