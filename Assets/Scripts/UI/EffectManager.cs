using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    GameObject mParent;
    Dictionary<AilmentID, GameObject> AilmentPositions = new Dictionary<AilmentID, GameObject>();
    Character mCharacter;

    void Start()
    {
        mParent = gameObject;
        mCharacter = GetComponentInParent<Character>();
    }

    void Update()
    {

        if(mCharacter.ContainsAilment(AilmentID.Stun))
        {
            if(!AilmentPositions.ContainsKey(AilmentID.Stun))
            {
                //add image
                GameObject newEffect = Instantiate(GameManager.sInstance.mStunnedObject, mParent.transform);
                newEffect.transform.position = new Vector3(0, 0, 0);
                newEffect.transform.localPosition = new Vector3(0, 0, 0);
                AilmentPositions.Add(AilmentID.Stun, newEffect);
            }
        }
        else if(AilmentPositions.ContainsKey(AilmentID.Stun))
        {
            //remove image
            GameObject Getobj = AilmentPositions[AilmentID.Stun];
            if(Getobj != null)
            {
                AilmentPositions.Remove(AilmentID.Stun);
                Destroy(Getobj.gameObject);
                Getobj = null;
            }
        }

        if (mCharacter.ContainsAilment(AilmentID.Slow))
        {
            if (!AilmentPositions.ContainsKey(AilmentID.Slow))
            {
                Debug.Log("Doesnt have stun Slow");
                //add image
                GameObject newEffect = Instantiate(GameManager.sInstance.mSlowedObject, mParent.transform);
                newEffect.transform.position = new Vector3(0, -1.103f, 0);
                newEffect.transform.localPosition = new Vector3(0, -1.103f, 0);
                AilmentPositions.Add(AilmentID.Slow, newEffect);
            }
        }
        else if (AilmentPositions.ContainsKey(AilmentID.Slow))
        {
            //remove image
            GameObject Getobj = AilmentPositions[AilmentID.Slow];
            if (Getobj != null)
            {
                AilmentPositions.Remove(AilmentID.Slow);
                Destroy(Getobj.gameObject);
                Getobj = null;
            }
        }

    }
}
