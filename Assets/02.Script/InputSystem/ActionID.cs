using System.Collections.Generic;
using UnityEngine;

public enum GameAction
{
    None,
    MoveLeft,
    MoveRight,
    Jump,
    ReStart,
    Interact,
}
public class DefaultKeySetting
{
    public Dictionary<KeyCode, GameAction> defaultKeySet = new Dictionary<KeyCode, GameAction>();
    public Dictionary<KeyCode, GameAction> KeyDefaultSet()
    {
        return new Dictionary<KeyCode, GameAction>()
        {
            { KeyCode.Space,GameAction.Jump },
            { KeyCode.R,GameAction.ReStart },
            { KeyCode.E,GameAction.Interact },
        };
    }
    public Dictionary<KeyCode,GameAction> PadDefaultSet()
    {
        return new Dictionary<KeyCode, GameAction>()
        {
        };
    }
}