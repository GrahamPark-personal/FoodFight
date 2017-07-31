using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public enum MouseMode
{
    Move = 0,
    Attack,
    AbilityAttack,
    None
}

public enum GameTurn
{
    Player = 0,
    Enemy
}

public enum AttackShape
{
    Area,
    Cross,
    OnCell,
    Heal,
    OtherCharacter,
    AreaNoCharacters,
    AllCharacters

}

public enum HoverShape
{
    SingleSpot,
    Square,
    Row,
    WallSurround

}

public enum TypeOfLevel
{
    KillTheBoss,
    KillAllEnemies
}



public class GameManager : MonoBehaviour
{
    #region Variables

    public TypeOfLevel mLevelType;

    public bool mCanControlEnemies = false;

    [HideInInspector]
    public bool GameFinished = false;
    [HideInInspector]
    public bool GameWon = false;
    [HideInInspector]
    public bool GameLost = false;

    public Grid mCurrGrid;

    public float mEntityMoveSpeed;

    public Image mWinScreen1;
    public Image mLoseScreen1;

    public int mCurrentRange;

    public Character mBoss;

    int mTurnCounter = 0;

    [HideInInspector]
    public string mCurrentPartical;

    [HideInInspector]
    public AudioClip mCurrentAudio;

    [HideInInspector]
    public int mOtherCharacterIndex = -1;

    [HideInInspector]
    public Character mSavedCharacter;

    #region ManagersAndClassReferences

    public CameraController mCamControl;

    public CameraRotation mCamRotation;

    public UIManager mUIManager;

    public static GameManager sInstance = null;

    [HideInInspector]
    public bool mLoadingSquares = false;

    [HideInInspector]
    public bool mOverBlock = false;

    #endregion

    #region Enums

    public GameTurn mGameTurn = GameTurn.Player;

    public MouseMode mMouseMode;

    [HideInInspector]
    public AttackShape mAttackShape;

    #endregion


    #region SelectionStatus

    [HideInInspector]
    public IntVector2 mSelectedCell;

    [HideInInspector]
    public bool mCharacterSelected = false;

    [HideInInspector]
    public bool mEnemySelected = false;

    public Character mCharacterObj = null;

    #endregion

    #region VisualBlocks

    public GameObject mLightUp;

    public GameObject mMoveAreaPrefab;
    float mBlockHeightIncrease = 0.01f;

    public GameObject mHoverBlock;

    public GameObject mAttackBlock;

    public GameObject mPlayerSelectBlock;

    public GameObject mAreaEffectBlock;

    public GameObject mWallBlock;

    public GameObject mBanana;

    public GameObject mGridSquare;

    #endregion

    #region CharacterAndEnemyArrays

    public Character[] mCharacters;

    public Character[] mEnemies;

    #endregion

    #region ListsAndStacks

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

    #endregion


    #region RandomAIStuff

    [HideInInspector]
    public int GCost = 10;

    int MapCellRemoveAmount = 10;

    #endregion

    #region PlayerStats

    [HideInInspector]
    public int mPlayersMoved = 0;
    [HideInInspector]
    public int mPlayersAttacked = 0;
    [HideInInspector]
    public int mTotalPlayers = 0;

    #endregion


    #region AttackAbilityStuff

    [HideInInspector]
    public Character[] mTargetChars = new Character[2];

    #endregion

    #region VisualFeedbackStuff

    [Space(20)]

    public GameObject AttackPreviewBlock;
    [HideInInspector]
    public HoverShape mPreviewShape;
    [HideInInspector]
    public List<GameObject> mPreviewBlocks = new List<GameObject>();
    [HideInInspector]
    public int mPreviewRadius = 0;
    [HideInInspector]
    public int mPreviewWidth = 3;
    [HideInInspector]
    public int mPreviewLength = 0;

    #endregion

    #endregion

    [HideInInspector]
    public bool mFinishedLastCutScene = false;

    public GameObject mStarObject;

    [Header("HeathBarStuff")]
    public bool mHealthBarIsVisual;

    //[Header("Status")]
    //public GameObject mStunnedObject;
    //public GameObject mSlowedObject;
    //public GameObject mStormCloak;

    TurnBar mTurnBar;

    #region Functions

    #region DebugArea

    public void ChangeEnemyToMove()
    {
        mMouseMode = MouseMode.Move;
        ResetSelected();
    }
    public void ChangeEnemyToBasicAttack()
    {
        mMouseMode = MouseMode.Attack;
        ResetSelected();
    }
    public void ChangeEnemyToBasicAbility()
    {
        AttackManager.sInstance.SetAttack(mBoss.mBasicAbility);
        mMouseMode = MouseMode.AbilityAttack;
        //ResetSelected();
    }

    public Text mCounter;

    #endregion

