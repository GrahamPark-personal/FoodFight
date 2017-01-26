﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDualAttack : CmdAttack {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }


    public virtual void Execute(Character attacker, Character target)
    {
        target.mHealth -= attacker.mDamage;
        attacker.mAttacked = true;
        //extra effects
        
    }
}
