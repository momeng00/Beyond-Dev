using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour
{
    private InputSystem _registeredInputSystem;
    private EventSystem controlledEventSystem;
    private bool previousSendNavigationEvents;
    private int lastInputFrame = -1;
    private KeyCode lastMoveKey = KeyCode.None;
    private float nextMoveTime;

    private void OnDestroy()
    {
        RestoreEventSystemNavigation();
        if (_registeredInputSystem == null)
            return;

        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.Escape, HandleCancelKey);
        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.RightArrow, HandleRight);
        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.LeftArrow, HandleLeft);
        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.DownArrow, HandleDown);
        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.UpArrow, HandleUp);
        _registeredInputSystem.DeregisterAction(KeyState.Pause, KeyCode.Return, HandleSubmit);

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
        UpdateEventSystemNavigation();
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
        {
            InputSystem.Instance.keyState = KeyState.Play_Key;
            RestoreEventSystemNavigation();
        }
    }

    public void HideLast()
    {
        if (showns.Last != null && showns.Last.Value.inputActionEnabled)
            showns.Last.Value.Hide();
    }

    void Start()
    {
        _registeredInputSystem = InputSystem.Instance;
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.Escape, HandleCancelKey);
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.RightArrow, HandleRight);
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.LeftArrow, HandleLeft);
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.DownArrow, HandleDown);
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.UpArrow, HandleUp);
        _registeredInputSystem.RegisterAction(KeyState.Pause, KeyCode.Return, HandleSubmit);
    }

    private void HandleRight() => DispatchFallbackInput(KeyCode.RightArrow);
    private void HandleLeft() => DispatchFallbackInput(KeyCode.LeftArrow);
    private void HandleDown() => DispatchFallbackInput(KeyCode.DownArrow);
    private void HandleUp() => DispatchFallbackInput(KeyCode.UpArrow);
    private void HandleSubmit() => DispatchFallbackInput(KeyCode.Return);

    private InputSystemUIInputModule GetNavigationModule()
    {
        if (EventSystem.current == null)
            return null;
        InputSystemUIInputModule module =
            EventSystem.current.currentInputModule as InputSystemUIInputModule;
        if (module == null || !module.isActiveAndEnabled)
            return null;
        return module;
    }

    private void HandleCancelKey()
    {
        InputSystemUIInputModule module = GetNavigationModule();
        if (module != null && module.cancel != null && module.cancel.action != null)
            return;
        HandleCancel();
    }

    // UI 액션이 없는 씬에서는 기존 InputSystem 키 등록을 사용한다.
    private void DispatchFallbackInput(KeyCode key)
    {
        InputSystemUIInputModule module = GetNavigationModule();
        if (module != null)
        {
            if (key == KeyCode.Return)
            {
                if (module.submit != null && module.submit.action != null)
                    return;
            }
            else if (module.move != null && module.move.action != null)
            {
                return;
            }
        }
        DispatchInput(key);
    }

    private bool CanDispatchInput()
    {
        return isActiveAndEnabled && lastInputFrame != Time.frameCount &&
            showns.Last != null && showns.Last.Value.inputActionEnabled;
    }

    private void HandleCancel()
    {
        if (!CanDispatchInput())
            return;

        lastInputFrame = Time.frameCount;
        HideLast();
    }

    private void DispatchInput(KeyCode key)
    {
        if (!CanDispatchInput())
            return;

        if (showns.Last.Value is IUIInputHandler handler)
        {
            lastInputFrame = Time.frameCount;
            handler.HandleInput(key);
        }
    }

    private void Update()
    {
        if (showns.Last == null)
            return;

        UpdateEventSystemNavigation();
        ProcessNavigationActions();
    }

    private void ProcessNavigationActions()
    {
        InputSystemUIInputModule module = GetNavigationModule();
        if (module == null || !CanDispatchInput())
        {
            lastMoveKey = KeyCode.None;
            return;
        }

        if (module.cancel != null && module.cancel.action != null &&
            module.cancel.action.WasPerformedThisFrame())
        {
            HandleCancel();
            return;
        }

        Vector2 movement = Vector2.zero;
        if (module.move != null && module.move.action != null)
            movement = module.move.action.ReadValue<Vector2>();
        KeyCode moveKey = GetMoveKey(movement);
        if (moveKey == KeyCode.None)
        {
            lastMoveKey = KeyCode.None;
        }
        else if (moveKey != lastMoveKey || Time.unscaledTime >= nextMoveTime)
        {
            float repeatInterval = module.moveRepeatRate;
            if (moveKey != lastMoveKey)
                repeatInterval = module.moveRepeatDelay;
            lastMoveKey = moveKey;
            nextMoveTime = Time.unscaledTime + repeatInterval;
            DispatchInput(moveKey);
        }

        if (module.submit != null && module.submit.action != null &&
            module.submit.action.WasPerformedThisFrame())
            DispatchInput(KeyCode.Return);
    }

    private static KeyCode GetMoveKey(Vector2 movement)
    {
        if (movement == Vector2.zero)
            return KeyCode.None;
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            if (movement.x > 0f)
                return KeyCode.RightArrow;
            return KeyCode.LeftArrow;
        }
        if (movement.y > 0f)
            return KeyCode.UpArrow;
        return KeyCode.DownArrow;
    }

    private void OnEnable()
    {
        if (showns.Last != null)
            UpdateEventSystemNavigation();
    }

    private void OnDisable()
    {
        RestoreEventSystemNavigation();
    }

    private void UpdateEventSystemNavigation()
    {
        if (controlledEventSystem == EventSystem.current)
            return;

        RestoreEventSystemNavigation();
        controlledEventSystem = EventSystem.current;
        if (controlledEventSystem == null)
            return;

        previousSendNavigationEvents = controlledEventSystem.sendNavigationEvents;
        // 기존 UI 액션을 최상단 창으로 전달하고 EventSystem의 중복 이동/실행만 막는다.
        controlledEventSystem.sendNavigationEvents = false;
    }

    private void RestoreEventSystemNavigation()
    {
        if (controlledEventSystem != null)
            controlledEventSystem.sendNavigationEvents = previousSendNavigationEvents;
        controlledEventSystem = null;
    }
}
