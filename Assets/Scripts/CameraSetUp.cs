using UnityEngine;
using System.Collections;

public class CameraSetUp : MonoBehaviour {

	void Start ()
    {
        gameObject.GetComponent<Camera>().transparencySortMode = TransparencySortMode.Perspective;

    }
	

	void Update ()
    {
	
	}
}
