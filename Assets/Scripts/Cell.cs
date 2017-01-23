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
    public Character mEnemyObj;

    [HideInInspector]
    public int F;
    [HideInInspector]
    public int G;
    [HideInInspector]
    public int H;

    [HideInInspector]
    public Cell mParent;

    public IntVector2 mPos;

    public bool mCannotMoveHere;

    //public bool mCharacterOnCell;
    public TypeOnCell mTypeOnCell;


    int id;

    protected bool Equals(Cell other)
    {
        return id == other.id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Cell)obj);
    }

    public override int GetHashCode()
    {
        return id; //or id.GetHashCode();
    }

    void Start ()
    {
        id = Random.Range(0, 100000000);
        mCellTransform = transform;

        if (mCannotMoveHere)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
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


            GameManager.sInstance.mUIManager.SelectCharacter(mPos);

            //print(mPos.x + "," + mPos.y);
            //GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mCharacterObj);
            


        }

        //move
        if (Input.GetMouseButtonDown(1) && !mCannotMoveHere && mTypeOnCell != TypeOnCell.character && GameManager.sInstance.mCharacterSelected)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.MoveTo(mPos);
            }
        }


        //attack
        if(Input.GetMouseButtonDown(1) && GameManager.sInstance.mMouseMode == MouseMode.Attack && GameManager.sInstance.mCharacterSelected)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.AttackPos(mPos);
            }
        }
    }
}
