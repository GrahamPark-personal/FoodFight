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

        characterDictionary.Add("YellowMage", mChars[0]);
        characterDictionary.Add("BlueMage", mChars[1]);
        characterDictionary.Add("BrownMage", mChars[2]);
        characterDictionary.Add("RedMage", mChars[3]);
        characterDictionary.Add("GreenMage", mChars[4]);
        characterDictionary.Add("BlackMage", mChars[5]);
        //add more characters here

        animationDictionary.Add("Idle", 0);
        animationDictionary.Add("Move", 1);
        animationDictionary.Add("YellowSingle", 2);
        animationDictionary.Add("BlueSingle", 3);
        animationDictionary.Add("BrownSingle", 4);
        animationDictionary.Add("RedSingle", 5);
        animationDictionary.Add("GreenSingle", 6);
        animationDictionary.Add("BlackSingle", 7);

        animationDictionary.Add("YellowBlue", 8);
        animationDictionary.Add("YellowBrown", 10);
        animationDictionary.Add("YellowRed", 11);
        animationDictionary.Add("YellowGreen", 12);
        animationDictionary.Add("YellowBlack", 13);

        animationDictionary.Add("BlueBrown", 14);
        animationDictionary.Add("BlueRed", 15);
        animationDictionary.Add("BlueGreen", 16);
        animationDictionary.Add("BlueBlack", 17);

        animationDictionary.Add("RedBrown", 18);
        animationDictionary.Add("RedGreen", 19);
        animationDictionary.Add("RedBlack", 20);

        animationDictionary.Add("BrownGreen", 21);
        animationDictionary.Add("BrownBlack", 22);

        animationDictionary.Add("GreenBlack", 23);

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
        tempChar.mAnimator.SetInteger("AnimState", tempState);

        //Here is a couple examples of how you might use this system:

        //Example 1:
        // mCharacters[0].AnimationHandler.SetAnimationState("BlueMage", BlueRed");

        //Example 2:
        // mCharacters[0].AnimationHandler.SetAnimationState("BlackMage, "Idle");

    }

}