    #region AwakeStartUpdate

    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        mWinScreen1.enabled = false;
        mWinScreen1.GetComponentInChildren<Image>().gameObject.SetActive(false);
        mLoseScreen1.enabled = false;
    }

    void Start()
    {
        mTurnBar = GameObject.FindGameObjectWithTag("TurnBar").GetComponent<TurnBar>();
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

    void Update()
    {
        if (mCounter)
        {
            mCounter.text = "Counter: " + mTurnCounter;
        }
        if (mFinishedLastCutScene)
        {
            //do end of level

            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        if (mOverBlock && mMouseMode != MouseMode.AbilityAttack && mMouseMode != MouseMode.Attack)
        {
            //mHoverBlock.SetActive(true);
        }
        else
        {
            //mHoverBlock.SetActive(false);
        }

        if (Input.GetMouseButtonDown(1))// && mMouseMode != MouseMode.Move
        {

            mMouseMode = MouseMode.None;

            SelectionBar.sInstance.AttackReset();


            ResetSelected();

        }

        if (mMouseMode == MouseMode.None)
        {
            mLightUp.SetActive(false);
        }
        else
        {
            mLightUp.SetActive(true);
        }

    }

    void FixedUpdate()
    {
        //mOverBlock = false;
    }

    #endregion

    #region WinLoseChecks

    IEnumerator WaitToLoadNextLevel()
    {
        yield return new WaitForSeconds(2.0f);
        mFinishedLastCutScene = true;
    }


    public bool CheckWin()
    {
        if (mLevelType == TypeOfLevel.KillTheBoss)
        {
            foreach (Character var in mEnemies)
            {

                if (var != null && var.tag == "Boss")
                {
                    return false;
                }
            }
        }
        else if (mLevelType == TypeOfLevel.KillAllEnemies)
        {
            foreach (Character var in mEnemies)
            {

                if (var != null)
                {
                    return false;
                }
            }
        }

        if (mLevelType == TypeOfLevel.KillTheBoss)
        {
            Debug.Log("Got here");
            CutSceneManager.sInstance.SetScene(CutSceneManager.sInstance.mSeconaryScene);
            CutSceneManager.sInstance.mCurrentPhrase = 0;
            CutSceneManager.sInstance.mLastPhrase = true;
            CutSceneManager.sInstance.SetActive(true);
        }
        else if (mLevelType == TypeOfLevel.KillAllEnemies)
        {
            //Debug.Log("Got here 2");
            mFinishedLastCutScene = true;
        }

        mWinScreen1.enabled = true;
        mWinScreen1.GetComponentInChildren<Image>().gameObject.SetActive(true);


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

    public void WonGame()
    {
        if (mLevelType == TypeOfLevel.KillTheBoss)
        {
            Debug.Log("Got here");
            CutSceneManager.sInstance.SetScene(CutSceneManager.sInstance.mSeconaryScene);
            CutSceneManager.sInstance.mCurrentPhrase = 0;
            CutSceneManager.sInstance.mLastPhrase = true;
            CutSceneManager.sInstance.SetActive(true);
        }
        else if (mLevelType == TypeOfLevel.KillAllEnemies)
        {
            //Debug.Log("Got here 2");
            mFinishedLastCutScene = true;
        }

        mWinScreen1.enabled = true;
        mWinScreen1.GetComponentInChildren<Image>().gameObject.SetActive(true);
    }

    public void LostGame()
    {
        mLoseScreen1.enabled = true;
    }

    #endregion

    #region EndTurns

    public void FinishPlayerTurn()
    {
        if (mGameTurn == GameTurn.Player)
        {

            GameSounds.sInstance.PlayAudio("ENEMY_TURN");

            sInstance.CheckLose();
            sInstance.CheckWin();

            mGameTurn = GameTurn.Enemy;
            mTurnBar.ShowBar(true);

            for (int j = 0; j < mCurrGrid.rows.Length; j++)
            {
                for (int k = 0; k < mCurrGrid.rows[0].cols.Length; k++)
                {
                    mCurrGrid.rows[j].cols[k].CheckEffects();
                }
            }

            for (int i = 0; i < mCharacters.Length; i++)
            {
                mCharacters[i].EndCharacterTurn();
            }

            for (int i = 0; i < mEnemies.Length; i++)
            {
                mEnemies[i].ResetTurn();
            }

            AIManager.sInstance.RunAIMove();

            mMouseMode = MouseMode.None;

            SelectionBar.sInstance.RoundCleanUp();

            mUIManager.ResetPopUp(false);
            ResetSelected();

            ConquereController.sInstance.UpdateZone();
            ConquereController.sInstance.DecrementCounters();

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
            sInstance.CheckLose();
            sInstance.CheckWin();

            mGameTurn = GameTurn.Player;

            mTurnBar.ShowBar(false);

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

            mTurnCounter++;

            mCharacterObj = null;


            mMouseMode = MouseMode.None;

            SelectionBar.sInstance.RoundCleanUp();


            mUIManager.ResetPopUp(false);
            ResetSelected();

            mUIManager.IncrementTurn();
            GameSounds.sInstance.PlayAudio("YOUR_TURN");

        }
        else
        {
            Debug.Log("Cannot finish enemy turn if it is the players");
        }
    }

    #endregion

    #region PositionChecks

    public bool IsMovableBlock(IntVector2 pos)
    {
        if (pos.x >= 0 && pos.x <= mCurrGrid.mSize.x) { }
        else
        {
            return false;
        }

        if (pos.y >= 0 && pos.y <= mCurrGrid.mSize.y) { }
        else
        {
            return false;
        }

        if (!mCurrGrid.rows[pos.y].cols[pos.x].mCannotMoveHere) { }
        else
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell == TypeOnCell.nothing) { }
        else
        {
            return false;
        }

        return true;
    }

    public bool IsOnGrid(IntVector2 pos)
    {
        if (pos.x >= 0 && pos.x < mCurrGrid.mSize.x) { }
        else
        {
            return false;
        }

        if (pos.y >= 0 && pos.y < mCurrGrid.mSize.y) { }
        else
        {
            return false;
        }

        //Debug.Log("Allowed: " + pos.x + ", " + pos.y);

        return true;
    }

    public bool IsOnGridAndCanMoveTo(IntVector2 pos)
    {
        if (pos.x >= 0 && pos.x <= mCurrGrid.mSize.x) { }
        else
        {
            return false;
        }

        if (pos.y >= 0 && pos.y <= mCurrGrid.mSize.y) { }
        else
        {
            return false;
        }

        if (mCurrGrid.rows == null)
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols == null)
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x] == null)
        {
            return false;
        }

        if (!mCurrGrid.rows[pos.y].cols[pos.x].mCannotMoveHere) { }
        else
        {
            return false;
        }

        return true;
    }

    public bool IsOnGridAndHasNoOneOnBlock(IntVector2 pos)
    {
        if (pos.x >= 0 && pos.x <= mCurrGrid.mSize.x) { }
        else
        {
            return false;
        }

        if (pos.y >= 0 && pos.y <= mCurrGrid.mSize.y) { }
        else
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj != null)
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj != null)
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x].mCannotMoveHere)
        {
            return false;
        }

        return true;
    }

    public bool IsMovableBlock(Cell cellPos)
    {
        IntVector2 pos = cellPos.mPos;
        if (pos.x >= 0 && pos.x <= mCurrGrid.mSize.x) { }
        else
        {
            return false;
        }

        if (pos.y >= 0 && pos.y <= mCurrGrid.mSize.y) { }
        else
        {
            return false;
        }

        if (!mCurrGrid.rows[pos.y].cols[pos.x].mCannotMoveHere) { }
        else
        {
            return false;
        }

        if (mCurrGrid.rows[pos.y].cols[pos.x].mTypeOnCell == TypeOnCell.nothing) { }
        else
        {
            return false;
        }

        return true;
    }
    #endregion

    #region AStarMovement
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
                mAttackAreaObjArray.Clear();

                break;
            }
        }
    }

    public List<IntVector2> GetNeighbors(IntVector2 pos)
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

    public List<IntVector2> GetEnemyNeighbors(IntVector2 pos)
    {

        List<IntVector2> temp = new List<IntVector2>();

        IntVector2 cursor;

        cursor = CopyValues(pos);
        cursor.x -= 1;

        if (IsOnGrid(cursor))
        {
            if (cursor.x >= 0 && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere)
            {
                temp.Add(cursor);
            }
        }

        cursor = CopyValues(pos);
        cursor.x += 1;

        if (IsOnGrid(cursor))
        {
            if (cursor.x < mCurrGrid.mSize.x && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere)
            {
                temp.Add(cursor);
            }
        }

        cursor = CopyValues(pos);
        cursor.y -= 1;

        if (IsOnGrid(cursor))
        {
            if (cursor.y >= 0 && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere)
            {
                temp.Add(cursor);
            }
        }

        cursor = CopyValues(pos);
        cursor.y += 1;

        if (IsOnGrid(cursor))
        {
            if (cursor.y < mCurrGrid.mSize.y && !mCurrGrid.rows[cursor.y].cols[cursor.x].mCannotMoveHere)
            {
                temp.Add(cursor);
            }
        }
        return temp;

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

    public bool listContains(List<IntVector2> tempList, IntVector2 vect)
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

    public IntVector2 lowestFScore(List<IntVector2> tempList)
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

    public int FindH(IntVector2 pos, IntVector2 newPos)
    {
        int temp = (Math.Abs(newPos.x - pos.x) + Math.Abs(newPos.y - pos.y)) * 10;
        return temp;
    }

    public IntVector2 CopyValues(IntVector2 other)
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

    public IntVector2 InitIntVectorValues(int sX, int sY, int sF, int sG, int sH)
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

    #endregion

    #region VisualHoverBlocks

    public void ChangeHoverObject(int character)
    {
        Debug.Log("got here");
        Transform currTransform = mPlayerSelectBlock.transform;
        Destroy(mPlayerSelectBlock);
        mPlayerSelectBlock = Instantiate(ParticleManager.sInstance.mCharacterParticals[character], currTransform.position + new Vector3(0, 0.5f, 0), currTransform.rotation);

    }

    public void SetHover(IntVector2 pos)
    {
        if (IsOnGridAndCanMoveTo(pos) && mOverBlock)
        {
            mHoverBlock.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;
            mHoverBlock.transform.rotation = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.rotation;
            mHoverBlock.transform.localScale = new Vector3(mCurrGrid.rows[pos.y].cols[pos.x].transform.localScale.x, mHoverBlock.transform.localScale.y, mCurrGrid.rows[pos.y].cols[pos.x].transform.localScale.z);
        }

    }

    public void MoveCharacterHover(IntVector2 pos)
    {
        HideCharacterHover(true);
        mPlayerSelectBlock.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;// + new Vector3(0, 1, 0);
    }

    public void HideCharacterHover(bool hide)
    {
        mPlayerSelectBlock.SetActive(hide);
    }

    public void AddPreviewBlock(IntVector2 pos)
    {
        if (IsOnGridAndCanMoveTo(pos))
        {
            Transform mCellTransform = mCurrGrid.rows[pos.y].cols[pos.x].transform;
            GameObject mTemp = Instantiate(AttackPreviewBlock, mCellTransform.position, mCellTransform.rotation);
            mTemp.transform.localScale = new Vector3(mCellTransform.localScale.x, mTemp.transform.localScale.y, mCellTransform.localScale.z);
            mPreviewBlocks.Add(mTemp);
        }
    }

    public void AddPreviewRow(IntVector2 Start, IntVector2 End)
    {
        string dir = "";
        if (Start.x > End.x)
        {
            dir = "Up";
        }
        else if (Start.x < End.x)
        {
            dir = "Down";
        }
        else if (Start.y > End.y)
        {
            dir = "Right";
        }
        else if (Start.y < End.y)
        {
            dir = "Left";
        }

        print("Direction: " + dir);

        IntVector2 temp = new IntVector2();
        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        if (dir == "Up")
        {
            print("Start: " + Start.x + "," + Start.y);
            print("End: " + End.x + "," + End.y);

            for (int i = Start.x; i > End.x; i--)
            {
                //print("Got here!!");

                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    // mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                    //mCellDamage
                    AddPreviewBlock(temp);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Down")
        {
            for (int i = Start.x; i < End.x; i++)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Right")
        {
            for (int i = Start.y; i > End.y; i--)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Left")
        {
            for (int i = Start.y; i < End.y; i++)
            {
                print("Got here");
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    AddPreviewBlock(temp);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
    }

    public void AddPreviewSquare(IntVector2 pos, int radius)
    {
        IntVector2 temp = new IntVector2();
        for (int i = pos.x - radius + 1; i < pos.x + radius; i++)
        {
            for (int j = pos.y - radius + 1; j < pos.y + radius; j++)
            {
                temp.x = i;
                temp.y = j;
                if (IsOnGridAndCanMoveTo(temp))
                {
                    AddPreviewBlock(temp);
                }

                //CreateAttackCell(temp);
            }
        }
    }

    #endregion

    #region AttackShapes

    void CreateCharacterTargets(IntVector2 pos)
    {
        print("Pos: " + pos.x + "," + +pos.y);

        foreach (Character item in mCharacters)
        {
            print("Name: " + item);
            print("itemPos: " + item.mCellPos.x + "," + +item.mCellPos.y);
            Character mParseChar = mCurrGrid.rows[item.mCellPos.y].cols[item.mCellPos.x].mCharacterObj;
            if (mParseChar != mCharacterObj)
            {
                mAttackAreaLocations.Add(item.mCellPos);

                //create the visual movement GameObject
                GameObject tempObject = mMoveAreaPrefab;
                tempObject.transform.localScale = mCurrGrid.rows[item.mCellPos.y].cols[item.mCellPos.x].mCellTransform.localScale;
                tempObject.transform.localScale = new Vector3(tempObject.transform.localScale.x, tempObject.transform.localScale.y + mBlockHeightIncrease, tempObject.transform.localScale.z);
                GameObject movePiece = (GameObject)Instantiate(tempObject, mCurrGrid.rows[item.mCellPos.y].cols[item.mCellPos.x].mCellTransform.position, mCurrGrid.rows[item.mCellPos.y].cols[item.mCellPos.x].mCellTransform.rotation);

                //add the gameobject to the stack
                mAttackAreaObjArray.Push(movePiece);
            }
        }
    }


    void CreateOnCellTarget(IntVector2 pos)
    {
        mAttackAreaLocations.Add(pos);

        //create the visual movement GameObject
        GameObject tempObject = mMoveAreaPrefab;
        tempObject.transform.localScale = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.localScale;
        tempObject.transform.localScale = new Vector3(tempObject.transform.localScale.x, tempObject.transform.localScale.y + mBlockHeightIncrease, tempObject.transform.localScale.z);
        GameObject movePiece = (GameObject)Instantiate(tempObject, mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position, mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.rotation);

        //add the gameobject to the stack
        mAttackAreaObjArray.Push(movePiece);
    }
    void CreateTargetAttack(IntVector2 pos, int radius)
    {
        Character tempCharacter;
        if (mTargetChars[0] == mCharacterObj)
        {
            tempCharacter = mTargetChars[1];
        }
        else
        {
            tempCharacter = mTargetChars[0];
        }

        List<Cell> mCells = new List<Cell>();

        mCells = GetCellsInRange(pos, radius);

        foreach (Cell item in mCells)
        {
            if (!item.mCannotMoveHere)
            {
                Character mParseChar = mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCharacterObj;
                if (mParseChar == tempCharacter)
                {
                    mAttackAreaLocations.Add(item.mPos);

                    //create the visual movement GameObject
                    GameObject tempObject = mMoveAreaPrefab;
                    tempObject.transform.localScale = mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.localScale;
                    tempObject.transform.localScale = new Vector3(tempObject.transform.localScale.x, tempObject.transform.localScale.y + mBlockHeightIncrease, tempObject.transform.localScale.z);
                    GameObject movePiece = (GameObject)Instantiate(tempObject, mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.position, mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.rotation);

                    //add the gameobject to the stack
                    mAttackAreaObjArray.Push(movePiece);
                }
            }

        }


    }

    void CrossAttack()
    {
        mAttackAreaLocations.Clear();
        IntVector2 tempPosition = mSelectedCell;
        int maxDistance = mCharacterObj.mDamageDistance;
        int currTimes = 0;

        AttackLineRight(currTimes, maxDistance, tempPosition);
        AttackLineLeft(currTimes, maxDistance, tempPosition);
        AttackLineUp(currTimes, maxDistance, tempPosition);
        AttackLineDown(currTimes, maxDistance, tempPosition);

    }

    void CrossAttack(int Range)
    {
        mAttackAreaLocations.Clear();
        IntVector2 tempPosition = mSelectedCell;
        int maxDistance = Range;
        int currTimes = 0;

        AttackLineRight(currTimes, maxDistance, tempPosition);
        AttackLineLeft(currTimes, maxDistance, tempPosition);
        AttackLineUp(currTimes, maxDistance, tempPosition);
        AttackLineDown(currTimes, maxDistance, tempPosition);

    }

    void createAreaNoCharacterAttack(IntVector2 pos, int radius)
    {
        List<Cell> mCells = new List<Cell>();

        mCells = GetCellsInRange(pos, radius);

        foreach (Cell item in mCells)
        {
            if (!item.mCannotMoveHere)
            {
                //add the location
                mAttackAreaLocations.Add(item.mPos);

                //create the visual movement GameObject
                GameObject movePiece = (GameObject)Instantiate(mAttackBlock, mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.position, mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.rotation);

                //add the gameobject to the stack
                mAttackAreaObjArray.Push(movePiece);
            }
        }

    }

    void createHealAttack(IntVector2 pos, int radius)
    {
        List<Cell> mCells = new List<Cell>();

        mCells = GetCellsInRange(pos, radius);

        foreach (Cell item in mCells)
        {
            if (!item.mCannotMoveHere)
            {
                //add the location
                mAttackAreaLocations.Add(item.mPos);

                //create the visual movement GameObject
                GameObject tempObject = mMoveAreaPrefab;
                tempObject.transform.localScale = mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.localScale;
                tempObject.transform.localScale = new Vector3(tempObject.transform.localScale.x, tempObject.transform.localScale.y + mBlockHeightIncrease, tempObject.transform.localScale.z);
                GameObject movePiece = (GameObject)Instantiate(tempObject, mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.position, mCurrGrid.rows[item.mPos.y].cols[item.mPos.x].mCellTransform.rotation);

                //add the gameobject to the stack
                mAttackAreaObjArray.Push(movePiece);
            }
        }

    }

    public void AreaAttack(int distance)
    {
        mAttackAreaLocations.Clear();
        IntVector2 tempPosition = mSelectedCell;
        int totalTimes = distance;

        int currTimes = 0;

        int startHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;

        UpCheck(currTimes, totalTimes, tempPosition, false, startHeight);
        DownCheck(currTimes, totalTimes, tempPosition, false, startHeight);
        RightCheck(currTimes, totalTimes, tempPosition, false, startHeight);
        LeftCheck(currTimes, totalTimes, tempPosition, false, startHeight);
    }

    #endregion

    #region AbilityAttacks

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
                GameObject movePiece = (GameObject)Instantiate(mAttackBlock, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.position, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.rotation);

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

    public void CreateAttackSquare(IntVector2 pos, int radius, EffectParameters effectParm, bool ignoreMiddle)
    {
        IntVector2 temp = new IntVector2();
        for (int i = pos.x - radius + 1; i < pos.x + radius; i++)
        {
            for (int j = pos.y - radius + 1; j < pos.y + radius; j++)
            {
                temp.x = i;
                temp.y = j;
                if (IsOnGridAndCanMoveTo(temp))
                {
                    mCurrGrid.rows[j].cols[i].AddEffect(effectParm);
                }

                //CreateAttackCell(temp);
            }
        }
    }


    public List<IntVector2> GetRowLocations(IntVector2 Start, IntVector2 End)
    {
        List<IntVector2> locations = new List<IntVector2>();

        string dir = "";
        if (Start.x > End.x)
        {
            dir = "Up";
        }
        else if (Start.x < End.x)
        {
            dir = "Down";
        }
        else if (Start.y > End.y)
        {
            dir = "Right";
        }
        else if (Start.y < End.y)
        {
            dir = "Left";
        }


        IntVector2 temp = new IntVector2();
        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        if (dir == "Up")
        {

            for (int i = Start.x; i > End.x; i--)
            {
                //print("Got here!!");

                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }
            }
        }
        else if (dir == "Down")
        {
            for (int i = Start.x; i < End.x; i++)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }
            }
        }
        else if (dir == "Right")
        {
            for (int i = Start.y; i > End.y; i--)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }
            }
        }
        else if (dir == "Left")
        {
            for (int i = Start.y; i < End.y; i++)
            {
                print("Got here");
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    locations.Add(temp);

                }
            }
        }

        return locations;
    }

    public void CreateRowEffect(IntVector2 Start, IntVector2 End, CellTag tag, int damage)
    {
        string dir = "";
        if (Start.x > End.x)
        {
            dir = "Up";
        }
        else if (Start.x < End.x)
        {
            dir = "Down";
        }
        else if (Start.y > End.y)
        {
            dir = "Right";
        }
        else if (Start.y < End.y)
        {
            dir = "Left";
        }

        print("Direction: " + dir);

        IntVector2 temp = new IntVector2();
        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        if (dir == "Up")
        {
            print("Start: " + Start.x + "," + Start.y);
            print("End: " + End.x + "," + End.y);

            for (int i = Start.x; i > End.x; i--)
            {
                //print("Got here!!");

                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    // mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                    //mCellDamage
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Down")
        {
            for (int i = Start.x; i < End.x; i++)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Right")
        {
            for (int i = Start.y; i > End.y; i--)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Left")
        {
            for (int i = Start.y; i < End.y; i++)
            {
                print("Got here");
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddCellTag(tag, damage);
                    mCurrGrid.rows[temp.y].cols[temp.x].AddVisualBlock(tag);
                    //mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
    }

    public void CreateRowEffect(IntVector2 Start, IntVector2 End, EffectParameters effectParm)
    {
        string dir = "";
        if (Start.x > End.x)
        {
            dir = "Up";
        }
        else if (Start.x < End.x)
        {
            dir = "Down";
        }
        else if (Start.y > End.y)
        {
            dir = "Right";
        }
        else if (Start.y < End.y)
        {
            dir = "Left";
        }

        print("Direction: " + dir);

        IntVector2 temp = new IntVector2();
        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        if (dir == "Up")
        {
            print("Start: " + Start.x + "," + Start.y);
            print("End: " + End.x + "," + End.y);

            for (int i = Start.x; i > End.x; i--)
            {
                //print("Got here!!");

                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Down")
        {
            for (int i = Start.x; i < End.x; i++)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Right")
        {
            for (int i = Start.y; i > End.y; i--)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
        else if (dir == "Left")
        {
            for (int i = Start.y; i < End.y; i++)
            {
                print("Got here");
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddEffect(effectParm);
                }
            }
        }
    }

    //mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();

    public void CreateBananas(IntVector2 Start, IntVector2 End)
    {
        string dir = "";
        if (Start.x > End.x)
        {
            dir = "Up";
        }
        else if (Start.x < End.x)
        {
            dir = "Down";
        }
        else if (Start.y > End.y)
        {
            dir = "Right";
        }
        else if (Start.y < End.y)
        {
            dir = "Left";
        }

        print("Direction: " + dir);

        IntVector2 temp = new IntVector2();
        temp = InitIntVectorValues(0, 0, 0, 0, 0);

        if (dir == "Up")
        {
            print("Start: " + Start.x + "," + Start.y);
            print("End: " + End.x + "," + End.y);

            for (int i = Start.x; i > End.x; i--)
            {
                //print("Got here!!");

                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }
            }
        }
        else if (dir == "Down")
        {
            for (int i = Start.x; i < End.x; i++)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = i;
                temp.y = Start.y;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = i;
                temp.y = Start.y - 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = i;
                temp.y = Start.y + 1;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }
            }
        }
        else if (dir == "Right")
        {
            for (int i = Start.y; i > End.y; i--)
            {
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }
            }
        }
        else if (dir == "Left")
        {
            for (int i = Start.y; i < End.y; i++)
            {
                print("Got here");
                //for each one from up to down add one if there is a space to.

                temp.x = Start.x;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = Start.x - 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }

                temp.x = Start.x + 1;
                temp.y = i;

                if (IsOnGrid(temp))
                {
                    mCurrGrid.rows[temp.y].cols[temp.x].AddBanana();
                }
            }
        }


    }



    //public void CreateBananas(IntVector2 Start, IntVector2 End)
    //{     
    //    Debug.Log("Start = " + Start.x + " , " + Start.y);
    //    Debug.Log("End = " + End.x + " , " + End.y);

    //    for (int i = 0; i < End.y - Start.y; i++)
    //    {
    //        for (int j = 0; j <= 1; j++)
    //        {
    //            //Cell currentCell = mCurrGrid.rows[Start.x].cols[Start.y + j];

    //            Debug.Log("Banana Created at " + Start.x + i + " , " + End.x + j);
    //            mCurrGrid.rows[Start.x + i].cols[Start.y + j].AddBanana();
    //        }
    //    }

    //}



    #endregion

    #region OtherAttackStuff

    public List<Character> GetCharactersInArea()
    {
        List<Character> temp = new List<Character>();
        foreach (IntVector2 item in mAttackAreaLocations)
        {
            if (mCurrGrid.rows[item.y].cols[item.x].mTypeOnCell != TypeOnCell.nothing)
            {
                if (mCurrGrid.rows[item.y].cols[item.x].mTypeOnCell == TypeOnCell.character)
                {
                    temp.Add(mCurrGrid.rows[item.y].cols[item.x].mCharacterObj);
                }
                else if (mCurrGrid.rows[item.y].cols[item.x].mTypeOnCell == TypeOnCell.enemy)
                {
                    temp.Add(mCurrGrid.rows[item.y].cols[item.x].mEnemyObj);
                }
            }
        }

        return temp;
    }


    public bool IsInAttackArea(IntVector2 pos)
    {
        foreach (IntVector2 item in mAttackAreaLocations)
        {
            if (pos.x == item.x && pos.y == item.y)
            {
                return true;
            }
        }
        return false;
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



    void NewMap()
    {

        IntVector2 tempPosition = mSelectedCell;
        int totalTimes = mCharacterObj.mMoveDistance;

        int currTimes = 0;//for y transversal

        int startHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;

        UpCheck(currTimes, totalTimes, tempPosition, true, startHeight);
        DownCheck(currTimes, totalTimes, tempPosition, true, startHeight);
        RightCheck(currTimes, totalTimes, tempPosition, true, startHeight);
        LeftCheck(currTimes, totalTimes, tempPosition, true, startHeight);

        StartCoroutine(WaitToAllowInput());

        //for flood
        //StartCoroutine(CellDraw());

    }

    IEnumerator WaitToAllowInput()
    {
        yield return new WaitForSeconds(0.1f);
        mLoadingSquares = false;
    }

    void UpCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move, int lastHeight)
    {

        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y--;
            if (tempPosition.y >= 0)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes, lastHeight);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }
                lastHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;
                RightCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
                LeftCheck(currTimes, totalTimes, tempPosition, move, lastHeight);

            }
            else
            {
                break;
            }
        }
    }
    void DownCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move, int lastHeight)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.y++;
            if (tempPosition.y < mCurrGrid.mSize.y)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes, lastHeight);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }

                lastHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;
                RightCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
                LeftCheck(currTimes, totalTimes, tempPosition, move, lastHeight);

            }
            else
            {
                break;
            }
        }
    }
    void RightCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move, int lastHeight)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x++;
            if (tempPosition.x < mCurrGrid.mSize.x)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes, lastHeight);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }
                lastHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;
                UpCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
                DownCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
            }
            else
            {
                break;
            }
        }
    }
    void LeftCheck(int currTimes, int totalTimes, IntVector2 tempPosition, bool move, int lastHeight)
    {
        while (currTimes < totalTimes)
        {
            currTimes++;
            tempPosition.x--;
            if (tempPosition.x >= 0)
            {
                if (move)
                {
                    totalTimes = CreateMapCell(tempPosition, totalTimes, lastHeight);
                }
                else
                {
                    totalTimes = CreateAttackCell(tempPosition, totalTimes, false);
                }
                lastHeight = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mHeightValue;
                UpCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
                DownCheck(currTimes, totalTimes, tempPosition, move, lastHeight);
            }
            else
            {
                break;
            }
        }
    }

    int HeightDifference(int last, IntVector2 current)
    {
        Cell currentCell = mCurrGrid.rows[current.y].cols[current.x];

        int diff = Mathf.Abs(last - currentCell.mHeightValue);
        return diff;

    }

    int CreateMapCell(IntVector2 tempPosition, int totalTimes, int lastHeight)
    {
        //print(tempPosition.x + "," + tempPosition.y);
        if (!mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCannotMoveHere && mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.character && mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mTypeOnCell != TypeOnCell.enemy)
        {
            if (!mMoveAreaLocations.Contains(tempPosition))
            {
                if (HeightDifference(lastHeight, tempPosition) > 1)
                {
                    //Debug.Log("Height Too Big");
                    return 0;
                }

                //add the location
                mMoveAreaLocations.Add(tempPosition);

                //create the visual movement GameObject
                GameObject tempObject = mMoveAreaPrefab;
                tempObject.transform.localScale = mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.localScale;
                tempObject.transform.localScale = new Vector3(tempObject.transform.localScale.x, tempObject.transform.localScale.y + mBlockHeightIncrease, tempObject.transform.localScale.z);
                GameObject movePiece = (GameObject)Instantiate(tempObject, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.position, mCurrGrid.rows[tempPosition.y].cols[tempPosition.x].mCellTransform.rotation);

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

    public void ClearAttack()
    {
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
    }

    #endregion

    #region Useful&UsedAlot

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
                        mCurrGrid.rows[pos.y].cols[pos.x].mCharacterObj.Damage(mCharacterObj.mDamage);
                    }
                    else
                    {
                        mCurrGrid.rows[pos.y].cols[pos.x].mEnemyObj.Damage(mCharacterObj.mDamage);
                    }
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
                mAttackAreaObjArray.Clear();

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

    IEnumerator removeAllParticles()
    {
        yield return new WaitForSeconds(8.0f);
        ParticleManager.sInstance.RemoveAllObjectsFromList();
    }

    public void SetSelected(IntVector2 pos, TypeOnCell objOnCell, Character charObj)
    {
        if (objOnCell == TypeOnCell.character && mGameTurn == GameTurn.Player && mMouseMode != MouseMode.Attack)
        {
            //mUIManager.ResetPopUp(true);
        }
        else if (mMouseMode == MouseMode.Move || mMouseMode == MouseMode.None)
        {
            mUIManager.ResetPopUp(false);
        }
        if (mMouseMode == MouseMode.Move)
        {
            mPreviewShape = HoverShape.SingleSpot;
        }

        if (mMouseMode != MouseMode.AbilityAttack)
        {
            StartCoroutine(removeAllParticles());
        }


        mSelectedCell = pos;
        //move lightblock to selected postion
        mLightUp.transform.position = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.position;
        mLightUp.transform.rotation = mCurrGrid.rows[pos.y].cols[pos.x].mCellTransform.rotation;

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

        if (mMouseMode == MouseMode.None)
        {
            return;
        }

        //if the selected block is a character show where it can move to
        if ((mCharacterSelected && mGameTurn == GameTurn.Player) || (mEnemySelected && mGameTurn == GameTurn.Enemy && mCanControlEnemies && mEnemySelected))
        {
            mCharacterObj = charObj;
            //MapMoveArea();

            if (mMouseMode == MouseMode.Move && mCharacterObj != null)
            {
                if (!mCharacterObj.mMoved)
                {
                    mLoadingSquares = true;
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
                if (mAttackShape == AttackShape.Area)
                {
                    AreaAttack(mCurrentRange);
                }
                else if (mAttackShape == AttackShape.Cross)
                {
                    CrossAttack(mCurrentRange);
                }
                else if (mAttackShape == AttackShape.Heal)
                {
                    createHealAttack(mSelectedCell, mCurrentRange);
                }
                else if (mAttackShape == AttackShape.OtherCharacter)
                {
                    CreateTargetAttack(mSelectedCell, mCurrentRange);
                }
                else if (mAttackShape == AttackShape.AreaNoCharacters)
                {
                    createAreaNoCharacterAttack(mSelectedCell, mCurrentRange);
                }
                else if (mAttackShape == AttackShape.AllCharacters)
                {
                    CreateCharacterTargets(mSelectedCell);
                }
                else if (mAttackShape == AttackShape.OnCell)
                {
                    CreateOnCellTarget(mSelectedCell);
                }
            }
        }
        else
        {
            mCharacterObj = null;
        }

        //mLightUp.transform.position += new Vector3(0, 1, 0);
    }

    public List<Cell> GetCellsInRange(IntVector2 start, int radius)
    {
        List<Cell> tempList = new List<Cell>();

        int colRadius;
        IntVector2 tempVector = new IntVector2();

        //for (int i = (radius + 1); i > 0; i--)
        //{
        //    colRadius = radius - i;
        //    for (int j = (-colRadius - 1); j < (colRadius + 1); j++)
        //    {

        //        tempVector.x = start.x + j;
        //        tempVector.y = start.y + i;
        //        print("test pos: " + tempVector.x + "," + tempVector.y);
        //        if (IsMovableBlock(tempVector))
        //        {
        //            print("Working Kinda");
        //            tempList.Add(mCurrGrid.rows[tempVector.y].cols[tempVector.x]);
        //        }
        //    }
        //}

        for (int i = (radius + 1); i > (-radius - 1); i--)
        {

            colRadius = radius - Math.Abs(i);
            for (int j = -colRadius; j < (colRadius + 1); j++)
            {

                tempVector.x = start.x + j;
                tempVector.y = start.y + i;

                if (IsOnGrid(tempVector))
                {
                    if (IsMovableBlock(tempVector) || mCurrGrid.rows[tempVector.y].cols[tempVector.x].mTypeOnCell == TypeOnCell.character)
                    {
                        tempList.Add(mCurrGrid.rows[tempVector.y].cols[tempVector.x]);
                    }
                }
            }
        }



        return tempList;
    }

    public void MoveCharacterSlot(IntVector2 newPos, Character ch)
    {
        mCurrGrid.rows[ch.mCellPos.y].cols[ch.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
        mCurrGrid.rows[ch.mCellPos.y].cols[ch.mCellPos.x].mCharacterObj = null;

        ch.mCellPos = newPos;

        mCurrGrid.rows[newPos.y].cols[newPos.x].mTypeOnCell = TypeOnCell.character;
        mCurrGrid.rows[newPos.y].cols[newPos.x].mCharacterObj = ch;
    }



    public void MoveEnemySlot(IntVector2 newPos, Character ch)
    {
        mCurrGrid.rows[ch.mCellPos.y].cols[ch.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
        mCurrGrid.rows[ch.mCellPos.y].cols[ch.mCellPos.x].mEnemyObj = null;

        ch.mCellPos = newPos;

        mCurrGrid.rows[newPos.y].cols[newPos.x].mTypeOnCell = TypeOnCell.enemy;
        mCurrGrid.rows[newPos.y].cols[newPos.x].mEnemyObj = ch;
    }

    #endregion

    #endregion

}

