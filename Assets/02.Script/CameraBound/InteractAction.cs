using System;
using UnityEngine;
using UnityEngine.Events;

public enum InteractValueType
{
    Bool,
    Int,
    Float,
    String
}

[Serializable]
public class BoolEvent : UnityEvent<bool> { }

[Serializable]
public class IntEvent : UnityEvent<int> { }

[Serializable]
public class FloatEvent : UnityEvent<float> { }

[Serializable]
public class StringEvent : UnityEvent<string> { }

[Serializable]
public class InteractAction
{
    public InteractValueType type;

    public bool boolValue;
    public int intValue;
    public float floatValue;
    public string stringValue;

    public BoolEvent onBool;
    public IntEvent onInt;
    public FloatEvent onFloat;
    public StringEvent onString;

    public void Execute()
    {
        switch (type)
        {
            case InteractValueType.Bool:
                onBool?.Invoke(boolValue);
                break;

            case InteractValueType.Int:
                onInt?.Invoke(intValue);
                break;

            case InteractValueType.Float:
                onFloat?.Invoke(floatValue);
                break;

            case InteractValueType.String:
                onString?.Invoke(stringValue);
                break;
        }
    }
}