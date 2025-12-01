using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : MonoBehaviour, IReset
{
    public bool isOnce;
    private bool hasExecuted;
    public UnityEvent OnAction;
    public UnityEvent OffAction;
    private Switch _switch;
    private void Start()
    {
        _switch = GetComponent<Switch>();
        if (_switch != null)
        {
            _switch.OnSwitchAction += EventAction;
        }
        else
        {
            Debug.Log("스위치가 있는 오브젝트에 연결하도록");
        }

        GameManager.Instance.OnReset += ResetAction;
    }

    private void EventAction(bool state) //넣고 싶은 기능이 있으면 여기다가 적어서 넣을 수 있음.
    {

        if (isOnce && hasExecuted)
            return;

        if (state)
        {
            OnAction?.Invoke();
            if (isOnce)
            {
                hasExecuted = true;
            }
        }
        else
            OffAction?.Invoke();
    }

    public void InitializeReset()
    {
        throw new NotImplementedException();
    }

    public void ResetAction()
    {
        hasExecuted = false;
        OffAction?.Invoke();
    }
}