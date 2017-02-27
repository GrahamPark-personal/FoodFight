using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TypeOnCell
{
    nothing = 0,
    character,
    enemy,
    playerMinion
}

public enum CellTag
{
    None = 0,
    Fire,
    Ice,
    Enchanted,
    Poison

}


public enum cellEffect
{
    nothing = 0,
    ElectricHailStorm = 1,
    LightningRod,
    Wall,
    Ice,
    Poison
}

public enum CellActionType
{
    Nothing = 0,
    StartOfTurn,
    EveryStep
}

public struct EffectParameters
{
    public CellActionType CellAction;
    public cellEffect Effect;
    public int Damage;
    public int Health;
    public int Slow;
    public int Stun;
    public int Taunt;
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

    //[HideInInspector]
    public Character mCharacterObj;

    //[HideInInspector]
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

    [HideInInspector]
    public int mCellDamage = 0;

    //public bool mCharacterOnCell;
    public TypeOnCell mTypeOnCell;

    public CellTag mCellTag = CellTag.None;

    public cellEffect mCellEffect;

    List<EffectParameters> mEffectParameters = new List<EffectParameters>();

    GameObject AreaEffectBlock;
    GameObject WallBlock;

    [HideInInspector]
    public GameObject Banana;

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
        AreaEffectBlock = Instantiate(GameManager.sInstance.mAreaEffectBlock, transform.position, transform.rotation);
        if (parm.Effect == cellEffect.Wall)
        {
            WallBlock = Instantiate(GameManager.sInstance.mWallBlock, transform.position + new Vector3(0, 1, 0), transform.rotation);
            mCannotMoveHere = true;
        }
        else if (parm.Effect == cellEffect.Ice)
        {
            mCellTag = CellTag.Ice;
        }
        else if (parm.Effect == cellEffect.Poison)
        {
            mCellTag = CellTag.Poison;
        }
        mEffectParameters.Add(parm);
    }

    public void AddVisualBlock()
    {
        AreaEffectBlock = Instantiate(GameManager.sInstance.mAreaEffectBlock, transform.position, transform.rotation);
    }

    public void AddBanana()
    {
        Banana = Instantiate(GameManager.sInstance.mBanana, transform.position + new Vector3(0, 1, 0), transform.rotation);
    }


    public Character GetCharacterObject()
    {
        if (mTypeOnCell == TypeOnCell.character)
        {
            return mCharacterObj;
        }
        else if (mTypeOnCell == TypeOnCell.enemy)
        {
            return mEnemyObj;
        }
        else
        {
            return null;
        }
    }

    public void CheckEffects()
    {
        if (mCellTag == CellTag.Fire)
        {
            mCellDamage = 0;
            mCellTag = CellTag.None;
            Destroy(AreaEffectBlock.gameObject);
            AreaEffectBlock = null;
        }
        for (int i = 0; i < mEffectParameters.Count; i++)
        {
            if (mEffectParameters[i].Effect == cellEffect.ElectricHailStorm || mEffectParameters[i].Effect == cellEffect.nothing)
            {
                if (mTypeOnCell == TypeOnCell.character && GameManager.sInstance.mGameTurn == GameTurn.Player)
                {
                    if (mEffectParameters[i].Damage != 0)
                    {
                        mCharacterObj.Damage(mEffectParameters[i].Damage);
                    }
                    if (mEffectParameters[i].Health != 0)
                    {
                        mCharacterObj.Heal(mEffectParameters[i].Health);
                    }

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
                    if (mEffectParameters[i].Damage != 0)
                    {
                        mEnemyObj.Damage(mEffectParameters[i].Damage);
                    }
                    if (mEffectParameters[i].Health != 0)
                    {
                        mEnemyObj.Heal(mEffectParameters[i].Health);
                    }

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



            }
            else if (mEffectParameters[i].Effect == cellEffect.LightningRod)
            {
                print("effect noticed");
                if (mTypeOnCell == TypeOnCell.character && GameManager.sInstance.mGameTurn == GameTurn.Player)
                {
                    print("effect and player turn noticed");
                    if (mEffectParameters[i].Health != 0)
                    {
                        print("heal activated");
                        mCharacterObj.Heal(mEffectParameters[i].Health);
                    }
                }
                if (mTypeOnCell == TypeOnCell.enemy && GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    if (mEffectParameters[i].Stun != 0)
                    {
                        mEnemyObj.AddAilment(AilmentID.Stun, mEffectParameters[i].EffectDuration, 0);
                    }
                }

            }

            //TODO::here goes the different types of effects

            EffectParameters temp = mEffectParameters[i];
            temp.EffectDuration--;
            mEffectParameters[i] = temp;

            if (mEffectParameters[i].EffectDuration <= 0)
            {
                if (mEffectParameters[i].Effect == cellEffect.Wall)
                {
                    Destroy(WallBlock.gameObject);
                    mCannotMoveHere = false;
                }
                else if (mEffectParameters[i].Effect == cellEffect.Ice)
                {
                    mCellTag = CellTag.None;
                    if(Banana != null)
                    {
                        Destroy(Banana.gameObject);
                        Banana = null;
                    }
                    
                }
                else if (mEffectParameters[i].Effect == cellEffect.Poison)
                {
                    mCellTag = CellTag.None;
                }

                mEffectParameters.Remove(mEffectParameters[i]);
                Destroy(AreaEffectBlock.gameObject);
                AreaEffectBlock = null;
            }
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
        if (Input.GetMouseButton(0) && !mCannotMoveHere && mTypeOnCell == TypeOnCell.character
            && (GameManager.sInstance.mAttackShape != AttackShape.Heal
            && GameManager.sInstance.mAttackShape != AttackShape.OtherCharacter
            && GameManager.sInstance.mAttackShape != AttackShape.AllCharacters
            && GameManager.sInstance.mAttackShape != AttackShape.OnCell))
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
            if (mTypeOnCell == TypeOnCell.enemy
                && GameManager.sInstance.mGameTurn == GameTurn.Enemy
                && (GameManager.sInstance.mAttackShape != AttackShape.Heal
                && GameManager.sInstance.mAttackShape != AttackShape.OtherCharacter
                && GameManager.sInstance.mAttackShape != AttackShape.AllCharacters
                && GameManager.sInstance.mAttackShape != AttackShape.OnCell))
            {

                GameManager.sInstance.SetSelected(mPos, mTypeOnCell, mEnemyObj);
            }
        }


        //if (mTypeOnCell == TypeOnCell.character)
        //{
        //    if (Input.GetKeyDown(KeyCode.C))
        //    {
        //        Debug.Log("C Key Pressed");
        //        mCharacterObj.AddAilment(AilmentID.Stun, 3, 0);
        //    }
        //}

        //if (mTypeOnCell == TypeOnCell.character)
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Debug.Log("S Key pressed.");
        //        mCharacterObj.AddAilment(AilmentID.Slow, 3, 3);
        //    }
        //}

        //if (mTypeOnCell == TypeOnCell.character)
        //{
        //    if (Input.GetKeyDown(KeyCode.X))
        //    {
        //        Debug.Log("X Key pressed.");
        //        mCharacterObj.AddAilment(AilmentID.Poison, 3, 3);
        //    }
        //}


        //move
        if (Input.GetMouseButtonUp(0) && !mCannotMoveHere
            && mTypeOnCell != TypeOnCell.character
            && GameManager.sInstance.mCharacterSelected
            && mTypeOnCell != TypeOnCell.character)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.MoveTo(mPos);
            }
        }

        if (GameManager.sInstance.mCanControlEnemies)
        {
            if (Input.GetMouseButtonUp(0)
                && !mCannotMoveHere
                && mTypeOnCell != TypeOnCell.character
                && GameManager.sInstance.mEnemySelected
                && mTypeOnCell != TypeOnCell.enemy)
            {
                if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                {
                    GameManager.sInstance.MoveTo(mPos);
                }
            }
        }


        //attack
        if (Input.GetMouseButtonUp(0)
            && GameManager.sInstance.mMouseMode == MouseMode.Attack
            && GameManager.sInstance.mCharacterSelected
            && mTypeOnCell != TypeOnCell.character)
        {
            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                GameManager.sInstance.AttackPos(mPos);

                //GameManager.sInstance.mCharacterObj.mAnimControl.mState = CharAnimState.Attack;
            }
        }

        if (Input.GetMouseButtonUp(0)
            && GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack
            && GameManager.sInstance.mCharacterSelected
            && mTypeOnCell != TypeOnCell.character
            && GameManager.sInstance.mAttackShape != AttackShape.OnCell
            && GameManager.sInstance.mAttackShape != AttackShape.Heal
            && GameManager.sInstance.mAttackShape != AttackShape.OtherCharacter
            && GameManager.sInstance.mAttackShape != AttackShape.AllCharacters)
        {

            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                for (int i = 0; i < GameManager.sInstance.mAttackAreaLocations.Count; i++)
                {
                    //check if selected position is in the list of moveable locations
                    if (GameManager.sInstance.mAttackAreaLocations[i].x == mPos.x
                        && GameManager.sInstance.mAttackAreaLocations[i].y == mPos.y)
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

        if (Input.GetMouseButtonUp(0)
            && GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack
            && GameManager.sInstance.mCharacterSelected
            && GameManager.sInstance.mAttackShape == AttackShape.OnCell)
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

        if (Input.GetMouseButtonUp(0)
            && GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack
            && GameManager.sInstance.mCharacterSelected
            && (GameManager.sInstance.mAttackShape == AttackShape.Heal
            || GameManager.sInstance.mAttackShape == AttackShape.OtherCharacter
            || GameManager.sInstance.mAttackShape == AttackShape.AllCharacters))
        {
            //attack based on clicking on another character
            AttackManager.sInstance.RunAttack(mPos);
            GameManager.sInstance.mMouseMode = MouseMode.Move;
            GameManager.sInstance.mAttackShape = AttackShape.Area;
            GameManager.sInstance.mCharacterObj.mAttacked = true;
            GameManager.sInstance.mCharacterObj = null;
            GameManager.sInstance.mCharacterSelected = false;
            GameManager.sInstance.mEnemySelected = false;
            GameManager.sInstance.ClearAttack();
            //GameManager.sInstance.ResetSelected();
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
