using System;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private static InputSystem _instance;
    public static InputSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("InputSystem").AddComponent<InputSystem>();
            }
            return _instance;
        }
    }

    private KeyState _keyState;
    public KeyState keyState
    {
        get
        {
            return _keyState;
        }
        set
        {
            _keyState = value;
            if (maps.ContainsKey(value))
            {
                currentMap = maps[value];
            }
        }
    }
    private KeyBinding currentMap;
    private Dictionary<KeyState, KeyBinding> maps = new Dictionary<KeyState, KeyBinding>();
    private void Awake()
    {
        foreach(KeyState state in Enum.GetValues(typeof(KeyState)))
        {
            maps.Add(state, new KeyBinding());
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            keyState = KeyState.Play_Key;
        }
        DoAction();
    }

    public void RegisterAction(KeyState state, KeyCode keyCode, Action act)
    {
        maps[state]._actionKey.Add(keyCode, act);
    }
    public void RegisterAction(KeyState state, string axis, Action<float> act)
    {
        maps[state]._actionAxis.Add(axis, act);
    }
    public void DeregisterAction(KeyState state, KeyCode keyCode, Action act)
    {

    }
    private void DoAction()
    {
        if (currentMap == null)
            return;
        foreach(var pair in currentMap._actionAxis)
        {
            pair.Value.Invoke(Input.GetAxisRaw(pair.Key));
        }

        foreach(var pair in currentMap._actionKey)
        {
            if (Input.GetKeyDown(pair.Key))
                pair.Value.Invoke();
        }
    }
    public void initialize(KeyState state, KeyCode keyCode)
    {

    }
}
public enum KeyState
{
    Play_Key,
    Play_Pad,
    Pause,
}