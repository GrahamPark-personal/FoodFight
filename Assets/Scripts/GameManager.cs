using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MouseMode
{
    Move = 0,
    Attack
}

public class GameManager : MonoBehaviour {

    public static GameManager sInstance = null;

    public MouseMode mMouseMode;

    public Grid mCurrGrid;

    [HideInInspector]
    public IntVector2 mSelectedCell;

    [HideInInspector]
    public bool mCharacterSelected = false;

    [HideInInspector]
    public bool mEnemySelected = false;

    public GameObject mLightUp;

    public GameObject mMoveAreaPrefab;

    public GameObject mHoverBlock;

    public GameObject mAttackBlock;

    [HideInInspector]
    public bool gridChanged = false;

    [HideInInspector]
    public Character mCharacterObj = null;

    public Character[] mCharacters;

    public Enemy[] mEnemies;

    [HideInInspector]
    public Stack<GameObject> mMoveAreaObjArray = new Stack<GameObject>();

    [HideInInspector]
    public List<IntVector2> mMoveAreaLocations = new List<IntVector2>();


    [HideInInspector]
    public Stack<GameObject> mAttackAreaObjArray = new Stack<GameObject>();

    [HideInInspector]
    public List<IntVector2> mAttackAreaLocations = new List<IntVector2>();

