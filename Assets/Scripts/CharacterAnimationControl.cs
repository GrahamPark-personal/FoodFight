using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharAnimState
{
    Idle = 0,
    BasicAttack = 1,
    Attack1 = 2,
    Attack2 = 3,
    Attack3 = 4
    
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
            //ChangeState(CharAnimState.PoweredDown);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            //ChangeState(CharAnimState.Attack);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeState(CharAnimState.Idle);
        }

        //if(mState == CharAnimState.Attack)
        //{
        //    StartCoroutine(ResetAnim());
        //}
    }

    IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(0.1f);
        mState = CharAnimState.Idle;
    }
}
