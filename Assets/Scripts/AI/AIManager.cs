using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIManager : MonoBehaviour
{

    public static AIManager sInstance = null;

    Stack<Transform> mPath = new Stack<Transform>();
    Stack<IntVector2> mPosPath = new Stack<IntVector2>();


    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void RunAIMove()
    {
        if (GameManager.sInstance.mGameTurn == GameTurn.Enemy)
        {
            StartCoroutine(RunActors());
        }
    }

    IEnumerator RunActors()
    {
        AIActor[] actors = FindObjectsOfType(typeof(AIActor)) as AIActor[];
        foreach (AIActor actor in actors)
        {
            IntVector2 slot = ConquereController.sInstance.FindOpenSpot();

            if (slot.x != -1 && slot.y != -1)
            {
                actor.mDesiredDestination = slot;
            }

            yield return new WaitForSeconds(2.0f);
            Calculate(actor.mCharacter);
        }


        yield return new WaitForSeconds(0.3f);
        GameManager.sInstance.FinishEnemyTurn();
    }

    List<IntVector2> GetArea(IntVector2 pos, int range)
    {
        List<IntVector2> tempList = new List<IntVector2>();

        int colRadius;
        IntVector2 tempVector = new IntVector2();

        for (int i = (range + 1); i > (-range - 1); i--)
        {

            colRadius = range - Mathf.Abs(i);

            for (int j = -colRadius; j < (colRadius + 1); j++)
            {

                tempVector.x = pos.x + j;
                tempVector.y = pos.y + i;

                if (GameManager.sInstance.IsOnGrid(tempVector))
                {
                    tempList.Add(tempVector);
                }


            }
        }

        return tempList;
    }
    List<Character> GetAllEnemiesAround(IntVector2 pos, int range, Character character)
    {
        List<IntVector2> area = GetArea(pos, range);

        List<Character> tempCharArea = new List<Character>();
        foreach (IntVector2 item in area)
        {
            //Debug.Log("pos: " + item.x + ", " + item.y);
            if (GameManager.sInstance.IsOnGrid(item))
            {
                Cell tempCell = GameManager.sInstance.mCurrGrid.rows[item.y].cols[item.x];

                if (tempCell != null)
                {
                    Character tempChar = tempCell.mCharacterObj;
                    if (tempChar != null
                        && tempChar.mCharacterType != CharacterType.None
                        && !IsEqual(tempChar.mCellPos, character.mCellPos)
                        && !tempCharArea.Contains(tempChar))
                    {
                        tempCharArea.Add(tempChar);
                    }
                }
                else
                {
                    Debug.Log("Couldnt get cell");
                }
            }
        }


        return (tempCharArea);

    }

    int Distance(IntVector2 pos, IntVector2 target)
    {
        return Mathf.Abs((target.x - pos.x)) + Mathf.Abs((target.y - pos.y));
    }

    IntVector2 FindClosestEnemy(Character character, IntVector2 pos, int range)
    {
        //mTauntCharacter
        Debug.Log("Pos: " + pos.x + "," + pos.y);
        Debug.Log("Range: " + range);
        Debug.Log("Character: " + character);
        List<Character> mCharacterList = GetAllEnemiesAround(pos, range, character);

        //Debug.Log("total Characters: " + mCharacterList.Count + " ,Character: " + mCharacterList[0]);

        if (character.mTauntCharacter != null)
        {
            if (mCharacterList.Contains(character.mTauntCharacter))
            {
                return (character.mTauntCharacter.mCellPos);
            }
        }

        Character mClosest = null;
        if (mCharacterList.Count > 0)
        {

            foreach (Character item in mCharacterList)
            {
                if (mClosest == null)
                {
                    mClosest = item;
                }
                else
                {
                    if (Distance(pos, item.mCellPos) < Distance(pos, mClosest.mCellPos))
                    {
                        mClosest = item;
                    }
                }
            }
            return mClosest.mCellPos;
        }
        IntVector2 temp = new IntVector2();
        temp.x = -1;
        temp.y = -1;
        return temp;
    }


    bool HasEnemyInArea(IntVector2 pos, int range, Character character)
    {

        if (GetAllEnemiesAround(pos, range, character).Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsEqual(IntVector2 pos1, IntVector2 pos2)
    {
        if (pos1.x == pos2.x && pos1.y == pos2.y)
        {
            return true;
        }

        return false;

    }


    bool HasLocation(List<IntVector2> list, IntVector2 pos)
    {

        foreach (IntVector2 element in list)
        {
            if (IsEqual(element, pos))
            {
                return true;
            }
        }

        return false;
    }


    void AStar(Character character, IntVector2 start, IntVector2 end, int movementPoints)
    {

        bool mNotFullPath = false;

        mPath.Clear();
        mPosPath.Clear();
        character.mPath.Clear();
        character.mPosPath.Clear();


        List<IntVector2> mOpenList = new List<IntVector2>();
        List<IntVector2> mClosedList = new List<IntVector2>();

        start.parent = null;

        start.G = 0;
        start.H = GameManager.sInstance.FindH(start, end);
        start.F = start.G + start.H;


        mOpenList.Add(start);

        IntVector2 mCurrent;

        IntVector2 pos = new IntVector2();

        bool found = false;

        Debug.Log("---------BEGIN-PATHFINDING---------");

        while (mOpenList.Count > 0)
        {
            mCurrent = GameManager.sInstance.lowestFScore(mOpenList);

            mOpenList.Remove(mCurrent);

            List<IntVector2> mNeighbours = GameManager.sInstance.GetEnemyNeighbors(mCurrent);


            for (int i = 0; i < mNeighbours.Count; i++)
            {

                pos = mNeighbours[i];

                if (!HasLocation(mOpenList, pos) && !HasLocation(mClosedList, pos))
                {
                    if (GameManager.sInstance.IsOnGridAndCanMoveTo(pos) && GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].GetCharacterObject() == null)
                    {
                        pos.parent = new IntVector2[1];
                        pos.parent[0] = mCurrent;
                        pos.G = mCurrent.G + GameManager.sInstance.GCost;
                        pos.H = GameManager.sInstance.FindH(pos, end);
                        pos.F = pos.G + pos.H;


                        if (IsEqual(pos, end))
                        {
                            //stop
                            Debug.Log("Found the end");
                            Debug.Log("End: " + end.x + "," + end.y);
                            found = true;
                            break;
                        }

                        mOpenList.Add(pos);
                    }
                    else
                    {
                        if (IsEqual(pos, end))// || GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].GetCharacterObject() != null
                        {

                            pos.parent = new IntVector2[1];
                            pos.parent[0] = mCurrent;
                            pos.G = mCurrent.G + GameManager.sInstance.GCost;
                            pos.H = GameManager.sInstance.FindH(pos, end);
                            pos.F = pos.G + pos.H;

                            found = true;
                            mNotFullPath = true;
                            break;
                        }
                        else if(GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].GetCharacterObject() != null)
                        {
                            pos.parent = new IntVector2[1];
                            pos.parent[0] = mCurrent;
                            pos.G = mCurrent.G + GameManager.sInstance.GCost;
                            pos.H = GameManager.sInstance.FindH(pos, end);
                            pos.F = pos.G + pos.H;
                        }
                    }

                }

                mNeighbours[i] = pos;
            }//end for loop

            if (found)
            {
                break;
            }

            mClosedList.Add(mCurrent);


        }//end while loop

        IntVector2 parcer = pos;
        int posMoved = 0;
        IntVector2 intTemp = new IntVector2();
        IntVector2 mFinalPos = new IntVector2();
        mFinalPos.x = -1;
        mFinalPos.y = -1;
        if (parcer.parent != null)
        {

            do
            {
                AddToPath(parcer);
                parcer = parcer.parent[0];
            }
            while (parcer.parent != null);

            while (mPath.Count > 0 && posMoved < (movementPoints))
            {
                Transform temp = mPath.Pop();
                intTemp = mPosPath.Pop();
                if (!GameManager.sInstance.IsOnGridAndHasNoOneOnBlock(intTemp))
                {
                    break;
                }

                posMoved++;


                //Debug.Log("pos: " + intTemp.x + ", " + intTemp.y);
                mFinalPos = intTemp;
                Debug.Log("Path Pos: " + intTemp.x + "," + intTemp.y);
                character.mPosPath.Enqueue(intTemp);
                character.mPath.Enqueue(temp);



                if (mFinalPos.x != -1 && mFinalPos.y != -1 && (found))
                {
                    Debug.Log("Broke-between");
                    Debug.Log("finalPos: " + mFinalPos.x + "," + mFinalPos.y);
                    character.mRunPath = true;
                    GameManager.sInstance.MoveEnemySlot(mFinalPos, character);
                    Debug.Log("Broke-between");

                }
                else
                {
                    Debug.Log("Couldnt find a path");
                }

            }

        }

        Debug.Log("ended with: " + intTemp.x + "," + intTemp.y);
        Debug.Log("---------END-PATHFINDING---------");
    }

    void AddToPath(IntVector2 pos)
    {
        Transform tempT = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform;
        mPath.Push(tempT);
        mPosPath.Push(pos);
    }


    void Attack(Character character, IntVector2 pos)
    {
        Character mCharToAttack = GameManager.sInstance.mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj;
        character.Attacking(pos);
        StartCoroutine(WaitToShowDamage(1.5f, character, mCharToAttack));
    }

    public void Calculate(Character character)
    {
        Debug.Log("-------------------------BEGIN OF CALCULATE----------------------------");
        Debug.Log("Character: " + character);

        //calculate

        if (character.ContainsAilment(AilmentID.Stun))
        {
            Debug.Log("Stunned");
            return;
        }

        //is active
        if (HasEnemyInArea(character.mCellPos, character.mActorComp.mActivationRange, character))
        {
            Debug.Log("Has enemy in area");

            //Is able to attack
            int mOffset = (character.mAttackType == AttackType.Melee) ? -2 : 0; //why -2 though?
            if (HasEnemyInArea(character.mCellPos, (character.mMoveDistance + character.mDamageDistance) - mOffset, character))
            {

                Debug.Log("Can attack");

                IntVector2 mCurrentEnemy = FindClosestEnemy(character, character.mCellPos, (character.mMoveDistance + character.mDamageDistance));

                if (mCurrentEnemy.x != -1 && mCurrentEnemy.y != -1)
                {



                    //Debug.Log("Current Targer: " + GameManager.sInstance.mCurrGrid.rows[mCurrentEnemy.y].cols[mCurrentEnemy.x].mCharacterObj.gameObject);

                    //int TotalMovement = (((Distance(character.mCellPos, mCurrentEnemy)) - ((character.mDamageDistance)) + mOffset)) + 4; //why 4 though?
                    int TotalMovement = 0;

                    int distanceBetweenUnits = Distance(character.mCellPos, mCurrentEnemy);

                    Debug.Log("Distance: " + distanceBetweenUnits);

                    if (distanceBetweenUnits == 1)
                    {
                        TotalMovement = 0;
                    }
                    else if (character.mMoveDistance + character.mDamageDistance > distanceBetweenUnits)
                    {
                        TotalMovement = Mathf.Abs(character.mMoveDistance - character.mDamageDistance);
                        if (TotalMovement == distanceBetweenUnits)
                        {
                            TotalMovement = distanceBetweenUnits - 1;
                        }

                    }
                    else
                    {
                        TotalMovement = character.mMoveDistance;
                        if (TotalMovement > character.mMoveDistance)
                        {
                            TotalMovement = character.mMoveDistance - 1;
                        }
                    }

                    Debug.Log("Total Movement: " + TotalMovement);

                    //Debug.Log("dist away: " + Distance(character.mCellPos, mCurrentEnemy));
                    //Debug.Log("damage dist: " + character.mDamageDistance);

                    if (TotalMovement == 0)
                    {
                        Debug.Log("just attack");
                        //attack
                        if (CanAttackPos(character, mCurrentEnemy))
                        {
                            Attack(character, mCurrentEnemy);
                        }
                    }
                    else if (TotalMovement < 0)
                    {
                        //move back, if ranged attack.
                        if (mOffset == 0)
                        {
                            Debug.Log("melee total movement < 0");
                            //melee, nothing happens here
                        }
                        else
                        {
                            Debug.Log("Ranged total movement < 0");
                            if (CanAttackPos(character, mCurrentEnemy))
                            {
                                Debug.Log("     |_ and can attack the enemy");
                                Attack(character, mCurrentEnemy);
                            }
                        }

                    }
                    else
                    {
                        //A* until no more points, then attack.
                        Debug.Log("Move towards enemy, then attack if possible");

                        AStar(character, character.mCellPos, mCurrentEnemy, TotalMovement);

                        if (CanAttackPos(character, mCurrentEnemy))
                        {
                            Debug.Log("     |_moved towards then attacked");
                            StartCoroutine(WaitToAttack(1.0f, character, mCurrentEnemy));
                        }

                    }

                }
                else
                {
                    Debug.Log("Moving to predefined destination");
                    AStar(character, character.mCellPos, character.mActorComp.mCurrentDestination, character.mMoveDistance);
                }
            }
            else
            {
                //move to current destination
                Debug.Log("Moving to predefined destination");
                AStar(character, character.mCellPos, character.mActorComp.mCurrentDestination, character.mMoveDistance);
            }

        }
    }

    bool CanAttackPos(Character character, IntVector2 pos)
    {
        foreach (IntVector2 currPos in GetArea(character.mCellPos, character.mDamageDistance))
        {
            if (IsEqual(currPos, pos))
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator WaitToShowDamage(float time, Character attacker, Character target)
    {
        yield return new WaitForSeconds(time);
        target.Damage(attacker.mDamage);
    }

    IEnumerator WaitToAttack(float time, Character character, IntVector2 pos)
    {
        yield return new WaitForSeconds(time);
        if (CanAttackPos(character, pos))
        {
            Attack(character, pos);
        }
    }

}
