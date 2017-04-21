using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridEditor))]
public class LevelEditor : Editor
{

    public GridEditor mGridEditor;

    Vector2 scrollPos;

    int xPos, yPos;

    int mEnemyStartX = 0;
    int mEnemyStartY = 0;

    int mEnemyHealth = 0;
    int mEnemyDamage = 0;
    int mEnemyMoveDistance = 0;
    int mEnemyDamageDistance = 0;

    int mCurrentSelectedX = 0;
    int mCurrentSelectedY = 0;

    EnemyOnCell mEnemyType;

    Cell mCurrentCell;

    string cellMovementInfo;

    bool mCannotMoveHere;

    void OnEnable()
    {
        mGridEditor = (GridEditor)target;
    }

    void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (mGridEditor.mGrid != null)
        {
            int X = mGridEditor.mGrid.rows[0].cols.Length;
            int Y = mGridEditor.mGrid.rows.Length;

            if(GUILayout.Button("Select Cells"))
            {

                //List<MeshRenderer> meshlist = new List<MeshRenderer>();
                GameObject[] gList = GameObject.FindGameObjectsWithTag("Cell");
                Selection.objects = gList;
            }

            if (GUILayout.Button("Select Melee Minions"))
            {

                List<GameObject> mMeleeEnemies = new List<GameObject>();

                Character[] mList = mGridEditor.mGameManager.mEnemies;

                foreach (Character item in mList)
                {
                    if(item.mAttackType == AttackType.Melee)
                    {
                        mMeleeEnemies.Add(item.gameObject);
                    }
                }

                GameObject[] gList = mMeleeEnemies.ToArray();
                Selection.objects = gList;
            }

            if (GUILayout.Button("Select Ranged Minions"))
            {

                List<GameObject> mMeleeEnemies = new List<GameObject>();

                Character[] mList = mGridEditor.mGameManager.mEnemies;

                foreach (Character item in mList)
                {
                    if (item.mAttackType == AttackType.Ranged)
                    {
                        mMeleeEnemies.Add(item.gameObject);
                    }
                }

                GameObject[] gList = mMeleeEnemies.ToArray();
                Selection.objects = gList;
            }

            EditorGUILayout.BeginHorizontal();

            xPos = EditorGUILayout.IntField("X:", xPos);
            yPos = EditorGUILayout.IntField("Y:", yPos);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Edit"))
            {
                if (xPos >= 0 && xPos <= X && yPos >= 0 && yPos <= Y)
                {
                    EditCell(xPos, yPos);
                }
            }


            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            //if (mGridEditor.mGrid != null)
            //{
            //    for (int y = 0; y < Y; y++)
            //    {
            //        EditorGUILayout.BeginHorizontal();

            //        for (int x = 0; x < X; x++)
            //        {
            //            if (GUILayout.Button(x + "," + y))
            //            {
            //                EditCell(x, y);
            //            }
            //        }

            //        EditorGUILayout.EndHorizontal();

            //    }

            //}
            //EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            //EditorGUILayout.BeginHorizontal();

            if (mCurrentCell != null)
            {
                mCannotMoveHere = mCurrentCell.mCannotMoveHere;

                EditorGUILayout.LabelField("Current Cell: " + mCurrentSelectedX + "," + mCurrentSelectedY);

                cellMovementInfo = mCannotMoveHere ? "Cannot Move Here" : "Can Move Here";

                if (GUILayout.Button(cellMovementInfo))
                {
                    mCannotMoveHere = !mCannotMoveHere;
                }

                mCurrentCell.mCannotMoveHere = mCannotMoveHere;

            }
            else
            {
                EditorGUILayout.LabelField("No Cell Selected");
            }

            //EditorGUILayout.EndHorizontal();


            GUILayout.Space(30);

            EditorGUILayout.LabelField("EnemyAddition");
            EditorGUILayout.BeginHorizontal();
            mEnemyStartX = EditorGUILayout.IntField("Position On Grid (X):", mEnemyStartX);
            mEnemyStartY = EditorGUILayout.IntField("(Y):", mEnemyStartY);
            EditorGUILayout.EndHorizontal();

            mEnemyHealth = EditorGUILayout.IntField("Health:", mEnemyHealth);
            mEnemyDamage = EditorGUILayout.IntField("Damage:", mEnemyDamage);
            mEnemyMoveDistance = EditorGUILayout.IntField("Move Distance:", mEnemyMoveDistance);
            mEnemyDamageDistance = EditorGUILayout.IntField("Damage Distance(if melee set to 1):", mEnemyDamageDistance);

            EditorGUILayout.BeginHorizontal();

            mEnemyType = (EnemyOnCell)EditorGUILayout.EnumPopup("EnemyType: ", mEnemyType);

            if (GUILayout.Button("Create Enemy"))
            {
                switch (mEnemyType)
                {
                    case EnemyOnCell.None:
                        break;
                    case EnemyOnCell.RangedMinion:
                        if (mGridEditor.mRangedEnemy != null)
                        {
                            GameObject temp = Instantiate(mGridEditor.mRangedEnemy);
                            Character tChar = temp.GetComponent<Character>();
                            tChar.mCellPos.x = mEnemyStartX;
                            tChar.mCellPos.y = mEnemyStartY;
                            tChar.mHealth = mEnemyHealth;
                            tChar.mDamage = mEnemyDamage;
                            tChar.mMoveDistance = mEnemyMoveDistance;
                            tChar.mDamageDistance = mEnemyDamageDistance;
                        }
                        break;
                    case EnemyOnCell.MeleeMinion:
                        if (mGridEditor.mMeleeEnemy != null)
                        {
                            GameObject temp = Instantiate(mGridEditor.mMeleeEnemy);
                            Character tChar = temp.GetComponent<Character>();
                            tChar.mCellPos.x = mEnemyStartX;
                            tChar.mCellPos.y = mEnemyStartY;
                            tChar.mHealth = mEnemyHealth;
                            tChar.mDamage = mEnemyDamage;
                            tChar.mMoveDistance = mEnemyMoveDistance;
                            tChar.mDamageDistance = mEnemyDamageDistance;
                        }
                        break;
                    case EnemyOnCell.Boss:
                        if (mGridEditor.mBoss != null)
                        {
                            GameObject temp = Instantiate(mGridEditor.mBoss);
                            Character tChar = temp.GetComponent<Character>();
                            tChar.mCellPos.x = mEnemyStartX;
                            tChar.mCellPos.y = mEnemyStartY;
                            tChar.mHealth = mEnemyHealth;
                            tChar.mDamage = mEnemyDamage;
                            tChar.mMoveDistance = mEnemyMoveDistance;
                            tChar.mDamageDistance = mEnemyDamageDistance;
                        }
                        break;
                    default:
                        break;
                }

            }

            EditorGUILayout.EndHorizontal();

        }

    }


    //void Update()
    //{
    //    if(window != null && !windowInit)
    //    {
    //        Debug.Log("Here");
    //        window.mEditor = this;

    //        windowInit = true;
    //    }
    //}

    void EditCell(int x, int y)
    {


        mCurrentSelectedX = x;
        mCurrentSelectedY = y;

        mCurrentCell = mGridEditor.GetCell(x, y);

        //CellWindow window = (CellWindow)EditorWindow.GetWindow(typeof(CellWindow));
        //windowInit = false;
        //if (mGridEditor.GridCreated && mGridEditor.mGrid != null)
        //{
        //    //open editable window
        //}
    }
}
