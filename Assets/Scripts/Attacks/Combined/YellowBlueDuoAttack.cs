using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlueDuoAttack : Attack {

    Cell mCell;
    public GameObject mElectricHailStormPrefab;

    public override void Init()
    {
        CreateID();

        SetDamage(5);
        SetRange(5);
        SetSlow(1);
        SetRadius(2);
        SetAOE(9);
        SetEffectDuration(3);
        SetDamageDuration(3);
        GameManager.sInstance.mCurrentRange = GetRange();

    }

    public override void Exit()
    {

    }

    public override void Execute(IntVector2 pos)
    {

        mCell = GameManager.sInstance.mCurrGrid.rows[pos.x].cols[pos.y];

        GameObject temp = Instantiate(mElectricHailStormPrefab);
        SquareDamageDuration mDmgDur = temp.GetComponent<SquareDamageDuration>();
        mDmgDur.mDamage = GetDamage();
        mDmgDur.mSlow = GetSlow();
        mDmgDur.mTurnsLeft = GetEffectDuration();

        GameObject[] visualLocations = new GameObject[GetAOE()];
        Cell[] cellLocations = new Cell[GetAOE()];

        int parcer = 0;

        //cellLocations[parcer] = GetCell(pos.x, pos.y);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x + 1, pos.y);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x, pos.y + 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x + 1, pos.y + 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x - 1, pos.y);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x, pos.y - 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x - 1, pos.y - 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x + 1, pos.y - 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;

        //cellLocations[parcer] = GetCell(pos.x - 1, pos.y + 1);
        //visualLocations[parcer] = GetObject(cellLocations, parcer);
        //parcer++;


    }

    Cell GetCell(int x, int y)
    {
        return GameManager.sInstance.mCurrGrid.rows[x].cols[y];
    }

    GameObject GetObject(Cell[] cellLocations, int i)
    {
        return Instantiate(GameManager.sInstance.mLightUp, cellLocations[i].gameObject.transform.position, cellLocations[i].gameObject.transform.rotation);
    }

}
