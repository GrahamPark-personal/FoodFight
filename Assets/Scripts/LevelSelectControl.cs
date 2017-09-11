using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectControl : MonoBehaviour
{
    public GameObject[] mLevelsObjs;
    public GameObject[] mActsObjs;

    public GameObject mLoading;
    public Animator mAnimator;

    public AudioSource mMusic;

    int currLevelStart = 0;
    bool mActBarUp = false;

    public float mCurrVolume;
    public float mTime;

    void Start()
    {
        mLoading.SetActive(false);
        mMusic.volume = mCurrVolume;

        foreach (GameObject obj in mActsObjs)
        {
            obj.SetActive(false);
        }
    }

    void SwitchActiveObjs()
    {
        mActBarUp = !mActBarUp;

        foreach (GameObject obj in mLevelsObjs)
        {
            obj.SetActive(!mActBarUp);
        }

        foreach (GameObject obj in mActsObjs)
        {
            obj.SetActive(mActBarUp);
        }
    }

    void Update()
    {

        if ((Input.GetMouseButtonDown(1) && mActBarUp ))
        {
            SwitchActiveObjs();
        }


        mCurrVolume =  Mathf.Lerp(mCurrVolume, 1.0f, mTime * Time.deltaTime);
        mMusic.volume = mCurrVolume;
    }



    public void OpenLevelSelect()
    {
        mAnimator.SetInteger("anim", 1);
    }

    public void CloseLevelSelect()
    {
        mAnimator.SetInteger("anim", 0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadAct1()
    {
        mLoading.SetActive(true);
        SceneManager.LoadScene(currLevelStart);
    }

    public void LoadAct2()
    {
        mLoading.SetActive(true);
        SceneManager.LoadScene(currLevelStart+1);
    }

    public void LoadAct3()
    {
        mLoading.SetActive(true);
        SceneManager.LoadScene(currLevelStart+2);
    }

    public void LoadIntroLevel()
    {
        mLoading.SetActive(true);
        SceneManager.LoadScene(1);
    }

    public void LoadRockLevel()
    {
        SwitchActiveObjs();
        currLevelStart = 3;

    }

    public void LoadFireLevel()
    {
        SwitchActiveObjs();
        currLevelStart = 6;


    }

    public void LoadNatureLevel()
    {
        SwitchActiveObjs();
        currLevelStart = 9;

    }

    public void LoadDarkLevel()
    {
        SwitchActiveObjs();
        currLevelStart = 12;

    }

    public void LoadBossLevel()
    {
        SwitchActiveObjs();
        currLevelStart = 15;
    }

}
