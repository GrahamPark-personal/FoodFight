﻿using UnityEngine;
using System.Collections;

public enum AttackType
{
    Melee = 0,
    Ranged
}

public class Character : MonoBehaviour {

    [HideInInspector]
    public Transform mPosition;

    public IntVector2 mCellPos;

    public int mHealth;

    public int mDamage;

    public int mMoveDistance;

    public int mDamageDistance;

    public AttackType mAttackType;

    void Start ()
    {
        mPosition = transform;
	}

	void Update ()
    {
	    if(mHealth <= 0)
        {
            //Die
            GameManager.sInstance.mCurrGrid.rows[mCellPos.x].cols[mCellPos.y].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.x].cols[mCellPos.y].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
        }
	}
}
