using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConquereComponents : MonoBehaviour
{

    //star images
    //currentStars on both sides

    public RawImage[] mRedStars;
    public RawImage[] mBlueStars;

    public Texture mRedEmpty;
    public Texture mRedFilled;

    public Texture mBlueEmpty;
    public Texture mBlueFilled;
    


    void Update()
    {
        int charTurnCount = ConquereController.sInstance.mCharacterTurnCounter;
        int enemyTurnCount = ConquereController.sInstance.mEnemyTurnCounter;

        for (int i = 0; i < mRedStars.Length; i++)
        {
            if (i < enemyTurnCount)
            {
                //set current star to empty
                mRedStars[i].texture = mRedEmpty;
            }
            else
            {
                //set current star to filled
                mRedStars[i].texture = mRedFilled;
            }
        }

        for (int i = 0; i < mBlueStars.Length; i++)
        {
            if (i < charTurnCount)
            {
                //set current star to empty
                mBlueStars[i].texture = mBlueEmpty;
            }
            else
            {
                //set current star to filled
                mBlueStars[i].texture = mBlueFilled;
            }
        }

    }
}
