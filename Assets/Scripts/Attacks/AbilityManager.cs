using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharAbility
{
    public Attack mAbility;
    public string mPartical;
}

[System.Serializable]
public struct CharactersAbilities
{
    public CharAbility[] mAbilities;
}



public class AbilityManager : MonoBehaviour {

    public static AbilityManager sInstance = null;

    public CharactersAbilities[] mCharacterAbilities;

    void Awake()
    {
        if(sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Attack GetAttack(int character)
    {
        return mCharacterAbilities[character].mAbilities[character].mAbility;
    }

    public Attack GetAttack(int character1, int character2)
    {
        return mCharacterAbilities[character1].mAbilities[character2].mAbility;
    }

    public string GetPartical(int character1,int character2)
    {
        return mCharacterAbilities[character1].mAbilities[character2].mPartical;
    }

    public string GetPartical(int character)
    {
        return mCharacterAbilities[character].mAbilities[character].mPartical;
    }

}
