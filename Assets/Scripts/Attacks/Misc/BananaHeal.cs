using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaHeal : MonoBehaviour {

    public int mHealAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Character tempChar = col.GetComponent<Character>();
            tempChar.Heal(mHealAmount);
            Destroy(gameObject);
        }
    }
}
