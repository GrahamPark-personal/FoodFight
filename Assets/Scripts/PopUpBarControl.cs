using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpBarControl : MonoBehaviour
{
    public GameObject mPopUpBar;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Bar shown: " + GameManager.sInstance.mUIManager.mEnemyPopUpBarShown);
        }

        bool shown = GameManager.sInstance.mUIManager.mEnemyPopUpBarShown;
        mPopUpBar.SetActive(shown);
    }
}
