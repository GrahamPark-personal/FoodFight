using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

using System.Linq;

public enum MouseMode
{
    Move = 0,
    Attack,
    AbilityAttack
}

public enum GameTurn
{
    Player = 0,
    Enemy
}

public class GameManager : MonoBehaviour
{

    public bool mCanControlEnemies = false;
    Character mEnemyControlled;

    public CameraController mCamControl;

    public UIManager mUIManager;

    [HideInInspector]
    public float moveDistanceThreshold = 0;

    public static GameManager sInstance = null;

    public GameTurn mGameTurn = GameTurn.Player;

    public MouseMode mMouseMode;

    public Grid mCurrGrid;

    public float mEntityMoveSpeed;

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

    public GameObject mPlayerSelectBlock;

    public GameObject mAreaEffectBlock;

    [HideInInspector]
    public bool gridChanged = false;

    //[HideInInspector]
    public Character mCharacterObj = null;

    public Character[] mCharacters;

    public GameObject[] mCharacterHealthBars = null;

    public Character[] mEnemies;

    [HideInInspector]
    public Stack<GameObject> mMoveAreaObjArray = new Stack<GameObject>();

    [HideInInspector]
    public List<IntVector2> mMoveAreaLocations = new List<IntVector2>();


    [HideInInspector]
    public Stack<GameObject> mAttackAreaObjArray = new Stack<GameObject>();

    [HideInInspector]
    public List<IntVector2> mAttackAreaLocations = new List<IntVector2>();

    Queue<Transform> mPath = new Queue<Transform>();
    Queue<IntVector2> mPosPath = new Queue<IntVector2>();

    public Image mWinScreen1;
    public Image mLoseScreen1;


    int GCost = 10;

    int MapCellRemoveAmount = 10;
    private float DrawWait = 0.7f;

    bool CantMoveLeft = false;
    bool CantMoveRight = false;
    bool CantMoveUp = false;
    bool CantMoveDown = false;

    bool canMoveSelectedLeft = true;
    bool canMoveSelectedRight = true;
    bool canMoveSelectedUp = true;
    bool canMoveSelectedDown = true;

    bool moveUp = false;
    bool moveDown = false;
    bool moveRight = false;
    bool moveLeft = false;

    [HideInInspector]
    public int mPlayersMoved = 0;
    [HideInInspector]
    public int mPlayersAttacked = 0;
    [HideInInspector]
    public int mTotalPlayers = 0;

