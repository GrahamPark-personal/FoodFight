using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndButtonControl : MonoBehaviour
{
    private RawImage mButtonImage;

    void Start()
    {
        mButtonImage = GetComponent<RawImage>();
        mButtonImage.enabled = false;
    }

    void Update()
    {
        if(GameManager.sInstance.GameLost)
        {
            mButtonImage.enabled = true;
        }
    }
}
