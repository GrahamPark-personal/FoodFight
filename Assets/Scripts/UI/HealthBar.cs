using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    Slider mSlider;
    Character mCharacter;

    void Start()
    {
        mSlider = GetComponentInChildren<Slider>();
        mCharacter = GetComponentInParent<Character>();
        mSlider.maxValue = mCharacter.mHealth;
        mSlider.minValue = 0;
        mSlider.value = mCharacter.mHealth;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        mSlider.value = mCharacter.mHealth;
    }
}
