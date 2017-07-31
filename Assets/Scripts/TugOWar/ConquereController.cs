using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ZoneMode
{
    NobodyOwnsIt,
    CharactersOwnIt,
    EnemiesOwnIt,
    Contested
}

[System.Serializable]
public struct Location
{
    public int x;
    public int y;
}

[System.Serializable]
public struct SpecificLocations
{
    public bool UseSpecificSpots;

    [Space(10)]
    public Location[] mZoneSpots;
}



public class ConquereController : MonoBehaviour
{
    /*
    
        Blocks can be concuerable
        Checks if there are other characters on the blocks
        If only one team is on the block, start decreasing the counter of the team
        If there are opposing characters on the blocks it is contested, and counters freeze
        Stays contested until only one team is in the zone
        Counter doesnt go up, only down
        If one of the counter gets to 0, then zone is captured, and there is a winner
        Texture of zone has default zone, your team, and the enemy team textures.
         
    */

    public static ConquereController sInstance = null;

    [Header("Count Numbers")]
    //public Text mCharacterText;
    //public Text mEnemyText;

    [Header("Attributes for the Conquere areas")]

    [Space(10)]
    [Header("The middle of the 3x3 area")]
    public Location mZoneMiddle;

    [Space(10)]
    [Header("Only used for specific locations, used zonepoints to set the locations")]
    public SpecificLocations mLocations;


    [Space(20)]
    [Header("Counters for each team")]
    public int mCharacterTurnCounter;
    public int mEnemyTurnCounter;

    [Space(10)]
    [Header("Textures")]
    public Texture2D mIdleTexture;
    public Texture2D mContestedTexture;
    public Texture2D mCharacterTeamTexture;
    public Texture2D mEnemyTeamTexture;

    Texture2D mCurrentTexture;


    List<Cell> mZoneLocations = new List<Cell>();
    ZoneMode mZoneMode = ZoneMode.NobodyOwnsIt;

    bool mEnteredZone = false;


    GameObject mNeutralConquestPartical;
    GameObject mYourConquestPartical;
    GameObject mEnemyConquestPartical;
    GameObject mContestedConquestPartical;

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

    void Start()
    {
        if (mLocations.UseSpecificSpots)
        {
            GetSpecificAreas();
        }
        else
        {
            GetArea();
        }

        UpdateZone();
    }

    void GetSpecificAreas()
    {
        foreach (Location vect in mLocations.mZoneSpots)
        {
            IntVector2 temp = new IntVector2();

            temp.x = vect.x;
            temp.y = vect.y;

            if (GameManager.sInstance.IsOnGridAndCanMoveTo(temp))
            {
                mZoneLocations.Add(GameManager.sInstance.mCurrGrid.rows[vect.y].cols[vect.x]);
            }
        }
    }

    void UpdateZoneParticals()
    {
        mYourConquestPartical.SetActive(false);
        mEnemyConquestPartical.SetActive(false);
        mContestedConquestPartical.SetActive(false);
        mNeutralConquestPartical.SetActive(false);
        switch (mZoneMode)
        {
            case ZoneMode.NobodyOwnsIt:
                mNeutralConquestPartical.SetActive(true);
                break;
            case ZoneMode.CharactersOwnIt:
                mYourConquestPartical.SetActive(true);
                break;
            case ZoneMode.EnemiesOwnIt:
                mEnemyConquestPartical.SetActive(true);
                break;
            case ZoneMode.Contested:
                mContestedConquestPartical.SetActive(true);
                break;
            default:
                break;
        }
    }

