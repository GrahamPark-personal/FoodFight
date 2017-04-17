using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{

    public static AnimationControl sInstance = null;

    Dictionary<string, Character> characterDictionary;
    Dictionary<string, int> animationDictionary;

    Character[] mChars = new Character[6];

    // Use this for initialization
    void Start()
    {
        if(sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            Destroy(this);
        }

        for (int i = 0; i < mChars.Length; i++)
        {
            mChars[i] = GameManager.sInstance.mUIManager.mCharacters[i];
        }

        characterDictionary.Add("YellowMage", mChars[0]);
        characterDictionary.Add("BlueMage", mChars[1]);
        characterDictionary.Add("BrownMage", mChars[2]);
        characterDictionary.Add("RedMage", mChars[3]);
        characterDictionary.Add("GreenMage", mChars[4]);
        characterDictionary.Add("BlackMage", mChars[5]);
        //add more characters here

        animationDictionary.Add("Hit", 0);
        animationDictionary.Add("Attack1", 1);
        animationDictionary.Add("Attack2", 2);
        animationDictionary.Add("Idle", 3);
        animationDictionary.Add("Reactivate", 4);
        animationDictionary.Add("Deactivated", 5);
        animationDictionary.Add("Deactivate", 6);

        //Add any additional animations here.  
        //Note:  Mages may have different Animations for the same attack.  For example:
        // When Using Banana Split the Blue mage may use a different animation than the Red Mage
        // just keep this in mind when thinking about what animations to add. 
        // For this reason Naming convention may change as well. (ie. "BlueBananaSplit)

    }

    // Update is called once per frame
    void Update()
    {

    }


    void SetAnimationState(string charName, string animationState)
    {
        Character tempChar = characterDictionary[charName];

        int tempState = animationDictionary[animationState];
        //the following line is where you set the animator state parameter. However you want to set it up (this is just a framework)
        tempChar.mAnimator.SetInteger("Blend", tempState);

        //Here is a couple examples of how you might use this system:

        //Example 1:
        // mCharacters[0].AnimationHandler.SetAnimationState("BlueMage", BlueRed");

        //Example 2:
        // mCharacters[0].AnimationHandler.SetAnimationState("BlackMage, "Idle");

    }

}
