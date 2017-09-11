using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialToggle : MonoBehaviour
{

    public static TutorialToggle sInstance = null;

    RawImage mImage;



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

    void Start()
    {
        mImage = GetComponent<RawImage>();
        mImage.enabled = false;
    }

    public void Toggle()
    {
        bool oldActive = mImage.IsActive();
        mImage.enabled = !oldActive;
    }

}
