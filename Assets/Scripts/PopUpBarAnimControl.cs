using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpBarAnimControl : MonoBehaviour
{

    Animator anim;

    bool mShowBar = false;

	
	void Start ()
    {
        anim = GetComponent<Animator>();		
	}
	
	
	void Update ()
    {
		if(GameManager.sInstance.mUIManager.mEnemyPopUpBarShown != mShowBar)
        {
            mShowBar = !mShowBar;
            

        }
	}
}
