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
        SceneManager.LoadScene(3);
    }

    public void LoadFireLevel()
    {
        SceneManager.LoadScene(6);

    }

    public void LoadNatureLevel()
    {
        SceneManager.LoadScene(9);

    }

    public void LoadDarkLevel()
    {
        SceneManager.LoadScene(12);

    }

    public void LoadBossLevel()
    {
        SceneManager.LoadScene(15);
    }

}