    void GetArea()
    {
        IntVector2 temp = new IntVector2();
        //mZoneLocations.Add(); for adding the middle position if it doesnt get it
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                temp.x = mZoneMiddle.x + dx;
                temp.y = mZoneMiddle.y + dy;
                if (GameManager.sInstance.IsOnGridAndCanMoveTo(temp))
                {
                    mZoneLocations.Add(GameManager.sInstance.mCurrGrid.rows[temp.y].cols[temp.x]);
                }
            }
        }

        if (mZoneLocations.Count < 9)
        {
            Debug.Log("[ConquereController]Zone Didnt get a full.. Count = " + mZoneLocations.Count);
        }

        foreach (Cell item in mZoneLocations)
        {
            //Debug.Log("[ConquereController] Contains Location:" + item.mPos.x + "," + item.mPos.y);
            item.mConquereArea = true;
        }
        Transform middleLocation = GameManager.sInstance.mCurrGrid.rows[mZoneMiddle.y].cols[mZoneMiddle.x].transform;
        Quaternion particalRotation = TexturesManager.sInstance.mNeutralConquestPartical.transform.rotation;
        mNeutralConquestPartical = Instantiate(TexturesManager.sInstance.mNeutralConquestPartical, middleLocation.position, particalRotation);
        mYourConquestPartical = Instantiate(TexturesManager.sInstance.mYourConquestPartical, middleLocation.position, particalRotation);
        mEnemyConquestPartical = Instantiate(TexturesManager.sInstance.mEnemyConquestPartical, middleLocation.position, particalRotation);
        mContestedConquestPartical = Instantiate(TexturesManager.sInstance.mContestedConquestPartical, middleLocation.position, particalRotation);

        mYourConquestPartical.SetActive(false);
        mEnemyConquestPartical.SetActive(false);
        mContestedConquestPartical.SetActive(false);

    }

    void Update()
    {
        //mCharacterText.text = "Character: " + mCharacterTurnCounter;
        //mEnemyText.text = "Enemy: " + mEnemyTurnCounter;
    }

    public void UpdateZone()
    {
        FindZoneMode();
        UpdateTextures();
    }

    void ActivateAllAI()
    {
        AIActor[] actors = FindObjectsOfType(typeof(AIActor)) as AIActor[];
        foreach (AIActor actor in actors)
        {
            actor.mActivationRange = 100;
        }
    }

    void UpdateTextures()
    {
        switch (mZoneMode)
        {
            case ZoneMode.NobodyOwnsIt:
                mCurrentTexture = mIdleTexture;
                break;
            case ZoneMode.CharactersOwnIt:
                if (!mEnteredZone)
                {
                    ActivateAllAI();
                }
                mEnteredZone = true;
                mCurrentTexture = mCharacterTeamTexture;
                break;
            case ZoneMode.EnemiesOwnIt:
                mCurrentTexture = mEnemyTeamTexture;
                break;
            case ZoneMode.Contested:
                mCurrentTexture = mContestedTexture;
                break;
            default:
                break;
        }


        foreach (Cell item in mZoneLocations)
        {
            item.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", mCurrentTexture);
        }

        //Debug.Log("ZoneMode: " + mZoneMode);

    }

    void FindZoneMode()
    {
        int numEnemies = 0;
        int numCharacters = 0;

        foreach (Cell item in mZoneLocations)
        {
            if (item.mCharacterObj != null)
            {
                numCharacters++;
            }
            else if (item.mEnemyObj != null)
            {
                numEnemies++;
            }

        }

        if (numEnemies > 0 && numCharacters == 0)
        {
            //enemy got it
            if (mZoneMode != ZoneMode.EnemiesOwnIt)
            {
                GameSounds.sInstance.PlayAudio("ENEMY_CAPTURED_ZONE");
            }
            mZoneMode = ZoneMode.EnemiesOwnIt;
        }
        else if (numCharacters > 0 && numEnemies == 0)
        {
            //characters got it
            if (mZoneMode != ZoneMode.CharactersOwnIt)
            {
                GameSounds.sInstance.PlayAudio("PLAYER_CAPTURED");
            }

            mZoneMode = ZoneMode.CharactersOwnIt;
        }
        else if (numEnemies > 0 && numCharacters > 0)
        {
            //contested
            if (mZoneMode != ZoneMode.Contested)
            {
                GameSounds.sInstance.PlayAudio("ZONE_CONTESTED");
            }
            mZoneMode = ZoneMode.Contested;
        }
        else
        {
            //nobody owns it
            mZoneMode = ZoneMode.NobodyOwnsIt;
        }

        UpdateZoneParticals();
    }

    public IntVector2 FindOpenSpot()
    {
        foreach (Cell item in mZoneLocations)
        {
            if (item.GetCharacterObject() == null)
            {
                return item.mPos;
            }
        }
        IntVector2 nulled = new IntVector2();
        nulled.x = -1;
        nulled.y = -1;
        return nulled;
    }

    public void DecrementCounters()
    {
        switch (mZoneMode)
        {
            case ZoneMode.NobodyOwnsIt:
                break;
            case ZoneMode.CharactersOwnIt:
                mCharacterTurnCounter--;
                GameSounds.sInstance.PlayAudio("BLUE_STAR");
                break;
            case ZoneMode.EnemiesOwnIt:
                mEnemyTurnCounter--;
                GameSounds.sInstance.PlayAudio("RED_STAR");
                break;
            case ZoneMode.Contested:

                break;
            default:
                break;
        }

        if (mCharacterTurnCounter <= 0)
        {
            //characters won
            GameManager.sInstance.WonGame();
        }
        else if (mEnemyTurnCounter <= 0)
        {
            //enemeies won
            GameManager.sInstance.LostGame();
        }

    }


}
