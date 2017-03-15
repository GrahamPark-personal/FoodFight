using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIControl : MonoBehaviour
{

    Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("anim", 0);
    }

    void Update()
    {

    }

    public void OpenLevels()
    {
        anim.SetInteger("anim", 1);
    }
}
