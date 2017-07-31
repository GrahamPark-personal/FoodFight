using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AblilityBarSoundControl : MonoBehaviour
{

    public void LowerSound(bool low)
    {
        AudioManager.sInstance.LowerTheMusic(low);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