    int MapCellRemoveAmount = 3;
    private float DrawWait = 0.7f;

    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
    }

    void Start ()
    {
        int x, y;

        for (int i = 0; i < mCharacters.Length; i++)
        {
            x = mCharacters[i].mCellPos.x;
            y = mCharacters[i].mCellPos.y;

            
            if((x <= mCurrGrid.mSize.x && y <= mCurrGrid.mSize.y) && (x >= 0 && y >= 0))
            {
                mCurrGrid.rows[x].cols[y].mTypeOnCell = TypeOnCell.character;
                mCurrGrid.rows[x].cols[y].mCharacterObj = mCharacters[i];
                mCharacters[i].transform.position = mCurrGrid.rows[x].cols[y].transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                Debug.LogError(mCharacters[i].name + "'s start position is out of range\n check its M Cell Pos");
            }
        }

        for (int i = 0; i < mEnemies.Length; i++)
        {
            x = mEnemies[i].mCellPos.x;
            y = mEnemies[i].mCellPos.y;

            if ((x <= mCurrGrid.mSize.x && y <= mCurrGrid.mSize.y) && (x >= 0 && y >= 0))
            {
                mCurrGrid.rows[x].cols[y].mTypeOnCell = TypeOnCell.enemy;
                mCurrGrid.rows[x].cols[y].mEnemyObj = mEnemies[i];
                mEnemies[i].transform.position = mCurrGrid.rows[x].cols[y].transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                Debug.LogError(mEnemies[i].name + "'s start position is out of range\n check its M Cell Pos");
            }

        }
    }
	
	
	//void Update ()
 //   {

 //   }

    public void SetHover(IntVector2 pos)
    {
        mHoverBlock.transform.position = mCurrGrid.rows[pos.x].cols[pos.y].mCellTransform.position;
    }

    public void MoveTo(IntVector2 pos)
    {
        //sift through all the locations
        for (int i = 0; i < mMoveAreaLocations.Count; i++)
        {
            //check if selected position is in the list of moveable locations
            if(mMoveAreaLocations[i].x == pos.x && mMoveAreaLocations[i].y == pos.y)
            {
                GameObject deleteObj;

                //Change Characters cell position
                mCurrGrid.rows[mCharacterObj.mCellPos.x].cols[mCharacterObj.mCellPos.y].mTypeOnCell = TypeOnCell.nothing;
                mCurrGrid.rows[pos.x].cols[pos.y].mTypeOnCell = TypeOnCell.character;
                mCurrGrid.rows[pos.x].cols[pos.y].mCharacterObj = mCharacterObj;
                mCharacterObj.mCellPos = pos;

                //change characters physical position
                mCharacterObj.transform.position = mCurrGrid.rows[pos.x].cols[pos.y].mCellTransform.position + new Vector3(0, 1, 0);

                mCharacterObj = null;
                mCharacterSelected = false;
                mEnemySelected = false;


                //destroy moveArea prefabs
                while (mMoveAreaObjArray.Count > 0)
                {
                    deleteObj = mMoveAreaObjArray.Pop();
                    Destroy(deleteObj.gameObject);
                }

                //destroy attack prefabs
                while (mAttackAreaObjArray.Count > 0)
                {
                    deleteObj = mAttackAreaObjArray.Pop();
                    Destroy(deleteObj.gameObject);
                }

                //clear locations, and objects
                mMoveAreaLocations.Clear();
                mMoveAreaObjArray.Clear();

                //clear locations, and objects
                mAttackAreaLocations.Clear();
                mAttackAreaLocations.Clear();

                break;
            }
        }
    }

    public void AttackPos(IntVector2 pos)
    {
        //sift through all the locations
        for (int i = 0; i < mAttackAreaLocations.Count; i++)
        {
            //check if selected position is in the list of moveable locations
            if (mAttackAreaLocations[i].x == pos.x && mAttackAreaLocations[i].y == pos.y)
            {
                GameObject deleteObj;

                //Damage Enemy
                if(mCurrGrid.rows[pos.x].cols[pos.y].mEnemyObj != null)
                {
                    mCurrGrid.rows[pos.x].cols[pos.y].mEnemyObj.mHealth -= mCharacterObj.mDamage;
                    print("Attacked Enemy(" + mCurrGrid.rows[pos.x].cols[pos.y].mEnemyObj.mHealth + " HP) with " + mCharacterObj.mDamage + "damage");
                }

                mCharacterObj = null;
                mCharacterSelected = false;
                mEnemySelected = false;


                //destroy moveArea prefabs
                while (mMoveAreaObjArray.Count > 0)
                {
                    deleteObj = mMoveAreaObjArray.Pop();
                    Destroy(deleteObj.gameObject);
                }

                //destroy attack prefabs
                while (mAttackAreaObjArray.Count > 0)
                {
                    deleteObj = mAttackAreaObjArray.Pop();
                    Destroy(deleteObj.gameObject);
                }

                //clear locations, and objects
                mMoveAreaLocations.Clear();
                mMoveAreaObjArray.Clear();

                //clear locations, and objects
                mAttackAreaLocations.Clear();
                mAttackAreaLocations.Clear();

                break;
            }
        }
    }

    public void ResetSelected()
    {
        TypeOnCell tempType = TypeOnCell.character;
        if (mCharacterSelected)
        {
            tempType = TypeOnCell.character;
        }
        else if(mEnemySelected)
        {
            tempType = TypeOnCell.enemy;
        }
        else
        {
            tempType = TypeOnCell.nothing;
        }

        SetSelected(mSelectedCell, tempType, mCharacterObj);
    }

    public void SetSelected(IntVector2 pos, TypeOnCell objOnCell, Character charObj)
    {
        mSelectedCell = pos;
        //move lightblock to selected postion
        mLightUp.transform.position = mCurrGrid.rows[pos.x].cols[pos.y].mCellTransform.position;

        mCharacterSelected = (objOnCell == TypeOnCell.character);

        mEnemySelected = (objOnCell == TypeOnCell.enemy);

        GameObject deleteObj;

        //destroy moveArea/attackArea prefabs
        while (mMoveAreaObjArray.Count > 0)
        {
            deleteObj = mMoveAreaObjArray.Pop();
            Destroy(deleteObj.gameObject);
        }

        while (mAttackAreaObjArray.Count > 0)
        {
            deleteObj = mAttackAreaObjArray.Pop();
            Destroy(deleteObj.gameObject);
        }

        //clear locations, and objects
        mMoveAreaLocations.Clear();
        mMoveAreaObjArray.Clear();

        mAttackAreaLocations.Clear();
        mAttackAreaObjArray.Clear();

        //if the selected block is a character show where it can move to
        if (mCharacterSelected)
        {
            mCharacterObj = charObj;
            //MapMoveArea();

            if(mMouseMode == MouseMode.Move)
            {
                NewMap();
            }
            else if(mMouseMode == MouseMode.Attack)
            {
                //attack
                if(charObj.mAttackType == AttackType.Melee)
                {
                    CrossAttack();
                }
                else if(charObj.mAttackType == AttackType.Ranged)
                {
                    AreaAttack();
                }
            }
        }
        else
        {
            mCharacterObj = null;
        }

        //mLightUp.transform.position += new Vector3(0, 1, 0);
    }

    void CrossAttack()
    {
        IntVector2 tempPosition = mSelectedCell;
        int maxDistance = mCharacterObj.mDamageDistance;
        int currTimes = 0;

        AttackLineRight(currTimes, maxDistance, tempPosition);
        AttackLineLeft(currTimes, maxDistance, tempPosition);
        AttackLineUp(currTimes, maxDistance, tempPosition);
        AttackLineDown(currTimes, maxDistance, tempPosition);

    }

    void AreaAttack()
    {
        IntVector2 tempPosition = mSelectedCell;
        int totalTimes = mCharacterObj.mDamageDistance;

        int currTimes = 0;//for y transversal

        UpCheck(currTimes, totalTimes, tempPosition, false);
        DownCheck(currTimes, totalTimes, tempPosition, false);
        RightCheck(currTimes, totalTimes, tempPosition, false);
        LeftCheck(currTimes, totalTimes, tempPosition, false);

        //for flood
        //StartCoroutine(CellDraw());
    }

    void AttackLineRight(int currTimes, int totalTimes, IntVector2 tempPosition)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x++;

            if (tempPosition.x < mCurrGrid.mSize.x)
            {
                totalTimes = CreateAttackCell(tempPosition, totalTimes,true);
            }
            else
            {
                break;
            }
        }
    }
    void AttackLineLeft(int currTimes, int totalTimes, IntVector2 tempPosition)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x--;

            if (tempPosition.x >= 0)
            {
                totalTimes = CreateAttackCell(tempPosition, totalTimes, true);
            }
            else
            {
                break;
            }
        }
    }
    void AttackLineUp(int currTimes, int totalTimes, IntVector2 tempPosition)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y--;

            if (tempPosition.y >= 0)
            {
                totalTimes = CreateAttackCell(tempPosition, totalTimes, true);
            }
            else
            {
                break;
            }
        }
    }
    void AttackLineDown(int currTimes, int totalTimes, IntVector2 tempPosition)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y++;

            if (tempPosition.y < mCurrGrid.mSize.y)
            {
                totalTimes = CreateAttackCell(tempPosition, totalTimes, true);
            }
            else
            {
                break;
            }
        }
    }


    int CreateAttackCell(IntVector2 tempPosition, int totalTimes,bool Line)
    {
        //print(tempPosition.x + "," + tempPosition.y);
        if (!mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mCannotMoveHere && mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mTypeOnCell != TypeOnCell.character)
        {
            if (!mAttackAreaLocations.Contains(tempPosition))
            {
                //add the location
                mAttackAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mAttackBlock, mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mCellTransform.position, transform.rotation);

                //add the gameobject to the stack
                mAttackAreaObjArray.Push(movePiece);
            }

            return totalTimes;
        }
        else
        {
            if (Line)
            {
                return 0;
            }
            else
            {
                return (totalTimes - MapCellRemoveAmount); //if there is a character, then remove MapCellRemoveAmount(3) to not break the map.
            }
        }
    }


    void NewMap()
    {
        IntVector2 tempPosition = mSelectedCell;
        int totalTimes = mCharacterObj.mMoveDistance;

        int currTimes = 0;//for y transversal

        UpCheck(currTimes, totalTimes, tempPosition, true);
        DownCheck(currTimes, totalTimes, tempPosition, true);
        RightCheck(currTimes, totalTimes, tempPosition, true);
        LeftCheck(currTimes, totalTimes, tempPosition, true);

        //for flood
        //StartCoroutine(CellDraw());

    }

    void UpCheck(int currTimes, int totalTimes, IntVector2 tempPosition,bool move)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y--;
            if (tempPosition.y >= 0)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes,false);
                }

                RightCheck(currTimes, totalTimes, tempPosition, move);
                LeftCheck(currTimes, totalTimes, tempPosition, move);

            }
            else
            {
                break;
            }
        }
    }
    void DownCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y++;
            if (tempPosition.y < mCurrGrid.mSize.y)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }


                RightCheck(currTimes, totalTimes, tempPosition, move);
                LeftCheck(currTimes, totalTimes, tempPosition, move);

            }
            else
            {
                break;
            }
        }
    }
    void RightCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x++;
            if (tempPosition.x < mCurrGrid.mSize.x)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }

                UpCheck(currTimes, totalTimes, tempPosition, move);
                DownCheck(currTimes, totalTimes, tempPosition, move);
            }
            else
            {
                break;
            }
        }
    }
    void LeftCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x--;
            if (tempPosition.x >= 0)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }

                UpCheck(currTimes, totalTimes, tempPosition,move);
                DownCheck(currTimes, totalTimes, tempPosition, move);
            }
            else
            {
                break;
            }
        }
    }

    int CreateMapCell(IntVector2 tempPosition, int totalTimes)
    {
        //print(tempPosition.x + "," + tempPosition.y);
        if (!mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mCannotMoveHere && mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mTypeOnCell != TypeOnCell.character && mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mTypeOnCell != TypeOnCell.enemy)
        {
            if (!mMoveAreaLocations.Contains(tempPosition))
            {
                //add the location
                mMoveAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mCellTransform.position, transform.rotation);

                //add the gameobject to the stack
                mMoveAreaObjArray.Push(movePiece);
            }

            return totalTimes;
        }
        else
        {
            return (totalTimes - MapCellRemoveAmount); //if there is a character, then remove MapCellRemoveAmount(3) to not break the map.
        }
    }


    //flood + old mapping

    //IEnumerator CellDraw()
    //{

    //    for (int i = 0; i < mMoveAreaLocations.Count; i++)
    //    {
    //        yield return new WaitForSeconds(DrawWait);
    //        IntVector2 tempPosition = mMoveAreaLocations[i];
    //        GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[tempPosition.x].cols[tempPosition.y].mCellTransform.position, transform.rotation);
    //        mMoveAreaObjArray.Push(movePiece);

    //    }
    //    //create the visual movement GameObject


    //    //add the gameobject to the stack

    //}

    //OldMapping
    //void MapMoveArea()
    //{
    //    //set temp move to the selected one
    //    IntVector2 moveObj = mSelectedCell;
    //    IntVector2 moveObj2;
    //    int times;

    //    //checkUp(within play area, that you can move there, that it doesnt have a character, it is in range of moveDistance)
    //    moveObj.y--;
    //    while (moveObj.y >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCannotMoveHere && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && (Mathf.Abs(moveObj.y - mSelectedCell.y) <= mCharacterObj.mMoveDistance))
    //    {
    //        //add the location
    //        mMoveAreaLocations.Add(moveObj);

    //        //create the visual movement GameObject
    //        GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCellTransform.position, transform.rotation);

    //        //add the gameobject to the stack
    //        mMoveAreaObjArray.Push(movePiece);

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.y - mSelectedCell.y);
    //        moveObj2.x--;
    //        times++;
    //        while (moveObj2.x >= 0 && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.x - 1 < 0)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.x--;
    //        }

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.y - mSelectedCell.y);
    //        moveObj2.x++;
    //        times++;
    //        while (moveObj2.x < mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.x + 1 > mCurrGrid.mSize.x)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.x++;
    //        }

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.x - mSelectedCell.x);
    //        moveObj2.y++;
    //        times++;
    //        while (moveObj2.y < mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.y + 1 > mCurrGrid.mSize.y)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.y++;
    //        }

    //        //change the block to check
    //        if (moveObj.y + 1 < 0)
    //        {
    //            break;
    //        }
    //        moveObj.y--;
    //    }
    //    moveObj.y++;
    //    if (mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && mCharacterObj.mMoveDistance >= 4)
    //    {
    //        IntVector2 temp = moveObj;
    //        if (temp.y - 2 >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y - 2].mCharacterOnCell)
    //        {
    //            temp.y -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.y + 2 <= mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj.x].cols[moveObj.y + 2].mCharacterOnCell)
    //        {
    //            temp.y += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x - 2 >= 0 && !mCurrGrid.rows[moveObj.x - 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x + 2 <= mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj.x + 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //    }

    //    //checkDown(within play area, that you can move there, that it doesnt have a character, it is in range of moveDistance)
    //    moveObj = mSelectedCell;
    //    moveObj.y++;
    //    while (moveObj.y < mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCannotMoveHere && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && (Mathf.Abs(moveObj.y - mSelectedCell.y) <= mCharacterObj.mMoveDistance))
    //    {
    //        //add the location
    //        mMoveAreaLocations.Add(moveObj);

    //        //create the visual movement GameObject
    //        GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCellTransform.position, transform.rotation);

    //        //add the gameobject to the stack
    //        mMoveAreaObjArray.Push(movePiece);


    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.y - mSelectedCell.y);
    //        moveObj2.x--;
    //        times++;
    //        while (moveObj2.x >= 0 && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.x - 1 < 0)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.x--;
    //        }

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.y - mSelectedCell.y);
    //        moveObj2.x++;
    //        times++;
    //        while (moveObj2.x < mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.x + 1 > mCurrGrid.mSize.x)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.x++;
    //        }


    //        //change the block to check
    //        if (moveObj.y + 1 > mCurrGrid.mSize.y)
    //        {
    //            break;
    //        }
    //        moveObj.y++;
    //    }
    //    moveObj.y--;
    //    if (mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && mCharacterObj.mMoveDistance >= 4)
    //    {
    //        IntVector2 temp = moveObj;
    //        if (temp.y - 2 >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y - 2].mCharacterOnCell)
    //        {
    //            temp.y -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.y + 2 <= mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj.x].cols[moveObj.y + 2].mCharacterOnCell)
    //        {
    //            temp.y += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x - 2 >= 0 && !mCurrGrid.rows[moveObj.x - 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x + 2 <= mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj.x + 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //    }

    //    //checkLeft(within play area, that you can move there, that it doesnt have a character, it is in range of moveDistance)
    //    moveObj = mSelectedCell;
    //    moveObj.x--;
    //    while (moveObj.x >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCannotMoveHere && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && (Mathf.Abs(moveObj.x - mSelectedCell.x) <= mCharacterObj.mMoveDistance))
    //    {
    //        //add the location
    //        mMoveAreaLocations.Add(moveObj);

    //        //create the visual movement GameObject
    //        GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCellTransform.position, transform.rotation);

    //        //add the gameobject to the stack
    //        mMoveAreaObjArray.Push(movePiece);


    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.x - mSelectedCell.x);
    //        moveObj2.y--;
    //        times++;
    //        while (moveObj2.y >= 0 && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.y - 1 < 0)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.y--;
    //        }

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.x - mSelectedCell.x);
    //        moveObj2.y++;
    //        times++;
    //        while (moveObj2.y < mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.y + 1 > mCurrGrid.mSize.y)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.y++;
    //        }




    //        //change the block to check
    //        if (moveObj.x - 1 < 0)
    //        {
    //            break;
    //        }
    //        moveObj.x--;
    //    }
    //    moveObj.x++;
    //    if (mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && mCharacterObj.mMoveDistance >= 4)
    //    {
    //        IntVector2 temp = moveObj;
    //        if (temp.y - 2 >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y - 2].mCharacterOnCell)
    //        {
    //            temp.y -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.y + 2 <= mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj.x].cols[moveObj.y + 2].mCharacterOnCell)
    //        {
    //            temp.y += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x - 2 >= 0 && !mCurrGrid.rows[moveObj.x - 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x + 2 <= mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj.x + 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //    }

    //    //checkRight(within play area, that you can move there, that it doesnt have a character, it is in range of moveDistance)
    //    moveObj = mSelectedCell;
    //    moveObj.x++;
    //    while (moveObj.x < mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCannotMoveHere && !mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && (Mathf.Abs(moveObj.x - mSelectedCell.x) <= mCharacterObj.mMoveDistance))
    //    {
    //        //add the location
    //        mMoveAreaLocations.Add(moveObj);

    //        //create the visual movement GameObject
    //        GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCellTransform.position, transform.rotation);

    //        //add the gameobject to the stack
    //        mMoveAreaObjArray.Push(movePiece);

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.x - mSelectedCell.x);
    //        moveObj2.y--;
    //        times++;
    //        while (moveObj2.y >= 0 && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.y - 1 < 0)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.y--;
    //        }

    //        moveObj2 = moveObj;

    //        times = Mathf.Abs(moveObj2.x - mSelectedCell.x);
    //        moveObj2.y++;
    //        times++;
    //        while (moveObj2.y < mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCannotMoveHere && !mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCharacterOnCell && (times <= mCharacterObj.mMoveDistance))
    //        {
    //            mMoveAreaLocations.Add(moveObj2);

    //            //create the visual movement GameObject
    //            GameObject movePiece2 = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[moveObj2.x].cols[moveObj2.y].mCellTransform.position, transform.rotation);

    //            //add the gameobject to the stack
    //            mMoveAreaObjArray.Push(movePiece2);

    //            //change the block to check
    //            if (moveObj2.y + 1 > mCurrGrid.mSize.y)
    //            {
    //                break;
    //            }
    //            times++;
    //            moveObj2.y++;
    //        }

    //        //change the block to check
    //        if (moveObj.x + 1 > mCurrGrid.mSize.x)
    //        {
    //            break;
    //        }
    //        moveObj.x++;
    //    }
    //    moveObj.x--;
    //    if (mCurrGrid.rows[moveObj.x].cols[moveObj.y].mCharacterOnCell && mCharacterObj.mMoveDistance >= 4)
    //    {
    //        IntVector2 temp = moveObj;
    //        if (temp.y - 2 >= 0 && !mCurrGrid.rows[moveObj.x].cols[moveObj.y - 2].mCharacterOnCell)
    //        {
    //            temp.y -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.y + 2 <= mCurrGrid.mSize.y && !mCurrGrid.rows[moveObj.x].cols[moveObj.y + 2].mCharacterOnCell)
    //        {
    //            temp.y += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x - 2 >= 0 && !mCurrGrid.rows[moveObj.x - 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x -= 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //        temp = moveObj;
    //        if (temp.x + 2 <= mCurrGrid.mSize.x && !mCurrGrid.rows[moveObj.x + 2].cols[moveObj.y].mCharacterOnCell)
    //        {
    //            temp.x += 2;
    //            mMoveAreaLocations.Add(temp);
    //            GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[temp.x].cols[temp.y].mCellTransform.position, transform.rotation);
    //            mMoveAreaObjArray.Push(movePiece);
    //        }
    //    }

    //}
}
