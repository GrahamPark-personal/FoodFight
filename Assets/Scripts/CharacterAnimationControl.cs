using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharAnimState
{
    Idle = 0,
    Attack = 1,
    PoweredDown = 2,
    Moving = 3,
}


public class CharacterAnimationControl : MonoBehaviour {

    Animator mAnim;

    public CharAnimState mState = CharAnimState.Idle;

    public void ChangeState(CharAnimState newState)
    {
        mState = newState;
        mAnim.SetInteger("State", (int)mState);
    }

	void Start ()
    {
        mAnim = GetComponent<Animator>();
    }
	
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeState(CharAnimState.PoweredDown);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeState(CharAnimState.Attack);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeState(CharAnimState.Idle);
        }

        if(mState == CharAnimState.Attack)
        {
            StartCoroutine(ResetAnim());
        }
    }

    IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(0.1f);
        mState = CharAnimState.Idle;
    }
}
