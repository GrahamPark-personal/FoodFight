using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is the base class for all attacks

public class CmdAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //This function will execute the attack (Override in every child class)
    public virtual void Execute(Character target)
    { }





}
