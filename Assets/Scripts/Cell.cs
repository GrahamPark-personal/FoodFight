using UnityEngine;
using System.Collections;

public enum TypeOnCell
{
    nothing = 0,
    character,
    enemy
}

public class Cell : MonoBehaviour
{

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

    void Start()
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

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log(mPos.x + "," + mPos.y);
        }

        //select block
        if (Input.GetMouseButton(0) && !mCannotMoveHere && mTypeOnCell == TypeOnCell.character)
        {
            if (mTypeOnCell != TypeOnCell.character)
            {
                mCharacterObj = null;
            }

            if (mTypeOnCell != TypeOnCell.enemy)
            {
                mEnemyObj = null;
            }


            if(GameManager.sInstance.mGameTurn == GameTurn.Player && mCharacterObj != null)
            {
                GameManager.sInstance.mUIManager.SelectCharacter(mPos);
            }

            if (GameManager.sInstance.mCanControlEnemies)
            {
                if(mTypeOnCell == TypeOnCell.enemy && GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    
                    GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mEnemyObj);
                }
            }

            //print(mPos.x + "," + mPos.y);
            //GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mCharacterObj);



        }


        if (mTypeOnCell == TypeOnCell.character)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("C Key Pressed");
                mCharacterObj.AddAilment(AilmentID.Stun, 3, 0);
            }
        }

        if (mTypeOnCell == TypeOnCell.character)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("S Key pressed.");
                mCharacterObj.AddAilment(AilmentID.Slow, 3, 3);
            }
        }

        if (mTypeOnCell == TypeOnCell.character)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("X Key pressed.");
                mCharacterObj.AddAilment(AilmentID.Poison, 3, 3);
            }
        }


        //move
        if (Input.GetMouseButtonUp(0) && !mCannotMoveHere && mTypeOnCell != TypeOnCell.character && GameManager.sInstance.mCharacterSelected && mTypeOnCell != TypeOnCell.character)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.MoveTo(mPos);
            }
        }

        if (GameManager.sInstance.mCanControlEnemies)
        {
            if (Input.GetMouseButtonUp(0) && !mCannotMoveHere && mTypeOnCell != TypeOnCell.character && GameManager.sInstance.mEnemySelected && mTypeOnCell != TypeOnCell.enemy)
            {
                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.MoveTo(mPos);
                }
            }
        }


        //attack
        if (Input.GetMouseButtonUp(0) && GameManager.sInstance.mMouseMode == MouseMode.Attack && GameManager.sInstance.mCharacterSelected && mTypeOnCell != TypeOnCell.character)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.AttackPos(mPos);
            }
        }

        if (GameManager.sInstance.mCanControlEnemies)
        {
            if (Input.GetMouseButtonUp(0) && !mCannotMoveHere && GameManager.sInstance.mMouseMode == MouseMode.Attack && GameManager.sInstance.mEnemySelected && mTypeOnCell != TypeOnCell.enemy)
            {
                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.AttackPos(mPos);
                }
            }
        }



    }
}
