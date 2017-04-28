﻿using UnityEngine;
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
    SpawnMinion,
    Virus,
    TimeBomb,
    None

}

public enum CharacterType
{
    Yellow = 0,
    Blue = 1,
    Brown = 2,
    Red = 3,
    Green = 4,
    Black = 5,
    None
}

public enum BuffID
{
    ThunderCloak = 0,
}

public enum CharacterAnimations
{
    Hit = 1,
    Attack1 = 2,
    Attack2 = 3,
    Idle = 4,
    Deactivated = 6,
}


[System.Serializable]
public struct DualAbilities
{
    public Attack mDuoAbility1;
    public CharacterType ability1Character1;
    public CharacterType ability1Character2;
    public Attack mDuoAbility2;
    public CharacterType ability2Character1;
    public CharacterType ability2Character2;
    public Attack mDuoAbility3;
    public CharacterType ability3Character1;
    public CharacterType ability3Character2;
    public Attack mDuoAbility4;
    public CharacterType ability4Character1;
    public CharacterType ability4Character2;
    public Attack mDuoAbility5;
    public CharacterType ability5Character1;
    public CharacterType ability5Character2;
}


public class Character : MonoBehaviour
{

    public struct StatusAilment
    {

        public AilmentID ID;
        public int duration;
        public int turnsPassed;
        public int extra;
    }

    public struct Buff
    {
        public BuffID ID;
        public int duration;
        public int turnsPassed;
        public int returnedDamage;
        public int slow;
        public int shield;
    }


    public Animator mAnimator;

    [HideInInspector]
    public AIActor mActorComp;

    Renderer[] mMaterialRend;

    //public GameManager mGM = null;

    bool damageOnce = true;

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

    public CharacterAnimations mAnimation;

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

    List<Buff> buffs = new List<Buff>();

    //[HideInInspector]
    public bool Linked = false;

    public bool mHasSpawnedMinion = false;

    //[HideInInspector]
    public Character CharacterLink;

    public Character SpawnedMinion1;
    public Character SpawnedMinion2;

    [HideInInspector]
    public StatusAilment mVirusAilment;

    [HideInInspector]
    public bool patientZero = false;

    [HideInInspector]
    public bool hasVirus = false;

    bool onIce = false;

