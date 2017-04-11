﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {


    [HideInInspector]
    public Transform mPosition;

    public IntVector2 mCellPos;

    public int mHealth;

    public int mDamage;

    public int mMoveDistance;

    void Start ()
    {
        mPosition = transform;
    }

	void Update ()
    {

        if (mHealth <= 0)
        {
            //Die
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
        }

    }
}