using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndButtonControl : MonoBehaviour
{
    private Image mButtonImage;

    void Start()
    {
        mButtonImage = GetComponent<Image>();
        Debug.Assert(mButtonImage, "[EndButtonControl]");
        mButtonImage.enabled = false;
    }

    void Update()
    {
        if (GameManager.sInstance.GameLost)
        {
            mButtonImage.enabled = true;
        }
    }
}
