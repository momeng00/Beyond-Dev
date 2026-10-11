using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private InputSystem _registeredInputSystem;

    private void OnDestroy()
    {
        if (_registeredInputSystem == null)
            return;

        _registeredInputSystem.DeregisterAction(KeyState.Pause,KeyCode.Escape,HideLast);

        _registeredInputSystem = null;
    }


    //싱글톤 사용
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<UIManager>();
                if (_instance == null)
                    _instance = new GameObject("UIManagement").AddComponent<UIManager>();
            }
            return _instance;
        }
    }
    // 등록된 모든 UI
    public Dictionary<Type, IUI> uis = new Dictionary<Type, IUI>();
    // 현재 보여지고있는 팝업 UI 들
    public LinkedList<IUI> showns = new LinkedList<IUI>(); 
    
    public T Get<T>()
        where T : IUI
    {
        if (uis.TryGetValue(typeof(T), out IUI ui))
            return (T)ui;
        else
            throw new Exception($"[UIManager] : {typeof(T)} has not been registered but you tried to get it ..");
    }
    public void Register(IUI ui)
    {
        Type type = ui.GetType();
        if (uis.TryAdd(type, ui) == false)
            throw new Exception($"[UIManager] : {type} already registered but you tried to add it again..!");
        Debug.Log($"[UIManager] : Registered {type}.");
    }
    private Coroutine enableNextUICoroutine;

    public void Push(IUI ui)
    {
        if (ui == null) return;
        InputSystem.Instance.keyState = KeyState.Pause;
        if (showns.Last?.Value == ui) return;
        int order;
        if (showns.Last == null)
        {
            order = 0;
        }
        else
        {
            order = showns.Last.Value.sortingOrder + 1;
        }
        if (showns.Last != null)
        {
            showns.Last.Value.inputActionEnabled = false;
            showns.Last.Value.SetVisible(false);
        }
        showns.Remove(ui);
        showns.AddLast(ui);
        ui.sortingOrder = order;
        ui.inputActionEnabled = false;
        ui.SetVisible(true);
        ScheduleInputHandoff();
    }

    public void Pop(IUI ui)
    {
        if (!showns.Contains(ui)) return;
        bool wasTop = showns.Last.Value == ui;
        ui.inputActionEnabled = false;
        ui.SetVisible(false);
        showns.Remove(ui);
        if (wasTop)
        {
            if (showns.Last != null)
                showns.Last.Value.SetVisible(true);
            ScheduleInputHandoff();
        }
    }

    private void ScheduleInputHandoff()
    {
        if (enableNextUICoroutine != null) StopCoroutine(enableNextUICoroutine);
        if (showns.Last != null) showns.Last.Value.inputActionEnabled = false;
        // Keep UI input selected until the closing key's frame has passed.
        InputSystem.Instance.keyState = KeyState.Pause;
        enableNextUICoroutine = StartCoroutine(EnableNextUI());
    }

    private IEnumerator EnableNextUI()
    {
        yield return null;
        enableNextUICoroutine = null;
        if (showns.Last != null)
            showns.Last.Value.inputActionEnabled = true;
        else
            InputSystem.Instance.keyState = KeyState.Play_Key;
    }

    public void HideLast()
    {
        if (showns.Last != null && showns.Last.Value.inputActionEnabled)
            showns.Last.Value.Hide();
    }

    void Start()
    {
        _registeredInputSystem = InputSystem.Instance;
        _registeredInputSystem.RegisterAction(KeyState.Pause,KeyCode.Escape,HideLast);
    }

}