using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    public int mHeightValue;

    public IntVector2 mPos;

    public bool mCannotMoveHere;

    //[HideInInspector]
    //public int mCellDamage = 0;

    //public bool mCharacterOnCell;
    public TypeOnCell mTypeOnCell;

    //public CellTag mCellTag = CellTag.None;
    //public List<CellTag> mCellTags = new List<CellTag>();
    public Dictionary<CellTag, int> mCellEffects = new Dictionary<CellTag, int>();

    //public cellEffect mCellEffect;

    List<EffectParameters> mEffectParameters = new List<EffectParameters>();

    GameObject AreaEffectBlock;
    GameObject WallBlock;

    [HideInInspector]
    public GameObject Banana;

    int effectDuration;


    public int GetDamageFromTag(CellTag tag)
    {
        int val = 0;

        if (mCellEffects.ContainsKey(tag))
        {
            val = mCellEffects[tag];
        }

        return val;
    }

    public void AddCellTag(CellTag tag, int damage)
    {
        if (!mCellEffects.ContainsKey(tag))
        {
            mCellEffects.Add(tag, damage);
        }
    }
    public void RemoveCellTag(CellTag tag)
    {
        if (mCellEffects.ContainsKey(tag))
        {
            mCellEffects.Remove(tag);
        }
    }
    public bool HasCellTag(CellTag tag)
    {
        if (mCellEffects.ContainsKey(tag))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddEffect(EffectParameters parm)
    {
        foreach (var item in mEffectParameters)
        {
            if (mEffectParameters.Contains(item))
            {
                Destroy(AreaEffectBlock);
                AreaEffectBlock = null;
                mEffectParameters.Remove(item);
                break;

            }
        }

        if (AreaEffectBlock == null)
        {
            //will have different blocks for each one
            AreaEffectBlock = Instantiate(GameManager.sInstance.mAreaEffectBlock, transform.position, transform.rotation);
        }
        if (parm.Effect == cellEffect.Wall)
        {
            WallBlock = Instantiate(GameManager.sInstance.mWallBlock, transform.position + new Vector3(0, 1, 0), transform.rotation);
            mCannotMoveHere = true;
        }
        else if (parm.Effect == cellEffect.Ice)
        {
            //mCellTag = CellTag.Ice;
            mCellEffects.Add(CellTag.Ice, parm.Damage);
        }
        else if (parm.Effect == cellEffect.Poison)
        {
            mCellEffects.Add(CellTag.Ice, parm.Damage);
            //mCellTag = CellTag.Poison;
        }
        else if (parm.Effect == cellEffect.ElectricHailStorm)
        {
            mCellEffects.Add(CellTag.Enchanted, parm.Damage);
        }



        mEffectParameters.Add(parm);
        int index = mEffectParameters.Count - 1;
        DoEffect(index);
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

    public void DoEffect(int i)
    {
        if (mEffectParameters[i].Effect == cellEffect.ElectricHailStorm || mEffectParameters[i].Effect == cellEffect.nothing)
        {
            if (mTypeOnCell == TypeOnCell.character)
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
            else if (mTypeOnCell == TypeOnCell.enemy)
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
            if (mTypeOnCell == TypeOnCell.character && GameManager.sInstance.mGameTurn == GameTurn.Player)
            {
                if (mEffectParameters[i].Health != 0)
                {
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
    }

    public void CheckEffects()
    {
        if (mCellEffects.ContainsKey(CellTag.Fire))
        {
            //mCellTag = CellTag.None;
            mCellEffects.Remove(CellTag.Fire);
            Destroy(AreaEffectBlock.gameObject);
            AreaEffectBlock = null;
        }
        for (int i = 0; i < mEffectParameters.Count; i++)
        {
            if (mEffectParameters[i].Effect == cellEffect.ElectricHailStorm || mEffectParameters[i].Effect == cellEffect.nothing)
            {
                Debug.Log("Getting to this point in the script");
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

            if (GameManager.sInstance.mGameTurn == GameTurn.Player)
            {


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
                        //mCellTag = CellTag.None;
                        mCellEffects.Remove(CellTag.Ice);
                        if (Banana != null)
                        {
                            Destroy(Banana.gameObject);
                            Banana = null;
                        }

                    }
                    else if (mEffectParameters[i].Effect == cellEffect.Poison)
                    {
                        //mCellTag = CellTag.None;
                        mCellEffects.Remove(CellTag.Poison);
                    }

                    mEffectParameters.Remove(mEffectParameters[i]);
                    Destroy(AreaEffectBlock.gameObject);
                    AreaEffectBlock = null;
                }
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
            //gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    IEnumerator WaitToFixAttack()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.sInstance.mMouseMode = MouseMode.Move;
        GameManager.sInstance.ResetSelected();
        GameManager.sInstance.mUIManager.ResetPopUp(true);
    }

    void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            GameManager.sInstance.mOverBlock = true;
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


                if (GameManager.sInstance.mGameTurn == GameTurn.Player && mCharacterObj != null && (GameManager.sInstance.mMouseMode == MouseMode.Move || GameManager.sInstance.mMouseMode == MouseMode.None))
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
                if (GameManager.sInstance.mGameTurn == GameTurn.Player && !GameManager.sInstance.mLoadingSquares && GameManager.sInstance.mMouseMode == MouseMode.Move)
                {

                    Character mTempChar = GameManager.sInstance.mCharacterObj;
                    GameManager.sInstance.MoveTo(mPos);
                    StartCoroutine(waitToReset(mTempChar));
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
                        Character mTempChar = GameManager.sInstance.mCharacterObj;
                        StartCoroutine(waitToReset(mTempChar));
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
                    //Debug.Log("Got here");
                    GameManager.sInstance.mCharacterObj.Attacking(mPos);
                    GameManager.sInstance.AttackPos(mPos);
                    StartCoroutine(WaitToFixAttack());

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
                            GameManager.sInstance.mCharacterObj.Attacking(mPos);
                            GameManager.sInstance.mMouseMode = MouseMode.Move;
                            //GameManager.sInstance.mCharacterObj.mAnimControl.mState = CharAnimState.Attack;
                            GameManager.sInstance.mCharacterObj.mAttacked = true;
                            //GameManager.sInstance.mCharacterObj = null;
                            //GameManager.sInstance.mCharacterSelected = false;
                            //GameManager.sInstance.mEnemySelected = false;
                            GameManager.sInstance.ResetSelected();
                            GameManager.sInstance.mUIManager.ResetPopUp(true);
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
                GameManager.sInstance.mCharacterObj.Attacking(mPos);
                GameManager.sInstance.mMouseMode = MouseMode.Move;
                //GameManager.sInstance.mCharacterObj.mAnimControl.mState = CharAnimState.Attack;
                GameManager.sInstance.mCharacterObj.mAttacked = true;
                //GameManager.sInstance.mCharacterObj = null;
                //GameManager.sInstance.mCharacterSelected = false;
                //GameManager.sInstance.mEnemySelected = false;
                GameManager.sInstance.ResetSelected();
                GameManager.sInstance.mUIManager.ResetPopUp(true);
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
                GameManager.sInstance.mCharacterObj.Attacking(mPos);
                GameManager.sInstance.mMouseMode = MouseMode.Move;
                GameManager.sInstance.mAttackShape = AttackShape.Area;
                GameManager.sInstance.mCharacterObj.mAttacked = true;
                //GameManager.sInstance.mCharacterObj = null;
                //GameManager.sInstance.mCharacterSelected = false;
                //GameManager.sInstance.mEnemySelected = false;
                GameManager.sInstance.ClearAttack();
                GameManager.sInstance.mUIManager.ResetPopUp(true);
                //GameManager.sInstance.ResetSelected();

            }


            if (GameManager.sInstance.mCanControlEnemies)
            {
                if (Input.GetMouseButtonUp(0) && !mCannotMoveHere && GameManager.sInstance.mMouseMode == MouseMode.Attack && GameManager.sInstance.mEnemySelected && mTypeOnCell != TypeOnCell.enemy)
                {
                    if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                    {
                        if (GameManager.sInstance.mCharacterObj != GameManager.sInstance.mCurrGrid.rows[mPos.y].cols[mPos.x])
                        {
                            GameManager.sInstance.AttackPos(mPos);
                            GameManager.sInstance.mCharacterObj.Attacking(mPos);
                            GameManager.sInstance.ClearAttack();
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0) && !mCannotMoveHere && GameManager.sInstance.mMouseMode == MouseMode.AbilityAttack && GameManager.sInstance.mEnemySelected && mTypeOnCell != TypeOnCell.enemy)
                {
                    if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
                    {
                        if (GameManager.sInstance.mCharacterObj != GameManager.sInstance.mCurrGrid.rows[mPos.y].cols[mPos.x])
                        {
                            AttackManager.sInstance.RunAttack(mPos);
                            GameManager.sInstance.mCharacterObj.Attacking(mPos);
                            GameManager.sInstance.ClearAttack();
                        }
                    }
                }
            }

        }


    }

    void OnMouseEnter()
    {

        GameManager.sInstance.mHoverBlock.SetActive(true);

        //if (GameManager.sInstance.IsOnGridAndCanMoveTo(mPos) && !GameManager.sInstance.mCurrGrid.rows[mPos.y].cols[mPos.x].mCannotMoveHere && GameManager.sInstance.mMouseMode != MouseMode.AbilityAttack)
        //{
        //    GameManager.sInstance.mHoverBlock.SetActive(false);
        //    GameManager.sInstance.mHoverBlock.SetActive(true);
        //}
        //else
        //{
        //    GameManager.sInstance.mHoverBlock.SetActive(false);
        //}

        if (GameManager.sInstance.mMouseMode != MouseMode.Move && GameManager.sInstance.IsInAttackArea(mPos))
        {
            GameManager.sInstance.mHoverBlock.SetActive(false);

            foreach (GameObject item in GameManager.sInstance.mPreviewBlocks)
            {
                Destroy(item.gameObject);
            }
            GameManager.sInstance.mPreviewBlocks.Clear();

            if (GameManager.sInstance.mPreviewShape == HoverShape.SingleSpot)
            {
                GameObject mTemp = Instantiate(GameManager.sInstance.AttackPreviewBlock, this.transform.position, this.transform.rotation);
                mTemp.transform.localScale = new Vector3(this.transform.localScale.x, mTemp.transform.localScale.y, this.transform.localScale.z);
                GameManager.sInstance.mPreviewBlocks.Add(mTemp);
            }
            else if (GameManager.sInstance.mPreviewShape == HoverShape.Row)
            {
                //from start to current position, create a row 3 x Y
                GameManager.sInstance.AddPreviewRow(GameManager.sInstance.mCharacterObj.mCellPos, mPos);
            }
            else if (GameManager.sInstance.mPreviewShape == HoverShape.Square)
            {
                //create square with a radius
                GameManager.sInstance.AddPreviewSquare(mPos, GameManager.sInstance.mPreviewRadius);
            }
            else if (GameManager.sInstance.mPreviewShape == HoverShape.WallSurround)
            {
                //draw the block from certain pos, use stuff from brownblack
                IntVector2 tempVector = new IntVector2();
                int radius = GameManager.sInstance.mPreviewRadius;
                tempVector = mPos;
                tempVector.y += radius;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.y -= radius;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.x += radius;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.x -= radius;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector.x += 1;
                tempVector.y -= 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector.x += 1;
                tempVector.y -= 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.y -= radius;

                tempVector.x += 1;
                tempVector.y += 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector.x += 1;
                tempVector.y += 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.x += radius;

                tempVector.x -= 1;
                tempVector.y += 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector.x -= 1;
                tempVector.y += 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector = mPos;
                tempVector.y += radius;

                tempVector.x -= 1;
                tempVector.y -= 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);

                tempVector.x -= 1;
                tempVector.y -= 1;
                GameManager.sInstance.AddPreviewBlock(tempVector);
            }
        }

    }

    void OnMouseExit()
    {
        GameManager.sInstance.mOverBlock = false;
        foreach (GameObject item in GameManager.sInstance.mPreviewBlocks)
        {
            Destroy(item.gameObject);
        }
        GameManager.sInstance.mPreviewBlocks.Clear();
    }

    IEnumerator waitToReset(Character mChar)
    {
        yield return new WaitForSeconds(0.1f);
        if (mChar != null)
        {
            GameManager.sInstance.mUIManager.SelectCharacter(mChar.mCellPos);
            GameManager.sInstance.mUIManager.ResetPopUp(true);
        }
        else
        {
            Debug.Log("character object is null");
        }

    }
}
