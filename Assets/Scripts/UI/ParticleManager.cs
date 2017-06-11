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


public class ParticleManager : MonoBehaviour
{

    static public ParticleManager sInstance = null;

    public GamePartical[] mParticals;

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
            mParticals[i].mControl = mParticals[i].mPartical.GetComponent<ParticleControl>();
        }
    }


    public GameObject SpawnPartical(string key, Transform mStart, Transform mEnd)
    {
        if (!mParticalContainer.ContainsKey(key))
        {
            Debug.Log("[ParticalManager] Couldn't find key: " + key);
            return null;
        }

        Debug.Log("Got into partical creation");

        GamePartical gamePart = mParticalContainer[key];

        GameObject obj;

        obj = Instantiate(gamePart.mPartical, mEnd.position + new Vector3(0, 1, 0), gamePart.mPartical.gameObject.transform.rotation);
        ParticleControl pCon = obj.GetComponent<ParticleControl>();

        Debug.Log("Action: " + pCon.mAction);

        if (pCon.mAction == mParticleAction.Stationary_DestroyAfterTime || pCon.mAction == mParticleAction.Stationary_DestroyOnCall)
        {
            obj.transform.position = mEnd.position + new Vector3(0, 1, 0);
            pCon.Init(mEnd);
        }
        else
        {
            Debug.Log("Else");
            obj.transform.position = mStart.position + new Vector3(0, 1, 0);
            pCon.Init(mEnd);
        }

        return obj;
    }


}
