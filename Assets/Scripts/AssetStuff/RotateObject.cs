using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    public Vector3 rotation;
    public float mSpeed;

    void Update()
    {
        transform.Rotate(rotation * mSpeed * Time.deltaTime);
    }
}
