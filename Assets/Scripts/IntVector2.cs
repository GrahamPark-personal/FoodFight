using UnityEngine;
using System.Collections;

[System.Serializable]
public struct IntVector2
{
    public int x;
    public int y;

    public int F;
    public int G;
    public int H;

    public IntVector2[] parent;
}
