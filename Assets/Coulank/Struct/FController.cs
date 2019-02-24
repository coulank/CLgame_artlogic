using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FModifierKeys
{
    public bool Ctrl;
    public bool Alt;
    public bool Shift;
    public void Clear()
    {
        Ctrl = false; Alt = false; Shift = false;
    }
}
public class FController
{

}
