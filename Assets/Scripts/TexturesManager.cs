using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturesManager : MonoBehaviour
{

    public static TexturesManager sInstance = null;

    public Texture2D mBlankTexture;

    public GameObject mNeutralConquestPartical;
    public GameObject mYourConquestPartical;
    public GameObject mEnemyConquestPartical;
    public GameObject mContestedConquestPartical;

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
        
    }

    void Update()
    {

    }
}
