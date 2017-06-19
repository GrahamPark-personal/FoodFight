using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectControl : MonoBehaviour
{
    public GameObject mActBar;
    public GameObject mLoading;
    public Animator mAnimator;

    public AudioSource mMusic;

    int currLevelStart = 0;
    bool mActBarUp = false;

    public float mCurrVolume;
    public float mTime;

    void Start()
    {
        mActBar.SetActive(false);
        mLoading.SetActive(false);
        mMusic.volume = mCurrVolume;
    }

    void Update()
    {
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
        currLevelStart = 1;
        mActBar.SetActive(true);
        mActBarUp = true;
    }

    public void LoadRockLevel()
    {
        currLevelStart = 3;
        mActBar.SetActive(true);
        mActBarUp = true;

    }

    public void LoadFireLevel()
    {
        currLevelStart = 6;
        mActBar.SetActive(true);
        mActBarUp = true;


    }

    public void LoadNatureLevel()
    {
        currLevelStart = 9;
        mActBar.SetActive(true);
        mActBarUp = true;

    }

    public void LoadDarkLevel()
    {
        currLevelStart = 12;
        mActBar.SetActive(true);
        mActBarUp = true;

    }

    public void LoadBossLevel()
    {
        currLevelStart = 15;
        mActBar.SetActive(true);
        mActBarUp = true;

    }

}
