using UnityEngine;
using System.Collections;

public enum TypeOnCell
{
    nothing = 0,
    character,
    enemy
}

public class Cell : MonoBehaviour {

    [HideInInspector]
    public Transform mCellTransform;

    [HideInInspector]
    public Character mCharacterObj;

    [HideInInspector]
    public Enemy mEnemyObj;

    [HideInInspector]
    public IntVector2 mPos;

    public bool mCannotMoveHere;

    //public bool mCharacterOnCell;
    public TypeOnCell mTypeOnCell;

    void Start ()
    {
        mCellTransform = transform;
	}
	
	
	void Update ()
    {
	        
	}

    void OnMouseOver()
    {
        GameManager.sInstance.SetHover(mPos);

        //select block
        if (Input.GetMouseButton(0) && !mCannotMoveHere)
        {
            if (mTypeOnCell != TypeOnCell.character)
            {
                mCharacterObj = null;
            }

            if (mTypeOnCell != TypeOnCell.enemy)
            {
                mEnemyObj = null;
            }

            //print(mPos.x + "," + mPos.y);

            GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mCharacterObj);



        }

        //move
        if (Input.GetMouseButtonDown(1) && !mCannotMoveHere && mTypeOnCell != TypeOnCell.character && GameManager.sInstance.mCharacterSelected)
        {
            GameManager.sInstance.MoveTo(mPos);
        }


        //attack
        if(Input.GetMouseButtonDown(1) && mTypeOnCell == TypeOnCell.enemy && GameManager.sInstance.mMouseMode == MouseMode.Attack && GameManager.sInstance.mCharacterSelected)
        {
            GameManager.sInstance.AttackPos(mPos);
        }
    }
}