    public int mCurrentRange;

    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        mWinScreen1.enabled = false;
        mLoseScreen1.enabled = false;
    }

    void Start()
    {
        int x, y;

        mTotalPlayers = mCharacters.Length;

        for (int i = 0; i < mCharacters.Length; i++)
        {
            x = mCharacters[i].mCellPos.x;
            y = mCharacters[i].mCellPos.y;

            mCharacters[i].mCharNumber = i;
            if ((x <= mCurrGrid.mSize.x && y <= mCurrGrid.mSize.y) && (x >= 0 && y >= 0))
            {
                mCurrGrid.rows[y].cols[x].mTypeOnCell = TypeOnCell.character;
                mCurrGrid.rows[y].cols[x].mCharacterObj = mCharacters[i];
                mCharacters[i].transform.position = mCurrGrid.rows[y].cols[x].transform.position + new Vector3(0, 1, 0);

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
                mCurrGrid.rows[y].cols[x].mTypeOnCell = TypeOnCell.enemy;
                mCurrGrid.rows[y].cols[x].mEnemyObj = mEnemies[i];
                mEnemies[i].transform.position = mCurrGrid.rows[y].cols[x].transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                Debug.LogError(mEnemies[i].name + "'s start position is out of range\n check its M Cell Pos");
            }

        }
    }


    void movePathSeg(Vector3 tempT)
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FinishEnemyTurn();
        }

        if (mCanControlEnemies)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (mMouseMode == MouseMode.Attack)
                {
                    mMouseMode = MouseMode.Move;
                }
                else if (mMouseMode == MouseMode.Move)
                {
                    mMouseMode = MouseMode.Attack;
                }
            }
        }

    }

    public void SetHover(IntVector2 pos)
    {
        mHoverBlock.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;
    }

    public bool CheckWin()
    {
        foreach (Character var in mEnemies)
        {
            if (var != null && var.tag == "Boss")
            {
                return false;
            }
        }
        mWinScreen1.enabled = true;
        return true;
    }

    public bool CheckLose()
    {
        foreach (Character var in mCharacters)
        {
            if (var != null)
            {
                return false;
            }
        }
        mLoseScreen1.enabled = true;
        return true;
    }


    public void FinishPlayerTurn()
    {
        if (mGameTurn == GameTurn.Player)
        {
            mGameTurn = GameTurn.Enemy;
            for (int i = 0; i < mCharacters.Length; i++)
            {
                mCharacters[i].EndCharacterTurn();
            }

            for (int i = 0; i < mEnemies.Length; i++)
            {

                mEnemies[i].ResetTurn();
                //for (int j = 0; j < mCurrGrid.rows.Length; j++)
                //{
                //    for (int k = 0; k < mCurrGrid.rows[0].cols.Length; k++)
                //    {
                //        mCurrGrid.rows[j].cols[k].CheckEffects();
                //    }
                //}

            }

            ResetSelected();
        }
        else
        {
            Debug.Log("Cannot finish player turn if it is the enemies");

        }
    }

    public void FinishEnemyTurn()
    {
        if (mGameTurn == GameTurn.Enemy)
        {
            mGameTurn = GameTurn.Player;

            for (int j = 0; j < mCurrGrid.rows.Length; j++)
            {
                for (int k = 0; k < mCurrGrid.rows[0].cols.Length; k++)
                {
                    mCurrGrid.rows[j].cols[k].CheckEffects();
                }
            }
            for (int i = 0; i < mCharacters.Length; i++)
            {
                mCharacters[i].ResetTurn();
            }

            for (int i = 0; i < mEnemies.Length; i++)
            {
                mEnemies[i].EndCharacterTurn();
            }

            ResetSelected();
        }
        else
        {
            Debug.Log("Cannot finish enemy turn if it is the players");
        }
    }

    public void MoveTo(IntVector2 pos)
    {
        //sift through all the locations
        for (int i = 0; i < mMoveAreaLocations.Count; i++)
        {
            //check if selected position is in the list of moveable locations
            if (mMoveAreaLocations[i].x == pos.x && mMoveAreaLocations[i].y == pos.y)
            {
                GameObject deleteObj;

                mCharacterObj.mPath.Clear();
                mCharacterObj.mPosPath.Clear();
                mPath.Clear();
                mPosPath.Clear();

                FindPath(pos);

                //Change Characters cell position
                mCurrGrid.rows[mCharacterObj.mCellPos.y].cols[mCharacterObj.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
                if (mCanControlEnemies && mGameTurn == GameTurn.Enemy)
                {
                    mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.enemy;
                    mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj = mCharacterObj;
                }
                else
                {
                    mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell = TypeOnCell.character;
                    mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj = mCharacterObj;
                }
                mCharacterObj.mCellPos = pos;

                IntVector2 tempPos = pos;

                tempPos.x -= 1;

                if (pos.x - 1 >= 0 && mMoveAreaLocations.Contains(tempPos))
                {
                    canMoveSelectedLeft = true;
                    //print("can move left");
                }

                tempPos = pos;
                tempPos.y -= 1;

                if (pos.y - 1 >= 0 && mMoveAreaLocations.Contains(tempPos))
                {
                    //print("can move up");
                    canMoveSelectedUp = true;
                }

                tempPos = pos;
                tempPos.x += 1;

                if (pos.x + 1 <= mCurrGrid.mSize.x && mMoveAreaLocations.Contains(tempPos))
                {
                    //print("can move right");
                    canMoveSelectedRight = true;
                }

                tempPos = pos;
                tempPos.y += 1;

                if (pos.y + 1 <= mCurrGrid.mSize.y && mMoveAreaLocations.Contains(tempPos))
                {
                    //print("can move down");
                    canMoveSelectedDown = true;
                }

                mCharacterObj.RemoveMoves(mPath.Count - 1);

                while (mPath.Count > 0)
                {
                    Transform temp = mPath.Dequeue();
                    mCharacterObj.mPath.Enqueue(temp);
                    IntVector2 intTemp = mPosPath.Dequeue();
                    mCharacterObj.mPosPath.Enqueue(intTemp);
                }

                //mCharacterObj.mFinalPosition.position = mCurrGrid.rows[pos.x].cols[pos.y].mCellTransform.position + new Vector3(0, 1, 0);

                mCharacterObj.mRunPath = true;



                mCharacterObj = null;
                mCharacterSelected = false;
                mEnemySelected = false;


                //change characters physical position
                //mCharacterObj.transform.position = mCurrGrid.rows[pos.x].cols[pos.y].mCellTransform.position + new Vector3(0, 1, 0);


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

    List<IntVector2> GetNeighbors(IntVector2 pos, List<IntVector2> closedList)
    {
        List<IntVector2> temp = new List<IntVector2>();

        IntVector2 cursor;

        cursor = CopyValues(pos);
        cursor.x -= 1;

        if (cursor.x >= 0 && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere && mCurrGrid.rows[cursor.y].cols[cursor.x].mTypeOnCell == TypeOnCell.nothing)
        {
            temp.Add(cursor);
        }

        cursor = CopyValues(pos);
        cursor.x += 1;

        if (cursor.x < mCurrGrid.mSize.x && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere && mCurrGrid.rows[cursor.y].cols[cursor.x].mTypeOnCell == TypeOnCell.nothing)
        {
            temp.Add(cursor);
        }

        cursor = CopyValues(pos);
        cursor.y -= 1;

        if (cursor.y >= 0 && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere && mCurrGrid.rows[cursor.y].cols[cursor.x].mTypeOnCell == TypeOnCell.nothing)
        {
            temp.Add(cursor);
        }

        cursor = CopyValues(pos);
        cursor.y += 1;

        if (cursor.y < mCurrGrid.mSize.y && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere && mCurrGrid.rows[cursor.y].cols[cursor.x].mTypeOnCell == TypeOnCell.nothing)//!closed.Contains(tester) && 
        {
            temp.Add(cursor);
        }

        return temp;

    }

    public Queue<IntVector2> FindPath(IntVector2 Start, IntVector2 End)
    {
        List<IntVector2> openList = new List<IntVector2>();
        List<IntVector2> closedList = new List<IntVector2>();
        List<IntVector2> NeighborList = new List<IntVector2>();


        IntVector2 cursor;

        IntVector2 calculationVector;

        cursor = InitIntVectorValues(0, 0, 0, 0, 0);
        calculationVector = InitIntVectorValues(0, 0, 0, 0, 0);

        openList.Add(Start);

        while (!closedList.Contains(End) && openList.Count > 0)
        {
            cursor = lowestFScore(openList);

            closedList.Add(cursor);
            openList.Remove(cursor); 
             

            calculationVector = CopyValues(cursor);
            calculationVector.x -= 1;

            NeighborList.Clear();
            NeighborList = GetNeighbors(cursor, closedList);

            //parse through the neighbors

            IntVector2 parcer;
            foreach (IntVector2 item in NeighborList)
            {
                parcer = item;
                if (!listContains(openList, parcer) && !listContains(closedList, parcer))
                {

                    //print("not in open");
                    parcer.parent[0] = cursor; //might have to use cells to hold the parent if it doesnt work
                    parcer.G = cursor.G + GCost;
                    parcer.H = FindH(item, End);
                    parcer.F = item.G + item.H;
                    openList.Add(parcer);
                }
                else if (listContains(openList, parcer))
                {

                    //print("in open list");
                    if (item.G < cursor.G)
                    {
                        openList.Remove(parcer);
                        parcer.parent[0] = cursor;
                        parcer.G = cursor.G + GCost;
                        parcer.F = item.G + item.H;
                        openList.Add(parcer);
                    }
                }

            }


        }

        Stack<IntVector2> myPath = new Stack<IntVector2>();

        Queue<IntVector2> mFinalPath = new Queue<IntVector2>();

        IntVector2 parse = cursor;

        while (parse.x != Start.x || parse.y != Start.y)
        {
            myPath.Push(parse);
            parse = parse.parent[0];
        }
        myPath.Push(parse);


        while (myPath.Count > 0)
        {
            parse = myPath.Pop();
            mFinalPath.Enqueue(parse);
        }

        return mFinalPath;


    }


    void FindPath(IntVector2 newPos)
    {

        List<IntVector2> open = new List<IntVector2>();
        List<IntVector2> closed = new List<IntVector2>();


        IntVector2 cursor;
        IntVector2 tester;


        cursor = InitIntVectorValues(0, 0, 0, 0, 0);
        tester = InitIntVectorValues(0, 0, 0, 0, 0);

        IntVector2 startPos = mSelectedCell;

        startPos.G = 0;
        startPos.F = startPos.H;

        open.Add(startPos);

        int timesThrough = 0;


        //print(cursor.x + "," + cursor.y);

        while (!closed.Contains(newPos) && open.Count > 0)
        {

            cursor = lowestFScore(open);
            closed.Add(cursor);
            open.Remove(cursor);

            if (cursor.x == newPos.x && cursor.y == newPos.y)
            {
                //print("found path");
                break;
            }

            tester = CopyValues(cursor);
            tester.x -= 1;

            if (tester.x >= 0 && !mCurrGrid.rows[tester.y].cols[tester.x].mCannotMoveHere && mCurrGrid.rows[tester.y].cols[tester.x].mTypeOnCell == TypeOnCell.nothing && !listContains(closed, tester))
            {
                if (!listContains(open, tester))
                {
                    //print("not in open");
                    tester.parent[0] = cursor; //might have to use cells to hold the parent if it doesnt work
                    tester.G = cursor.G + GCost;
                    tester.H = FindH(tester, newPos);
                    tester.F = tester.G + tester.H;
                    open.Add(tester);
                }
                else if (listContains(open, tester))
                {
                    //print("in open list");
                    if (tester.G < cursor.G)
                    {
                        tester.parent[0] = cursor;
                        tester.G = cursor.G + GCost;
                        tester.F = tester.G + tester.H;
                    }
                }
            }

            tester = CopyValues(cursor);
            tester.x += 1;

            if (tester.x < mCurrGrid.mSize.x && !mCurrGrid.rows[tester.y].cols[tester.x].mCannotMoveHere && mCurrGrid.rows[tester.y].cols[tester.x].mTypeOnCell == TypeOnCell.nothing && !listContains(closed, tester))
            {
                if (!listContains(open, tester))
                {
                    //print("not in open");
                    tester.parent[0] = cursor; //might have to use cells to hold the parent if it doesnt work
                    tester.G = cursor.G + GCost;
                    tester.H = FindH(tester, newPos);
                    tester.F = tester.G + tester.H;
                    open.Add(tester);
                }
                else if (listContains(open, tester))
                {
                    //print("in open list");
                    if (tester.G < cursor.G)
                    {
                        tester.parent[0] = cursor;
                        tester.G = cursor.G + GCost;
                        tester.F = tester.G + tester.H;
                    }
                }
            }

            tester = CopyValues(cursor);
            tester.y -= 1;

            if (tester.y >= 0 && !mCurrGrid.rows[tester.y].cols[tester.x].mCannotMoveHere && mCurrGrid.rows[tester.y].cols[tester.x].mTypeOnCell == TypeOnCell.nothing && !listContains(closed, tester))
            {
                if (!listContains(open, tester))
                {
                    //print("not in open");
                    tester.parent[0] = cursor; //might have to use cells to hold the parent if it doesnt work
                    tester.G = cursor.G + GCost;
                    tester.H = FindH(tester, newPos);
                    tester.F = tester.G + tester.H;
                    open.Add(tester);
                }
                else if (listContains(open, tester))
                {
                    //print("in open list");
                    if (tester.G < cursor.G)
                    {
                        tester.parent[0] = cursor;
                        tester.G = cursor.G + GCost;
                        tester.F = tester.G + tester.H;
                    }
                }
            }

            tester = CopyValues(cursor);
            tester.y += 1;

            if (tester.y < mCurrGrid.mSize.y && !mCurrGrid.rows[tester.y].cols[tester.x].mCannotMoveHere && mCurrGrid.rows[tester.y].cols[tester.x].mTypeOnCell == TypeOnCell.nothing && !listContains(closed, tester))//!closed.Contains(tester) && 
            {
                if (!listContains(open, tester))
                {
                    //print("not in open");
                    tester.parent[0] = cursor; //might have to use cells to hold the parent if it doesnt work
                    tester.G = cursor.G + GCost;
                    tester.H = FindH(tester, newPos);
                    tester.F = tester.G + tester.H;
                    open.Add(tester);
                }
                else if (listContains(open, tester))
                {
                    //print("in open list");
                    if (tester.G < cursor.G)
                    {
                        tester.parent[0] = cursor;
                        tester.G = cursor.G + GCost;
                        tester.F = tester.G + tester.H;
                    }
                }
            }

        }


        //cursor = end
        //startpos = start

        Stack<IntVector2> myPath = new Stack<IntVector2>();

        IntVector2 parse = cursor;

        while (parse.x != startPos.x || parse.y != startPos.y)
        {
            myPath.Push(parse);
            parse = parse.parent[0];
        }
        myPath.Push(parse);


        while (myPath.Count > 0)
        {
            parse = myPath.Pop();
            AddToPath(parse);

        }


        //print("Open:" + open.Count);
        //print("Closed:" + closed.Count);

        //AddToPath(newPos);

    }



    bool listContains(List<IntVector2> tempList, IntVector2 vect)
    {
        bool finalDecision = false;

        foreach (IntVector2 item in tempList)
        {
            if (item.x == vect.x && item.y == vect.y)
            {
                finalDecision = true;
            }
        }

        return finalDecision;
    }

    IntVector2 lowestFScore(List<IntVector2> tempList)
    {
        IntVector2 smallF = InitIntVectorValues(0, 0, 0, 0, 0);
        int smallestValue = 999999999;
        int newValue = 0;

        foreach (IntVector2 item in tempList)
        {
            newValue = Mathf.Min(smallestValue, item.F);
            if (newValue != smallestValue)
            {
                smallF = item;
            }
            smallestValue = newValue;
        }

        return smallF;
    }

    int FindH(IntVector2 pos, IntVector2 newPos)
    {
        int temp = (Math.Abs(newPos.x - pos.x) + Math.Abs(newPos.y - pos.y)) * 10;
        return temp;
    }

    IntVector2 CopyValues(IntVector2 other)
    {
        IntVector2 temp;

        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        temp.x = other.x;
        temp.y = other.y;
        temp.F = other.F;
        temp.G = other.G;
        temp.H = other.H;

        return temp;
    }

    IntVector2 InitIntVectorValues(int sX, int sY, int sF, int sG, int sH)
    {
        IntVector2 temp = new IntVector2();
        temp.x = sX;
        temp.y = sY;
        temp.F = sF;
        temp.G = sG;
        temp.H = sH;
        temp.parent = new IntVector2[1];

        return temp;
    }


    void AddToPath(IntVector2 pos)
    {

        Transform tempT = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform;
        mPath.Enqueue(tempT);
        mPosPath.Enqueue(pos);
    }

    void GetMoves(IntVector2 currPos)
    {

        IntVector2 tempPos;

        moveUp = false;
        moveDown = false;
        moveRight = false;
        moveLeft = false;

        tempPos = currPos;
        tempPos.y -= 1;

        if (mMoveAreaLocations.Contains(tempPos))
        {
            moveUp = true;
        }

        tempPos = currPos;
        tempPos.y += 1;

        if (mMoveAreaLocations.Contains(tempPos))
        {
            moveDown = true;
        }

        tempPos = currPos;
        tempPos.x -= 1;

        if (mMoveAreaLocations.Contains(tempPos))
        {
            moveLeft = true;
        }

        tempPos = currPos;
        tempPos.x += 1;

        if (mMoveAreaLocations.Contains(tempPos))
        {
            moveRight = true;
        }
    }

    //moveUp = false;
    //moveDown = false;
    //moveRight = false;
    //moveLeft = false;

    //tempPos = currPos;
    //tempPos.y -= 1;

    //if (mMoveAreaLocations.Contains(tempPos))
    //{

    //}

    //if ((currPos.y == newPos.y) && (currPos.x != newPos.x) && (!moveRight || !moveLeft))
    //{
    //    //move right(and up or down) // currPos.x < newPos.x // currPos.x > newPos.x // move up or down, then move left or right
    //    if (moveUp && canMoveSelectedUp)
    //    {

    //    }
    //    else if(moveDown && canMoveSelectedDown)
    //    {

    //    }
    //}

    public void MoveCharacterHover(IntVector2 pos)
    {
        HideCharacterHover(true);
        mPlayerSelectBlock.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;
    }

    public void HideCharacterHover(bool hide)
    {
        mPlayerSelectBlock.SetActive(hide);
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
                if ((mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj != null) || (mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj != null && mCanControlEnemies && mGameTurn == GameTurn.Enemy))
                {

                    if (mCanControlEnemies && mGameTurn == GameTurn.Enemy)
                    {
                        mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj.mHealth -= mCharacterObj.mDamage;
                    }
                    else
                    {
                        mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj.mHealth -= mCharacterObj.mDamage;
                    }
                    //print("Attacked Enemy(" + mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj.mHealth + " HP) with " + mCharacterObj.mDamage + "damage");
                }
                mCharacterObj.mAttacked = true;
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

                //clear vector2 move
                mPath.Clear();

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
        else if (mEnemySelected)
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
        if (objOnCell == TypeOnCell.character && mGameTurn == GameTurn.Player && mMouseMode != MouseMode.Attack)
        {
            mUIManager.ResetPopUp(true);
        }
        else if (mMouseMode != MouseMode.Attack)
        {
            mUIManager.ResetPopUp(false);
        }
        mSelectedCell = pos;
        //move lightblock to selected postion
        mLightUp.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;

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
        if ((mCharacterSelected && mGameTurn == GameTurn.Player) || (mEnemySelected && mGameTurn == GameTurn.Enemy && mCanControlEnemies && mEnemySelected))
        {
            mCharacterObj = charObj;
            //MapMoveArea();

            if (mMouseMode == MouseMode.Move)
            {
                if (!mCharacterObj.mMoved)
                {
                    NewMap();
                }
            }
            else if (mMouseMode == MouseMode.Attack && !mCharacterObj.mAttacked)
            {
                //attack
                if (charObj.mAttackType == AttackType.Melee)
                {
                    CrossAttack();
                }
                else if (charObj.mAttackType == AttackType.Ranged)
                {
                    AreaAttack(mCharacterObj.mDamageDistance);
                }
            }
            else if (mMouseMode == MouseMode.AbilityAttack && !mCharacterObj.mAttacked)
            {
                AreaAttack(mCurrentRange);
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

    public void AreaAttack(int distance)
    {

        IntVector2 tempPosition = mSelectedCell;
        int totalTimes = distance;

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
                totalTimes = CreateAttackCell(tempPosition, totalTimes, true);
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


    int CreateAttackCell(IntVector2 tempPosition, int totalTimes, bool Line)
    {
        //print(tempPosition.x + "," + tempPosition.y);
        //CHANGED SO ENEMIES COULD ATTACK PLAYER

        //(!mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere && !Line) && 
        if (((mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.character) || (mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.enemy && mGameTurn == GameTurn.Enemy && mCanControlEnemies)))
        {
            if (!mAttackAreaLocations.Contains(tempPosition) && !mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere)
            {
                //add the location
                mAttackAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mAttackBlock, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.position, transform.rotation);

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

    public void CreateAttackSquare(IntVector2 pos, int radius, EffectParameters effectParm)
    {
        IntVector2 temp = new IntVector2();
        for (int i = pos.x - radius + 1; i < pos.x + radius; i++)
        {
            for (int j = pos.y - radius + 1; j < pos.y + radius; j++)
            {
                temp.x = i;
                temp.y = j;
                mCurrGrid.rows[j].cols[i].AddEffect(effectParm);
                CreateAttackCell(temp);
            }
        }
    }

    void CreateVisualCell(IntVector2 tempPosition)
    {
        if (((mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.character) || (mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.enemy && mGameTurn == GameTurn.Enemy && mCanControlEnemies)))
        {
            if (!mAttackAreaLocations.Contains(tempPosition) && !mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere)
            {



            }

        }
    }
    void CreateAttackCell(IntVector2 tempPosition)
    {
        if (((mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.character) || (mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.enemy && mGameTurn == GameTurn.Enemy && mCanControlEnemies)))
        {
            if (!mAttackAreaLocations.Contains(tempPosition) && !mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere)
            {
                //add the location
                mAttackAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mAttackBlock, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.position, transform.rotation);

                //add the gameobject to the stack
                mAttackAreaObjArray.Push(movePiece);
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

    void UpCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move)


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

                UpCheck(currTimes, totalTimes, tempPosition, move);
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
        if (!mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere && mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.character && mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.enemy)
        {
            if (!mMoveAreaLocations.Contains(tempPosition))
            {
                //add the location
                mMoveAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mMoveAreaPrefab, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.position, transform.rotation);

                //add the gameobject to the stack
                mMoveAreaObjArray.Push(movePiece);
            }

            return totalTimes;
        }
        else
        {
            return (totalTimes - totalTimes); //if there is a character, then remove MapCellRemoveAmount(3) to not break the map.
        }
    }


    //    IntVector2 currPos = mCharacterObj.mCellPos;

    //    mDontGoHere.Clear();

    //        IntVector2 tempPos;
    //        AddToPath(currPos);
    //        //while ((currPos.x != newPos.x) || (currPos.y != newPos.y))
    //        for (int i = 0; i<mMoveAreaLocations.Count; i++)
    //        {
    //            mDontGoHere.Add(currPos);

    //            GetMoves(currPos);

    //            if (moveRight && currPos.x<newPos.x)
    //            {
    //                currPos = CheckPathRight(currPos, newPos);
    //                if (currPos.y != newPos.y)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.y<newPos.y && (moveUp))
    //                    {
    //                        currPos = CheckPathUp(currPos, newPos);
    //}
    //                    else if (currPos.y > newPos.y && (moveDown))
    //                    {
    //                        currPos = CheckPathDown(currPos, newPos);
    //                    }
    //                }
    //            }
    //            else if ((currPos.y == newPos.y) && (!moveUp || !moveDown))
    //            {
    //                if (currPos.y != newPos.y)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.y<newPos.y && (moveUp))
    //                    {
    //                        currPos = CheckPathUp(currPos, newPos);
    //                    }
    //                    else if (currPos.y > newPos.y && (moveDown))
    //                    {
    //                        currPos = CheckPathDown(currPos, newPos);
    //                    }
    //                }
    //            }
    //            if (moveUp && ((currPos.y<newPos.y)))
    //            {
    //                currPos = CheckPathUp(currPos, newPos);
    //                if (currPos.x != newPos.x)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.x<newPos.x && (moveRight))
    //                    {
    //                        currPos = CheckPathRight(currPos, newPos);
    //                    }
    //                    else if (currPos.x > newPos.x && (moveLeft))
    //                    {
    //                        currPos = CheckPathLeft(currPos, newPos);
    //                    }
    //                }
    //            }
    //            else if (((currPos.x == newPos.x) && (!moveLeft || !moveRight)))
    //            {
    //                if (currPos.x != newPos.x)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.x<newPos.x && (moveRight))
    //                    {
    //                        currPos = CheckPathRight(currPos, newPos);
    //                    }
    //                    else if (currPos.x > newPos.x && (moveLeft))
    //                    {
    //                        currPos = CheckPathLeft(currPos, newPos);
    //                    }
    //                }
    //            }

    //            if (moveLeft && ((currPos.x > newPos.x)))
    //            {
    //                currPos = CheckPathLeft(currPos, newPos);
    //                if (currPos.y != newPos.y)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.y<newPos.y && (moveUp))
    //                    {
    //                        currPos = CheckPathUp(currPos, newPos);
    //                    }
    //                    else if (currPos.y > newPos.y && (moveDown))
    //                    {
    //                        currPos = CheckPathDown(currPos, newPos);
    //                    }
    //                }
    //            }
    //            else if (((currPos.y == newPos.y) && (!moveUp || !moveDown)))
    //            {
    //                if (currPos.y != newPos.y)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.y<newPos.y && (moveUp))
    //                    {
    //                        currPos = CheckPathUp(currPos, newPos);
    //                    }
    //                    else if (currPos.y > newPos.y && (moveDown))
    //                    {
    //                        currPos = CheckPathDown(currPos, newPos);
    //                    }
    //                }
    //            }

    //            if (moveDown && ((currPos.y > newPos.y)))
    //            {
    //                currPos = CheckPathDown(currPos, newPos);
    //                if (currPos.x == newPos.x)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.x<newPos.x && (moveRight))
    //                    {
    //                        currPos = CheckPathRight(currPos, newPos);
    //                    }
    //                    else if (currPos.x > newPos.x && (moveLeft))
    //                    {
    //                        currPos = CheckPathLeft(currPos, newPos);
    //                    }
    //                }

    //            }
    //            else if ((currPos.x == newPos.x) && (!moveLeft || !moveRight))
    //            {
    //                if (currPos.x == newPos.x)
    //                {
    //                    mDontGoHere.Add(currPos);
    //                    GetMoves(currPos);

    //                    if (currPos.x<newPos.x && (moveRight))
    //                    {
    //                        currPos = CheckPathRight(currPos, newPos);
    //                    }
    //                    else if (currPos.x > newPos.x && (moveLeft))
    //                    {
    //                        currPos = CheckPathLeft(currPos, newPos);
    //                    }
    //                }
    //            }

    //        }
    //        AddToPath(newPos);

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
