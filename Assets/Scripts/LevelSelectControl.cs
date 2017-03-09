using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
        SceneManager.LoadScene(3);

    }

    public void LoadNatureLevel()
    {
        SceneManager.LoadScene(4);

    }

    public void LoadDarkLevel()
    {
        SceneManager.LoadScene(5);

    }

    public void LoadBossLevel()
    {
        SceneManager.LoadScene(6);

    }

}
