using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCutSceneLater : MonoBehaviour
{

    void Start()
    {
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CutSceneManager.sInstance.StartCutscene();
            Destroy(this);
        }
    }
}
