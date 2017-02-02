using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TypeOnCell
{
    nothing = 0,
    character,
    enemy
}

public enum cellEffect
{
    nothing = 0,
    ElectricHailStorm = 1
}

public struct EffectParameters
{
    public int Damage;
    public int Health;
    public int Slow;
    public int Stun;
    public int Poison;
    public int Range;
    public int Radius;
    public int AOE;
    public int EffectDuration;
    public int DamageDuration;
    public int ID;
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

    public cellEffect mCellEffect;

    List<EffectParameters> mEffectParameters = new List<EffectParameters>();

    GameObject AreaEffectBlock;

    int effectDuration;

    public void AddEffect(EffectParameters parm)
    {
        foreach (var item in mEffectParameters)
        {
            if (mEffectParameters.Contains(item))
            {
                Destroy(AreaEffectBlock);
                AreaEffectBlock = null;
                mEffectParameters.Remove(item);
            }
        }
        effectDuration = parm.EffectDuration;
        AreaEffectBlock = Instantiate(GameManager.sInstance.mAreaEffectBlock, transform.position, transform.rotation);

        mEffectParameters.Add(parm);
    }

    public void CheckEffects()
    {
        //print("count: " + mEffectParameters.Count);
        for (int i = 0; i < mEffectParameters.Count; i++)
        {
            
            if (mTypeOnCell == TypeOnCell.character && GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                mCharacterObj.mHealth -= mEffectParameters[i].Damage;
                mCharacterObj.mHealth += mEffectParameters[i].Health;
                if (mEffectParameters[i].Poison != 0)
                {
                    mCharacterObj.AddAilment(AilmentID.Poison, mEffectParameters[i].EffectDuration, mEffectParameters[i].Poison);
                }
                if (mEffectParameters[i].Stun != 0)
                {
                    mCharacterObj.AddAilment(AilmentID.Stun, mEffectParameters[i].EffectDuration, 0);
                }
                if (mEffectParameters[i].Slow != 0)
                {
                    mCharacterObj.AddAilment(AilmentID.Slow, mEffectParameters[i].EffectDuration, mEffectParameters[i].Slow);
                }
                //EffectParameters temp = mEffectParameters[i];
                //temp.EffectDuration--;
                //mEffectParameters[i] = temp;
                ////print(temp.EffectDuration);
                //if (mEffectParameters[i].EffectDuration <= 0)
                //{
                //    mEffectParameters.Remove(mEffectParameters[i]);
                //    Destroy(AreaEffectBlock);
                //    AreaEffectBlock = null;
                //}
            }
            else if (mTypeOnCell == TypeOnCell.enemy && GameManager.sInstance.mGameTurn == GameTurn.Enemy)
            {
                mEnemyObj.mHealth -= mEffectParameters[i].Damage;
                mEnemyObj.mHealth += mEffectParameters[i].Health;
                if (mEffectParameters[i].Poison != 0)
                {
                    mEnemyObj.AddAilment(AilmentID.Poison, mEffectParameters[i].EffectDuration, mEffectParameters[i].Poison);
                }
                if (mEffectParameters[i].Stun != 0)
                {
                    mEnemyObj.AddAilment(AilmentID.Stun, mEffectParameters[i].EffectDuration, 0);
                }
                if (mEffectParameters[i].Slow != 0)
                {
                    mEnemyObj.AddAilment(AilmentID.Slow, mEffectParameters[i].EffectDuration, mEffectParameters[i].Slow);
                }
                //EffectParameters temp = mEffectParameters[i];
                //temp.EffectDuration--;
                //mEffectParameters[i] = temp;
                ////print(temp.EffectDuration);
                //if (mEffectParameters[i].EffectDuration <= 0)
                //{
                //    mEffectParameters.Remove(mEffectParameters[i]);
                //    Destroy(AreaEffectBlock);
                //    AreaEffectBlock = null;
                //}
            }

            EffectParameters temp = mEffectParameters[i];
            temp.EffectDuration--;
            mEffectParameters[i] = temp;
            //print(mEffectParameters[i].EffectDuration + "00");
        }


    }

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


            if (GameManager.sInstance.mGameTurn == GameTurn.Player && mCharacterObj != null)
            {
                GameManager.sInstance.mUIManager.SelectCharacter(mPos);
            }


            //print(mPos.x + "," + mPos.y);
            //GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mCharacterObj);



        }

        if (GameManager.sInstance.mCanControlEnemies && Input.GetMouseButton(0) && !mCannotMoveHere)
        {
            if (mTypeOnCell == TypeOnCell.enemy && GameManager.sInstance.mGameTurn == GameTurn.Enemy)
            {

                GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mEnemyObj);
            }
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

                //GameManager.sInstance.mCharacterObj.mAnimControl.mState = CharAnimState.Attack;
            }
        }

        if (Input.GetMouseButtonUp(0) && GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack && GameManager.sInstance.mCharacterSelected && mTypeOnCell != TypeOnCell.character)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                for (int i = 0; i < GameManager.sInstance.mAttackAreaLocations.Count; i++)
                {
                    //check if selected position is in the list of moveable locations
                    if (GameManager.sInstance.mAttackAreaLocations[i].x == mPos.x && GameManager.sInstance.mAttackAreaLocations[i].y == mPos.y)
                    {
                        AttackManager.sInstance.RunAttack(mPos);
                        GameManager.sInstance.mMouseMode = MouseMode.Move;
                        //GameManager.sInstance.mCharacterObj.mAnimControl.mState = CharAnimState.Attack;
                        GameManager.sInstance.mCharacterObj.mAttacked = true;
                        GameManager.sInstance.mCharacterObj = null;
                        GameManager.sInstance.mCharacterSelected = false;
                        GameManager.sInstance.mEnemySelected = false;
                        GameManager.sInstance.ResetSelected();


                    }
                }
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