    IEnumerator TurnBackToIdleAfter()
    {
        yield return new WaitForSeconds(0.5f);
        mAnimation = CharacterAnimations.Idle;
    }

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
        if (ID == AilmentID.Virus)
        {
            mVirusAilment = ailment;
            hasVirus = true;
        }
        statusAilments.Add(ailment);
        print("AddedAilement to " + mCharNumber + ", with attack " + ID + ", total ailments are: " + statusAilments.Count);
    }

    public bool ContainsAilment(AilmentID id)
    {
        for (int i = 0; i < statusAilments.Count; i++)
        {
            if (statusAilments[i].ID == id)
            {
                return true;
            }
        }
        return false;
    }

    public void AddBuff(BuffID id, int duration, int slow, int dmg)
    {
        Buff buff = new Buff();
        buff.ID = id;
        buff.duration = duration;
        buff.turnsPassed = 0;
        buff.slow = slow;
        buff.returnedDamage = dmg;

        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].ID == buff.ID)
            {
                buffs.Remove(buffs[i]);
            }
        }

        buffs.Add(buff);

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
                else if (statusAilments[i].ID == AilmentID.SpawnMinion)
                {
                    mHasSpawnedMinion = false;
                    GameManager.sInstance.mCurrGrid.rows[SpawnedMinion1.mCellPos.y].cols[SpawnedMinion1.mCellPos.x].mCharacterObj = null;
                    GameManager.sInstance.mCurrGrid.rows[SpawnedMinion1.mCellPos.y].cols[SpawnedMinion1.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
                    Destroy(SpawnedMinion1.gameObject);
                    SpawnedMinion1 = null;

                    GameManager.sInstance.mCurrGrid.rows[SpawnedMinion2.mCellPos.y].cols[SpawnedMinion2.mCellPos.x].mCharacterObj = null;
                    GameManager.sInstance.mCurrGrid.rows[SpawnedMinion2.mCellPos.y].cols[SpawnedMinion2.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
                    Destroy(SpawnedMinion2.gameObject);
                    SpawnedMinion2 = null;
                }
                else if (statusAilments[i].ID == AilmentID.Virus)
                {
                    hasVirus = false;
                    patientZero = false;
                }
                else if (statusAilments[i].ID == AilmentID.TimeBomb)
                {
                    Character[] neighbors = new Character[4];
                    neighbors[0] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y - 1].cols[mCellPos.x].GetCharacterObject();
                    neighbors[1] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y + 1].cols[mCellPos.x].GetCharacterObject();
                    neighbors[2] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x - 1].GetCharacterObject();
                    neighbors[3] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x + 1].GetCharacterObject();

                    for (int neighborIndex = 0; neighborIndex < 4; neighborIndex++)
                    {

                        Debug.Log("Inside Neighbor check");
                        if (neighbors[neighborIndex] != null)
                        {
                            Debug.Log("Applying final bomb damage.");
                            neighbors[neighborIndex].Damage(statusAilments[i].extra * 3);
                        }
                    }
                        Damage(statusAilments[i].extra * 2);
                }

                statusAilments.Remove(statusAilments[i]);

            }

        }


    }

    public void clearBuffs()
    {

        for (int i = 0; i < buffs.Count; i++)
        {
            Buff temp = buffs[i];
            temp.turnsPassed++;
            buffs[i] = temp;
            if (buffs[i].duration < buffs[i].turnsPassed)
            {
                buffs.Remove(buffs[i]);
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
            if (statusAilments[i].ID == AilmentID.Virus)
            {
                Damage(statusAilments[i].extra);
            }
            if (statusAilments[i].ID == AilmentID.Heal)
            {
                Heal(statusAilments[i].extra);
            }
            if (statusAilments[i].ID == AilmentID.TimeBomb)
            {
                Character[] neighbors = new Character[4];
                neighbors[0] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y - 1].cols[mCellPos.x].mEnemyObj;
                neighbors[1] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y + 1].cols[mCellPos.x].mEnemyObj;
                neighbors[2] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x - 1].mEnemyObj;
                neighbors[3] = GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x + 1].mEnemyObj;

                for (int neighborIndex = 0; neighborIndex < 4; neighborIndex++)
                {

                    if (neighbors[neighborIndex] != null)
                    {
                        neighbors[neighborIndex].Damage(statusAilments[i].extra);
                    }
                }

            }

        }
    }



    public void CheckBuffs(Character attacker)
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].ID == BuffID.ThunderCloak)
            {
                print("ThunderCloak Code Activated");
                attacker.mMoved = true;
                attacker.mMoveDistance = 0;
                attacker.mHealth -= buffs[i].returnedDamage;
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
        mMaterialRend = GetComponentsInChildren<Renderer>();

        mPosition = transform;
        mFinalPosition = transform;
        mMaxHealth = mHealth;
        mTotalMove = mMoveDistance;

        StartSpeed = GameManager.sInstance.mEntityMoveSpeed;
        speed = StartSpeed;
    }

    public void ResetTurn()
    {
        //mAnimation = CharacterAnimations.Idle;

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

    }

    public void EndCharacterTurn()
    {
        mMoveDistance = 0;
        mMoved = true;
        mAttacked = true;
        //mAnimation = CharacterAnimations.Deactivated;

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
        if(mAnimator != null)
        {
            mAnimator.SetInteger("AnimState", (int)mAnimation);
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
                if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].Banana != null)
                {
                    int bananaHeal = 3;
                    Heal(bananaHeal);
                    Destroy(GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].Banana.gameObject);
                }

                lastBlock = nextBlock;
                tempT = mPath.Dequeue();
                tempV = tempT.position + new Vector3(0, 1, 0);
                nextBlock = mPosPath.Dequeue();

                IntVector2 direction = new IntVector2();

                direction.x = nextBlock.x - lastBlock.x;
                direction.y = nextBlock.y - lastBlock.y;

                if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellTag == CellTag.Fire)
                {
                    //set player on fire
                    if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mTypeOnCell == TypeOnCell.enemy)
                    {
                        Damage(GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellDamage);
                    }
                }

                if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellTag == CellTag.Enchanted)
                {
                    //set player on fire
                    if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mTypeOnCell == TypeOnCell.enemy)
                    {
                        Damage(GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellDamage);
                    }
                    if (GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mTypeOnCell == TypeOnCell.character)
                    {
                        Heal(GameManager.sInstance.mCurrGrid.rows[nextBlock.y].cols[nextBlock.x].mCellDamage - 2);
                    }
                }

                if (patientZero)
                {
                    //find characters around
                    List<Character> charactersAround = aroundCharacter(nextBlock);
                    //add virus ailment
                    foreach (Character item in charactersAround)
                    {
                        if (item != this)
                        {
                            item.AddAilment(AilmentID.Virus, mVirusAilment.duration, mVirusAilment.extra);
                            print("added virus to: " + item);
                        }
                    }

                }


                mMoving = true;
            }


            if (mPath.Count == 0 && !mMoving)
            {
                mRunPath = false;
                onIce = false;
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

    List<Character> aroundCharacter(IntVector2 pos)
    {
        List<Character> characters = new List<Character>();

        IntVector2 tempPos;

        Character tempChar;

        tempPos = pos;
        tempPos.x++;
        if (GameManager.sInstance.IsOnGrid(tempPos))
        {
            tempChar = GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].GetCharacterObject();
            if (tempChar != null)
            {
                characters.Add(tempChar);
            }
        }

        tempPos = pos;
        tempPos.x--;
        if (GameManager.sInstance.IsOnGrid(tempPos))
        {
            tempChar = GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].GetCharacterObject();
            if (tempChar != null)
            {
                characters.Add(tempChar);
            }
        }

        tempPos = pos;
        tempPos.y++;
        if (GameManager.sInstance.IsOnGrid(tempPos))
        {
            tempChar = GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].GetCharacterObject();
            if (tempChar != null)
            {
                characters.Add(tempChar);
            }
        }

        tempPos = pos;
        tempPos.y--;
        if (GameManager.sInstance.IsOnGrid(tempPos))
        {
            tempChar = GameManager.sInstance.mCurrGrid.rows[tempPos.y].cols[tempPos.x].GetCharacterObject();
            if (tempChar != null)
            {
                characters.Add(tempChar);
            }
        }

        return characters;
    }


    IEnumerator ChangeColor()
    {
        Color[] colorList = new Color[mMaterialRend.Length];

        for (int i = 0; i < mMaterialRend.Length; i++)
        {
            colorList[i] = mMaterialRend[i].material.GetColor("_Color");
            mMaterialRend[i].material.SetColor("_Color", Color.red);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < mMaterialRend.Length; i++)
        {
            mMaterialRend[i].material.SetColor("_Color", colorList[i]);
        }
    }

    public void Damage(int amount)
    {
        mAnimation = CharacterAnimations.Hit;
        StartCoroutine(TurnBackToIdleAfter());

        //TODO:: Deal with attack based abilities
        if (mMaterialRend != null)
        {
            StartCoroutine(ChangeColor());
        }

        if (Linked && CharacterLink != null)
        {
            CharacterLink.Heal(amount);
        }

        mHealth -= amount;
        if (mHealth <= 0)
        {
            if(this.tag == "Boss")
            {
                GameManager.sInstance.GameFinished = true;
                GameManager.sInstance.GameWon = true;
                GameManager.sInstance.mWinScreen1.enabled = true;
            }
            mHealth = 0;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[mCellPos.y].cols[mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
            Destroy(this.gameObject);
        }

    }


    public void Damage(Character attacker, int amount)
    {

        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].ID == BuffID.ThunderCloak)
            {
                amount -= buffs[i].shield;
                attacker.mMoveDistance = 0;
                attacker.mMoved = true;
                attacker.Damage(buffs[i].returnedDamage);
            }
        }

        Damage(amount);
    }

    public void Attacking()
    {
        mAnimation = CharacterAnimations.Attack1;
        StartCoroutine(TurnBackToIdleAfter());
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
