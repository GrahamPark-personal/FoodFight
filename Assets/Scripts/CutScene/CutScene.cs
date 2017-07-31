using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CutSceneSide
{
    Left,
    Right
}


[System.Serializable]
public struct Phrase
{
    public bool mBlank;
    public CharacterType mCharacter;
    public CutSceneSide mSide;
    public string mSentence;
    public bool mRemoveOtherCharacter;
    public AudioClip[] mPhraseAudio;
}


public class CutScene : MonoBehaviour
{
    public CharacterType mLeftCharacter;
    public CharacterType mRightCharacter;
    public Phrase[] mConvo;
}
