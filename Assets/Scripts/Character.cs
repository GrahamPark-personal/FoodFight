using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum AttackType
{
    Melee = 0,
    Ranged
}

public enum AilmentID
{
    Stun = 0,
    Poison,
    Slow,
    Taunt,
    Heal,
    Link,
    None

}

public enum CharacterType
{
    Yellow,
    Blue,
    Brown,
    Red,
    Green,
    Black,
    None
}

[System.Serializable]
public struct DualAbilities
{
    public Attack mDuoAbility1;
    public Attack mDuoAbility2;
    public Attack mDuoAbility3;
    public Attack mDuoAbility4;
    public Attack mDuoAbility5;
}

public class Character : MonoBehaviour
{

    struct StatusAilment
    {

        public AilmentID ID;
        public int duration;
        public int turnsPassed;
        public int extra;
    }

    public CharacterAnimationControl mAnimControl;

    public GameManager mGM = null;


    [Space(30)]
    public Attack mBasicAbility;

    [Space(10)]
    public DualAbilities mDualAbilities;

    [HideInInspector]
    public Transform mPosition;

    [Space(30)]
    public IntVector2 mCellPos;

    public int mCharNumber;

    public int mHealth;

    private int mMaxHealth;

    public int mDamage;

    public int mMoveDistance;

    public int mDamageDistance;

    public CharacterType mCharacterType;

    public AttackType mAttackType;

    public Direction mDirection;

    public float mRotationSpeed;

    int mCurrDirection;

    [HideInInspector]
    public Cell returnLocation;

    [HideInInspector]
    public bool needsToReturn;

    int mTotalMove;

    [HideInInspector]
    public Queue<Transform> mPath = new Queue<Transform>();
    public Queue<IntVector2> mPosPath = new Queue<IntVector2>();

    [HideInInspector]
    public Transform mFinalPosition;

    [HideInInspector]
    public bool mRunPath = false;

    Vector3 tempV;

    [HideInInspector]
    public float speed;

    float StartSpeed;

    IntVector2 nextBlock;
    IntVector2 lastBlock;

    [HideInInspector]
    public bool mMoved = false;
    [HideInInspector]
    public bool mAttacked = false;

    bool mMoving = false;

    public Character mTarget;

    public Character mTauntCharacter;

    int mAilmentHealth = 0;

    List<StatusAilment> statusAilments = new List<StatusAilment>();

    //[HideInInspector]
    public bool Linked = false;

    //[HideInInspector]
    public Character CharacterLink;

    public void AddAilment(AilmentID ID, int duration, int extra)
    {
        StatusAilment ailment;
        ailment.ID = ID;
        ailment.duration = duration;
        ailment.turnsPassed = 0;
        ailment.extra = extra;

        for (int i = 0; i < statusAilments.Count; i++)
        {
            if (statusAilments[i].ID == ailment.ID)
            {
                statusAilments.Remove(statusAilments[i]);
            }
        }
        if (ID == AilmentID.Heal)
        {
            mAilmentHealth = extra;
            Heal(extra);
        }
        statusAilments.Add(ailment);
        print("AddedAilement to " + mCharNumber + ", with attack " + ID + ", total ailments are: " + statusAilments.Count);
    }

    public void clearAilments()
    {

        for (int i = 0; i < statusAilments.Count; i++)
        {
            StatusAilment temp = statusAilments[i];
            temp.turnsPassed++;
            statusAilments[i] = temp;
            if (statusAilments[i].duration < statusAilments[i].turnsPassed)
            {
                if (statusAilments[i].ID == AilmentID.Heal)
                {
                    if (mHealth > mMaxHealth)
                    {
                        mHealth = mMaxHealth;
                    }
                }
                else if (statusAilments[i].ID == AilmentID.Link)
                {
                    Linked = false;
                    CharacterLink = null;
                }

                statusAilments.Remove(statusAilments[i]);

            }

        }


    }

    public void CheckAilments()
    {
        for (int i = 0; i < statusAilments.Count; i++)
        {
            if (statusAilments[i].ID == AilmentID.Stun)
            {
                print("Stun Code Executed");
                mAttacked = true;
                mMoved = true;
                mMoveDistance = 0;
            }
            if (statusAilments[i].ID == AilmentID.Slow)
            {
                Debug.Log("Slow Code Executed");
                mMoveDistance -= statusAilments[i].extra;
            }
            if (statusAilments[i].ID == AilmentID.Poison)
            {
                Debug.Log("Poison Code Executed");
                Damage(statusAilments[i].extra);
            }

        }
    }

