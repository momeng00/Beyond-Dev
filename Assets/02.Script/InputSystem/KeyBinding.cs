using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinding
{
    public Dictionary<KeyCode, Action> _actionKey = new Dictionary<KeyCode, Action>();
    public Dictionary<string, Action<float>> _actionAxis = new Dictionary<string, Action<float>>();
    
    //public KeyCode GetKeyCodeForAction(GameAction actionID)
    //{
    //    foreach (var pair in keyCodeToActionMap)
    //    {
    //        if (pair.Value == actionID)
    //        {
    //            return pair.Key;
    //        }
    //    }
    //    return KeyCode.None;
    //}
    

}