using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectControl : MonoBehaviour
{

    public Animator mAnimator;

    void Start()
    {

    }

    void Update()
    {

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

    public void LoadIntroLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadRockLevel()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadFireLevel()
    {
        SceneManager.LoadScene(5);

    }

    public void LoadNatureLevel()
    {
        SceneManager.LoadScene(8);

    }

    public void LoadDarkLevel()
    {
        SceneManager.LoadScene(9);

    }

    public void LoadBossLevel()
    {
        SceneManager.LoadScene(12);
    }

}
