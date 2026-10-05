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
                _instance = FindAnyObjectByType<InputSystem>();
                if (_instance == null)
                {
                    _instance = new GameObject("InputSystem").AddComponent<InputSystem>();
                }
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
            Debug.Log(InputSystem.Instance.keyState);
            if (maps.ContainsKey(value))
            {
                currentMap = maps[value];
            }
        }
    }
    private KeyBinding currentMap;
    private Dictionary<KeyState, KeyBinding> maps = new Dictionary<KeyState, KeyBinding>();
    private readonly List<string> axisKeys = new List<string>();
    private readonly List<KeyCode> keyCodes = new List<KeyCode>();
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
        keyState = KeyState.Play_Key; 
    }

    // Update is called once per frame
    void Update()
    {
        DoAction();
    }

    public void RegisterAction(KeyState state, KeyCode keyCode, Action act)
    {
        if (act == null)
            return;

        KeyBinding map = maps[state];
        map._actionKey.TryGetValue(keyCode, out Action registered);
        map._actionKey[keyCode] = registered + act;
    }
    public void RegisterAction(KeyState state, string axis, Action<float> act)
    {
        if (act == null)
            return;

        KeyBinding map = maps[state];
        map._actionAxis.TryGetValue(axis, out Action<float> registered);
        map._actionAxis[axis] = registered + act;
    }
    public void DeregisterAction(KeyState state, KeyCode keyCode, Action act)
    {
        if (act == null ||
            !maps.TryGetValue(state, out KeyBinding map) ||
            !map._actionKey.TryGetValue(keyCode, out Action registered))
        {
            return;
        }

        registered -= act;
        if (registered == null)
            map._actionKey.Remove(keyCode);
        else
            map._actionKey[keyCode] = registered;
    }
    public void DeregisterAction(KeyState state, string axis, Action<float> act)
    {
        if (act == null ||
            !maps.TryGetValue(state, out KeyBinding map) ||
            !map._actionAxis.TryGetValue(axis, out Action<float> registered))
        {
            return;
        }

        registered -= act;
        if (registered == null)
            map._actionAxis.Remove(axis);
        else
            map._actionAxis[axis] = registered;
    }
    private void DoAction()
    {
        KeyBinding map = currentMap;
        if (map == null)
            return;

        axisKeys.Clear();
        keyCodes.Clear();
        axisKeys.AddRange(map._actionAxis.Keys);
        keyCodes.AddRange(map._actionKey.Keys);

        foreach (string axis in axisKeys)
        {
            if (currentMap != map)
                return;

            if (map._actionAxis.TryGetValue(axis, out Action<float> action))
                action?.Invoke(Input.GetAxisRaw(axis));
        }

        foreach (KeyCode keyCode in keyCodes)
        {
            if (currentMap != map)
                return;

            if (Input.GetKeyDown(keyCode) &&
                map._actionKey.TryGetValue(keyCode, out Action action))
            {
                action?.Invoke();
            }
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