    public int GetMaxHealth()
    {
        return mMaxHealth;
    }

    void GetTarget()
    {
        mTarget = null;
        //do AI Stuff

        foreach (var ailment in statusAilments)
        {
            if (ailment.ID == AilmentID.Taunt)
            {
                mTarget = mTauntCharacter;
                break;
            }

        }

    }

    void Start()
    {
        mPosition = transform;
        mFinalPosition = transform;
        mMaxHealth = mHealth;
        mTotalMove = mMoveDistance;

        StartSpeed = GameManager.sInstance.mEntityMoveSpeed;
        speed = StartSpeed;
    }

    public void ResetTurn()
    {
        mMoveDistance = mTotalMove;
        mMoved = false;
        mAttacked = false;

        GetTarget();

        if (needsToReturn)
        {
            bool isStunned = false;
            foreach (var ailment in statusAilments)
            {
                if (ailment.ID == AilmentID.Stun)
                {
                    isStunned = true;
                    break;
                }

            }
            if (!isStunned)
            {
                GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
                GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;

                mPosition.position = GameManager.sInstance.mCurrGrid.rows[returnLocation.mPos.y].cols[returnLocation.mPos.x].transform.position + new Vector3(0, 1, 0);
                mCellPos = returnLocation.mPos;
                //targetcell is now the end position

                GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.character;
                GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;
                GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCharacterObj = this;


                needsToReturn = false;
            }
        }

        clearAilments();
        CheckAilments();

        if (mAnimControl != null)
        {
            mAnimControl.ChangeState(CharAnimState.Idle);
        }

    }

    public void EndCharacterTurn()
    {
        mMoveDistance = 0;
        mMoved = true;
        mAttacked = true;
        if (mAnimControl != null)
        {
            mAnimControl.ChangeState(CharAnimState.PoweredDown);
        }


    }

    public void RemoveMoves(int amount)
    {
        mMoveDistance -= amount;
        if (mMoveDistance <= 0)
        {
            mMoved = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.sInstance.mCharacters[0].Damage(5);
        }

        if (mAttacked && mMoved)
        {
            EndCharacterTurn();
        }

        if (mRunPath)
        {
            Transform tempT;


            if (mPath.Count > 0 && !mMoving)
            {
                lastBlock = nextBlock;
                tempT = mPath.Dequeue();
                nextBlock = mPosPath.Dequeue();

                if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellTag == CellTag.Fire)
                {
                    //set player on fire
                    if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mTypeOnCell == TypeOnCell.enemy)
                    {
                        Damage(GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellDamage);
                    }
                }


                tempV = tempT.position + new Vector3(0, 1, 0);
                //print(nextBlock.x + "," + nextBlock.y + "|" + lastBlock.x + "," + lastBlock.y + "|||" + mCellPos.x + "," + mCellPos.y);
                mMoving = true;
            }

            if (mPath.Count == 0 && !mMoving)
            {
                mRunPath = false;
            }

        }

        //changes direction of character

        if (nextBlock.x > lastBlock.x)
        {
            mDirection = Direction.pos2;
        }
        else if (nextBlock.x < lastBlock.x)
        {
            mDirection = Direction.pos4;
        }
        else if (nextBlock.y > lastBlock.y)
        {
            mDirection = Direction.pos3;
        }
        else if (nextBlock.y < lastBlock.y)
        {
            mDirection = Direction.pos1;
        }

        if (transform.position == tempV)
        {
            speed = StartSpeed;
            mMoving = false;
        }

        //actual rotation of the character
        if (mDirection == Direction.pos1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(0, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos2)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(90, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos3)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(180, Vector3.up), mRotationSpeed * Time.deltaTime);
        }
        else if (mDirection == Direction.pos4)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(270, Vector3.up), mRotationSpeed * Time.deltaTime);
        }

    }

    public void Damage(int amount)
    {
        //TODO:: Deal with attack based abilities

        if (Linked && CharacterLink != null)
        {
            CharacterLink.Heal(amount);
        }

        mHealth -= amount;
        if (mHealth <= 0)
        {
            mHealth = 0;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
        }
    }

    public void Heal(int amount)
    {
        //TODO:: Deal with heal based abilities

        mHealth += amount;
        if (mHealth > mMaxHealth)
        {
            mHealth = mMaxHealth;
        }


    }

    void FixedUpdate()
    {
        if (mMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tempV, speed * Time.deltaTime);
        }
    }
}
