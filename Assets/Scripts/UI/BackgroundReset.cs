using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundReset : MonoBehaviour
{
    void OnMouseEnter()
    {
        GameManager.sInstance.mHoverBlock.SetActive(false);
    }
}
